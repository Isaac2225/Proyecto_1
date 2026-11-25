using IDAO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Modelo;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.ObjectModel;
using System.Text;

namespace DAO
{
    public class DAOParada : IDAOParada
    {
        private readonly MyDBContext _context;

        public DAOParada(MyDBContext c)
        {
            _context = c;
        }

        public Parada GetParada(int? idTrayecto = null, int? orden = null)
        {
            ObservableCollection<Parada> paradas = GetParadas(idTrayecto?.ToString());

            return paradas.FirstOrDefault(p => orden == null || p.Orden == orden);
        }

        public ObservableCollection<Parada> GetParadas(
    string idTrayectoFiltro = null,
    string nombreFiltro = null,
    int offset = 0,
    int rows_per_page = -1)
        {
            ObservableCollection<Parada> resultat = new ObservableCollection<Parada>();

            var connexio = _context.Database.GetDbConnection();
            if (connexio.State == System.Data.ConnectionState.Closed) connexio.Open();

            using (var consulta = connexio.CreateCommand())
            {
                if (consulta is OracleCommand oc) oc.BindByName = true;

                var sql = new StringBuilder(@"select p.trajecte_id,
                                             p.estacio_id,
                                             p.ordre,
                                             e.nom_es,
                                             p.hora_arribada,
                                             p.hora_sortida
                                      from parada p
                                      join estacio e on e.id_es = p.estacio_id
                                      where 1 = 1");

                // Filtro por trayecto: solo si se ha pasado un número válido
                if (!string.IsNullOrWhiteSpace(idTrayectoFiltro) &&
                    Int32.TryParse(idTrayectoFiltro, out int idFiltreInt))
                {
                    sql.Append(" and p.trajecte_id = :ID_TEXT");
                    Utils.CrearParametre(consulta, idFiltreInt, "ID_TEXT", System.Data.DbType.Int32);
                }

                // Filtro por nombre: solo si se ha pasado texto
                if (!string.IsNullOrWhiteSpace(nombreFiltro))
                {
                    sql.Append(" and upper(e.nom_es) like upper(:NOM_TEXT)");
                    Utils.CrearParametre(consulta, "%" + nombreFiltro + "%", "NOM_TEXT", System.Data.DbType.String);
                }

                sql.Append(" order by p.trajecte_id, p.ordre");

                if (rows_per_page > 0)
                {
                    sql.Append($" offset {offset} rows fetch next {rows_per_page} rows only");
                }

                consulta.CommandText = sql.ToString();
                consulta.Transaction = _context.Database.CurrentTransaction?.GetDbTransaction();

                using (var reader = consulta.ExecuteReader())
                {
                    int ordTrayecto = reader.GetOrdinal("trajecte_id");
                    int ordEstacio = reader.GetOrdinal("estacio_id");
                    int ordOrdre = reader.GetOrdinal("ordre");
                    int ordNom = reader.GetOrdinal("nom_es");
                    int ordArribada = reader.GetOrdinal("hora_arribada");
                    int ordSortida = reader.GetOrdinal("hora_sortida");

                    while (reader.Read())
                    {
                        int trayecto_id = Convert.ToInt32(reader[ordTrayecto]);
                        int idEstacion = Convert.ToInt32(reader[ordEstacio]);
                        int orden = Convert.ToInt32(reader[ordOrdre]);
                        string nom = reader.GetString(ordNom);

                        DateTime? llegada = reader.IsDBNull(ordArribada) ? (DateTime?)null : reader.GetDateTime(ordArribada);
                        DateTime? salida = reader.IsDBNull(ordSortida) ? (DateTime?)null : reader.GetDateTime(ordSortida);

                        Trayecto trayecto = new Trayecto(null, default(DateTime), null);
                        trayecto.Id = trayecto_id;

                        resultat.Add(new Parada(trayecto, idEstacion, orden, nom, llegada, salida));
                    }
                }
            }

            return resultat;
        }

        public int GetNumeroParadas(string idTrayectoFiltro, string nombreFiltro)
        {
            var connexio = _context.Database.GetDbConnection();
            if (connexio.State == System.Data.ConnectionState.Closed) connexio.Open();

            using (var consulta = connexio.CreateCommand())
            {
                if (consulta is OracleCommand oc) oc.BindByName = true;

                consulta.CommandText = @"select count(1) from parada
                                  where (:ID_TEXT = -1 or trajecte_id = :ID_TEXT)
                                    and upper(nom) like upper(:NOM_TEXT)";

                consulta.Transaction = _context.Database.CurrentTransaction?.GetDbTransaction();

                int id_filtre_int = -1;
                if (!Int32.TryParse(idTrayectoFiltro, out id_filtre_int)) id_filtre_int = -1;

                Utils.CrearParametre(consulta, id_filtre_int, "ID_TEXT", System.Data.DbType.Int32);
                Utils.CrearParametre(consulta, "%" + nombreFiltro + "%", "NOM_TEXT", System.Data.DbType.String);

                return Convert.ToInt32(consulta.ExecuteScalar());
            }
        }

        public bool InsertParada(Parada p)
        {
            var connexio = _context.Database.GetDbConnection();
            if (connexio.State == System.Data.ConnectionState.Closed) connexio.Open();

            var transaccio = _context.Database.CurrentTransaction;
            if (transaccio == null) throw new Exception("NO TRANSACTION");

            using (var consulta = connexio.CreateCommand())
            {
                if (consulta is OracleCommand oc) oc.BindByName = true;

                consulta.CommandText = @"insert into parada (
                            TRAJECTE_ID,
                            ESTACIO_ID,
                            ORDRE,
                            HORA_ARRIBADA,
                            HORA_SORTIDA
                        ) values (
                            :TRAJECTE_ID,
                            :ESTACIO_ID,
                            (select nvl(max(ordre), 0) + 1 from parada where trajecte_id = :TRAJECTE_ID),
                            :HORA_ARRIBADA,
                            :HORA_SORTIDA
                        )";

                consulta.Transaction = transaccio.GetDbTransaction();

                Utils.CrearParametre(consulta, p.Id_trayecto.Id, "TRAJECTE_ID", System.Data.DbType.Int32);
                Utils.CrearParametre(consulta, p.IdEstacion, "ESTACIO_ID", System.Data.DbType.Int32);
                Utils.CrearParametre(consulta, (object)p.HoraLlegada ?? DBNull.Value, "HORA_ARRIBADA", System.Data.DbType.DateTime);
                Utils.CrearParametre(consulta, (object)p.HoraSalida ?? DBNull.Value, "HORA_SORTIDA", System.Data.DbType.DateTime);

                consulta.Transaction = transaccio.GetDbTransaction();

                Utils.CrearParametre(consulta, p.Id_trayecto?.Id, "TRAJECTE_ID", System.Data.DbType.Int32);
                Utils.CrearParametre(consulta, p.Orden, "ORDRE", System.Data.DbType.Int32);
                Utils.CrearParametre(consulta, p.Nombre, "NOM", System.Data.DbType.String);
                Utils.CrearParametre(consulta, 1, "ESTACIO_NUMERO", System.Data.DbType.Int32);
                Utils.CrearParametre(consulta, p.HoraPaso, "HORAPAS", System.Data.DbType.DateTime);

                if (consulta.ExecuteNonQuery() != 1)
                {
                    transaccio.Rollback();
                    return false;
                }
                return true;
            }
        }

