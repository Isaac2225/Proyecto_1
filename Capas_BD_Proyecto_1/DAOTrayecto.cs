using IDAO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Modelo;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Text;

namespace DAO
{
    public class DAOTrayecto :IDAOTrayecto
    {
        private readonly MyDBContext _context;

        public DAOTrayecto(MyDBContext c)
        {
            _context = c;
        }

        public Trayecto GetTrayecto(int id)
        {
            var connexio = _context.Database.GetDbConnection();
            if (connexio.State == System.Data.ConnectionState.Closed)
            {
                connexio.Open();
            }

            using (var consulta = connexio.CreateCommand())
            {
                if (consulta is OracleCommand oc)
                {
                    oc.BindByName = true;
                }

                consulta.CommandText = @"select id, nom, datacreacio, validat, usuari_id, mitja_id
                                   from trajecte
                                  where id = :ID";

                consulta.Transaction = _context.Database.CurrentTransaction?.GetDbTransaction();

                Utils.CrearParametre(consulta, id, "ID", System.Data.DbType.Int32);

                using (var reader = consulta.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return LeerTrayecto(reader);
                    }
                }
            }

            return null;
        }

        public ObservableCollection<Trayecto> GetTrayectos(
            string origen = "",
            string destino = "",
            int offset = 0,
            int rows_per_page = -1,
            TimeSpan? horaSalida = null,     
            TimeSpan? horaLlegada = null)    
        {
            ObservableCollection<Trayecto> resultat = new ObservableCollection<Trayecto>();

            if (horaSalida.HasValue && horaLlegada.HasValue && horaLlegada.Value <= horaSalida.Value)
            {
                return resultat;
            }

            var connexio = _context.Database.GetDbConnection();
            if (connexio.State == System.Data.ConnectionState.Closed)
            {
                connexio.Open();
            }

            using (var consulta = connexio.CreateCommand())
            {
                if (consulta is OracleCommand oc)
                {
                    oc.BindByName = true;
                }

                // JOINs y filtros de hora: solo se añaden si el usuario los ha indicado
                string joinsHores = "";
                string filtresHores = "";

                if (horaSalida.HasValue)
                {
                    joinsHores += @"
                    JOIN parada po ON po.trajecte_id = t.id
                                  AND po.estacio_id = t.estacio_origen_id";
                    filtresHores += @"
                    AND TO_CHAR(po.hora_sortida, 'HH24:MI') >= :HORA_SORTIDA";
                }

                if (horaLlegada.HasValue)
                {
                    joinsHores += @"
                    JOIN parada pd ON pd.trajecte_id = t.id
                                  AND pd.estacio_id = t.estacio_desti_id";
                    filtresHores += @"
                    AND TO_CHAR(pd.hora_arribada, 'HH24:MI') <= :HORA_ARRIBADA";
                }

                if (horaSalida.HasValue && horaLlegada.HasValue)
                {
                    filtresHores += @"
                    AND pd.hora_arribada > po.hora_sortida";
                }

                consulta.CommandText = $@"
                    SELECT
                    t.*,
                    eo.nom_es       AS nom_origen,
                    ed.nom_es       AS nom_desti,
                    u.email         AS u_email,
                    u.nom           AS u_nom,
                    u.password      AS u_password,
                    u.tipus         AS u_tipus,
                    u.data_registre AS u_dataregistre,
                    m.nom           AS m_nom
                    FROM trajecte t
                    JOIN estacio eo ON eo.id_es = t.estacio_origen_id
                    JOIN estacio ed ON ed.id_es = t.estacio_desti_id
                    LEFT JOIN usuari u ON u.id = t.usuari_id
                    LEFT JOIN mitja_transport m ON m.id = t.mitja_id
                    {joinsHores}
                    WHERE UPPER(eo.nom_es) LIKE UPPER(:ORIGEN)
                    AND UPPER(ed.nom_es) LIKE UPPER(:DESTI)
                    {filtresHores}
                    ORDER BY t.id";

                if (rows_per_page > 0)
                {
                    consulta.CommandText += $" offset {offset} rows fetch next {rows_per_page} rows only";
                }

                consulta.Transaction = _context.Database.CurrentTransaction?.GetDbTransaction();

                Utils.CrearParametre(consulta, "%" + origen + "%", "ORIGEN", System.Data.DbType.String);
                Utils.CrearParametre(consulta, "%" + destino + "%", "DESTI", System.Data.DbType.String);

                if (horaSalida.HasValue)
                {
                    Utils.CrearParametre(consulta, horaSalida.Value.ToString(@"hh\:mm"), "HORA_SORTIDA", System.Data.DbType.String);
                }
                if (horaLlegada.HasValue)
                {
                    Utils.CrearParametre(consulta, horaLlegada.Value.ToString(@"hh\:mm"), "HORA_ARRIBADA", System.Data.DbType.String);
                }

                using (var reader = consulta.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        resultat.Add(LeerTrayecto(reader));
                    }
                }
            }

            return resultat;
        }

        public ObservableCollection<Trayecto> GetTrayectosValidados(
            string origen = "",
            string destino = "",
            int offset = 0,
            int rows_per_page = -1,
            TimeSpan? horaSalida = null,
            TimeSpan? horaLlegada = null)
        {
            ObservableCollection<Trayecto> resultat = new ObservableCollection<Trayecto>();

            if (horaSalida.HasValue && horaLlegada.HasValue && horaLlegada.Value <= horaSalida.Value)
            {
                return resultat;
            }

            var connexio = _context.Database.GetDbConnection();
            if (connexio.State == System.Data.ConnectionState.Closed)
            {
                connexio.Open();
            }

            using (var consulta = connexio.CreateCommand())
            {
                if (consulta is OracleCommand oc)
                {
                    oc.BindByName = true;
                }

                // JOINs y filtros de hora: solo se añaden si el usuario los ha indicado
                string joinsHores = "";
                string filtresHores = "";

                if (horaSalida.HasValue)
                {
                    joinsHores += @"
                    JOIN parada po ON po.trajecte_id = t.id
                                  AND po.estacio_id = t.estacio_origen_id";
                    filtresHores += @"
                    AND TO_CHAR(po.hora_sortida, 'HH24:MI') >= :HORA_SORTIDA";
                }

                if (horaLlegada.HasValue)
                {
                    joinsHores += @"
                    JOIN parada pd ON pd.trajecte_id = t.id
                                  AND pd.estacio_id = t.estacio_desti_id";
                    filtresHores += @"
                    AND TO_CHAR(pd.hora_arribada, 'HH24:MI') <= :HORA_ARRIBADA";
                }

                if (horaSalida.HasValue && horaLlegada.HasValue)
                {
                    filtresHores += @"
                    AND pd.hora_arribada > po.hora_sortida";
                }

                consulta.CommandText = $@"
                    SELECT
                    t.*,
                    eo.nom_es       AS nom_origen,
                    ed.nom_es       AS nom_desti,
                    u.email         AS u_email,
                    u.nom           AS u_nom,
                    u.password      AS u_password,
                    u.tipus         AS u_tipus,
                    u.data_registre AS u_dataregistre,
                    m.nom           AS m_nom
                    FROM trajecte t
                    JOIN estacio eo ON eo.id_es = t.estacio_origen_id
                    JOIN estacio ed ON ed.id_es = t.estacio_desti_id
                    LEFT JOIN usuari u ON u.id = t.usuari_id
                    LEFT JOIN mitja_transport m ON m.id = t.mitja_id
                    {joinsHores}
                    WHERE UPPER(eo.nom_es) LIKE UPPER(:ORIGEN)
                    AND UPPER(ed.nom_es) LIKE UPPER(:DESTI) and validat = 1
                    {filtresHores}
                    ORDER BY t.id";

                if (rows_per_page > 0)
                {
                    consulta.CommandText += $" offset {offset} rows fetch next {rows_per_page} rows only";
                }

                consulta.Transaction = _context.Database.CurrentTransaction?.GetDbTransaction();

                Utils.CrearParametre(consulta, "%" + origen + "%", "ORIGEN", System.Data.DbType.String);
                Utils.CrearParametre(consulta, "%" + destino + "%", "DESTI", System.Data.DbType.String);

                if (horaSalida.HasValue)
                {
                    Utils.CrearParametre(consulta, horaSalida.Value.ToString(@"hh\:mm"), "HORA_SORTIDA", System.Data.DbType.String);
                }
                if (horaLlegada.HasValue)
                {
                    Utils.CrearParametre(consulta, horaLlegada.Value.ToString(@"hh\:mm"), "HORA_ARRIBADA", System.Data.DbType.String);
                }

                using (var reader = consulta.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        resultat.Add(LeerTrayecto(reader));
                    }
                }
            }

            return resultat;
        }

        // Método auxiliar para no repetir el mapeo
        private Trayecto LeerTrayecto(DbDataReader reader)
        {
            int id = Convert.ToInt32(reader["id"]);
            string nom = reader.GetString(reader.GetOrdinal("nom"));
            DateTime data = reader.GetDateTime(reader.GetOrdinal("datacreacio"));

            // Usuario (puede ser NULL)
            Usuario usuario = null;
            int ordUsu = reader.GetOrdinal("usuari_id");
            if (!reader.IsDBNull(ordUsu))
            {
                string tipoStr = reader.IsDBNull(reader.GetOrdinal("u_tipus"))
                    ? " " : reader.GetString(reader.GetOrdinal("u_tipus"));

                usuario = new Usuario(
                    Convert.ToInt32(reader[ordUsu]),
                    reader.GetString(reader.GetOrdinal("u_email")),
                    reader.GetString(reader.GetOrdinal("u_nom")),
                    reader.GetString(reader.GetOrdinal("u_password")),
                    tipoStr.Length > 0 ? tipoStr[0] : ' ',
                    reader.GetDateTime(reader.GetOrdinal("u_dataregistre")));
            }

            // Medio de transporte (puede ser NULL)
            MedioTransporte medio = null;
            int ordMitja = reader.GetOrdinal("mitja_id");
            if (!reader.IsDBNull(ordMitja))
            {
                medio = new MedioTransporte(
                    Convert.ToInt32(reader[ordMitja]),
                    reader.GetString(reader.GetOrdinal("m_nom")));
            }

            var trayecto = new Trayecto(id, nom, data, usuario, medio);

            int ordValidat = reader.GetOrdinal("validat");
            trayecto.Validado = !reader.IsDBNull(ordValidat) && Convert.ToInt32(reader[ordValidat]) == 1;

            // Origen y destino (se asignan por propiedad)
            trayecto.Origen = new Estacion(
                Convert.ToInt32(reader["estacio_origen_id"]),
                reader.GetString(reader.GetOrdinal("nom_origen")),
                0, 0, null);

            trayecto.Destino = new Estacion(
                Convert.ToInt32(reader["estacio_desti_id"]),
                reader.GetString(reader.GetOrdinal("nom_desti")),
                0, 0, null);

            return trayecto;
        }

        public int GetNumeroTrayectos(string id_filtre, string nom_filtre)
        {
            var connexio = _context.Database.GetDbConnection();
            if (connexio.State == System.Data.ConnectionState.Closed)
            {
                connexio.Open();
            }

            using (var consulta = connexio.CreateCommand())
            {
                if (consulta is OracleCommand oc)
                {
                    oc.BindByName = true;
                }

                consulta.CommandText = @"select count(1) from trajecte
                                  where (:ID_TEXT = -1 or id = :ID_TEXT)
                                    and upper(nom) like upper(:NOM_TEXT)";

                consulta.Transaction = _context.Database.CurrentTransaction?.GetDbTransaction();

                int id_filtre_int = -1;
                if (!Int32.TryParse(id_filtre, out id_filtre_int))
                {
                    id_filtre_int = -1;
                }

                Utils.CrearParametre(consulta, id_filtre_int, "ID_TEXT", System.Data.DbType.Int32);
                Utils.CrearParametre(consulta, "%" + nom_filtre + "%", "NOM_TEXT", System.Data.DbType.String);

                return Convert.ToInt32(consulta.ExecuteScalar());
            }
        }

        public bool InsertTrayecto(Trayecto t)
        {
            var connexio = _context.Database.GetDbConnection();

            if (connexio.State == System.Data.ConnectionState.Closed)
            {
                connexio.Open();
            }

            var transaccio = _context.Database.CurrentTransaction;
            if (transaccio == null) throw new Exception("NO TRANSACTION");

            using (var consulta = connexio.CreateCommand())
            {
                if (consulta is OracleCommand oc)
                {
                    oc.BindByName = true;
                }

                consulta.CommandText = @"insert into trajecte (nom, datacreacio, validat, usuari_id, mitja_id)
                                 values (:NOM, :DATACREACIO, :VALIDAT, :USUARI_ID, :MITJA_ID)
                                 returning id into :NEW_ID";

                consulta.Transaction = transaccio.GetDbTransaction();

                Utils.CrearParametre(consulta, t.Nombre, "NOM", System.Data.DbType.String);
                Utils.CrearParametre(consulta, t.FechaCreacion, "DATACREACIO", System.Data.DbType.Date);
                Utils.CrearParametre(consulta, t.Validado ? 1 : 0, "VALIDAT", System.Data.DbType.Int32); 

                // Extraemos el ID de los objetos anidados
                Utils.CrearParametre(consulta, t.Id_usuario?.Id, "USUARI_ID", System.Data.DbType.Int32);
                Utils.CrearParametre(consulta, t.Medio?.Id, "MITJA_ID", System.Data.DbType.Int32);

                var pId = consulta.CreateParameter();
                pId.ParameterName = "NEW_ID";
                pId.DbType = System.Data.DbType.Int32;
                pId.Direction = System.Data.ParameterDirection.Output;
                consulta.Parameters.Add(pId);

                int numeroFilesAfectades = consulta.ExecuteNonQuery();

                if (numeroFilesAfectades != 1)
                {
                    transaccio.Rollback();
                    return false;
                }
                else
                {
                    t.Id = Convert.ToInt32(pId.Value);
                    return true;
                }
            }
        }

        public bool UpdateTrayecto(Trayecto t)
        {
            var connexio = _context.Database.GetDbConnection();

            if (connexio.State == System.Data.ConnectionState.Closed)
            {
                connexio.Open();
            }

            var transaccio = _context.Database.CurrentTransaction;
            if (transaccio == null) throw new Exception("NO TRANSACTION");

            using (var consulta = connexio.CreateCommand())
            {
                if (consulta is OracleCommand oc)
                {
                    oc.BindByName = true;
                }

                consulta.CommandText = @"update trajecte
                                    set nom = :NOM,
                                        datacreacio = :DATACREACIO,
                                        validat = :VALIDAT,
                                        usuari_id = :USUARI_ID,
                                        mitja_id = :MITJA_ID
                                  where id = :ID";

                consulta.Transaction = transaccio.GetDbTransaction();

                Utils.CrearParametre(consulta, t.Nombre, "NOM", System.Data.DbType.String);
                Utils.CrearParametre(consulta, t.FechaCreacion, "DATACREACIO", System.Data.DbType.Date);
                Utils.CrearParametre(consulta, t.Validado ? 1 : 0, "VALIDAT", System.Data.DbType.Int32);
                Utils.CrearParametre(consulta, t.Id_usuario?.Id, "USUARI_ID", System.Data.DbType.Int32);
                Utils.CrearParametre(consulta, t.Medio?.Id, "MITJA_ID", System.Data.DbType.Int32);
                Utils.CrearParametre(consulta, t.Id, "ID", System.Data.DbType.Int32);

                int numeroFilesAfectades = consulta.ExecuteNonQuery();

                if (numeroFilesAfectades != 1)
                {
                    transaccio.Rollback();
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public bool DeleteTrayecto(int id)
        {
            var connexio = _context.Database.GetDbConnection();

            if (connexio.State == System.Data.ConnectionState.Closed)
            {
                connexio.Open();
            }

            var transaccio = _context.Database.CurrentTransaction;
            if (transaccio == null) throw new Exception("NO TRANSACTION");

            using (var consulta = connexio.CreateCommand())
            {
                consulta.CommandText = @"delete from trajecte where id = :ID";

                consulta.Transaction = transaccio.GetDbTransaction();

                Utils.CrearParametre(consulta, id, "ID", System.Data.DbType.Int32);

                int numeroFilesAfectades = consulta.ExecuteNonQuery();

                if (numeroFilesAfectades != 1)
                {
                    transaccio.Rollback();
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
    }
}