        public bool UpdateParada(Parada p)
        {
            var connexio = _context.Database.GetDbConnection();
            if (connexio.State == System.Data.ConnectionState.Closed) connexio.Open();

            var transaccio = _context.Database.CurrentTransaction;
            if (transaccio == null) throw new Exception("NO TRANSACTION");

            using (var consulta = connexio.CreateCommand())
            {
                if (consulta is OracleCommand oc) oc.BindByName = true;

                consulta.CommandText = @"update parada
                                    set nom = :NOM,
                                        horapas = :HORAPAS
                                  where trajecte_id = :TRAJECTE_ID
                                    and ordre = :ORDRE";

                consulta.Transaction = transaccio.GetDbTransaction();

                Utils.CrearParametre(consulta, p.Nombre, "NOM", System.Data.DbType.String);
                Utils.CrearParametre(consulta, p.HoraPaso, "HORAPAS", System.Data.DbType.DateTime);
                Utils.CrearParametre(consulta, p.Id_trayecto?.Id, "TRAJECTE_ID", System.Data.DbType.Int32);
                Utils.CrearParametre(consulta, p.Orden, "ORDRE", System.Data.DbType.Int32);

                if (consulta.ExecuteNonQuery() != 1)
                {
                    transaccio.Rollback();
                    return false;
                }
                return true;
            }
        }

        public bool DeleteParada(int idTrayecto, int orden)
        {
            var connexio = _context.Database.GetDbConnection();
            if (connexio.State == System.Data.ConnectionState.Closed) connexio.Open();

            var transaccio = _context.Database.CurrentTransaction;
            if (transaccio == null) throw new Exception("NO TRANSACTION");

            using (var consulta = connexio.CreateCommand())
            {
                if (consulta is OracleCommand oc) oc.BindByName = true;

                consulta.CommandText = @"delete from parada
                                  where trajecte_id = :TRAJECTE_ID
                                    and ordre = :ORDRE";
                consulta.Transaction = transaccio.GetDbTransaction();

                Utils.CrearParametre(consulta, idTrayecto, "TRAJECTE_ID", System.Data.DbType.Int32);
                Utils.CrearParametre(consulta, orden, "ORDRE", System.Data.DbType.Int32);

                if (consulta.ExecuteNonQuery() != 1)
                {
                    transaccio.Rollback();
                    return false;
                }
                return true;
            }
        }
    }
}