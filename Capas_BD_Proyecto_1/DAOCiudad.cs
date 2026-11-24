
using DAO;
using IDAOCiudad;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Modelo;
using Oracle.ManagedDataAccess.Client;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Capas_BD_Proyecto_1
{
    public class DAOCiudad : IDAOCiutat

    {

        private readonly MyDBContext _context;


        public DAOCiudad(MyDBContext c)
        {
            _context = c;
        }

        public Ciudad GetCiudad(int id)
        {
            ObservableCollection<Ciudad> ciudades = GetCiudades(id.ToString(), "");

            if(ciudades.Count == 0)
            {
                return null;
            } else
            {
                return ciudades[0];
            }
        }

        public ObservableCollection<Ciudad> GetCiudades(string oid_filtr, string nombre_filtro, int offset = 0, int rows_per_page = -1)
        {
            ObservableCollection<Ciudad> resultat = new ObservableCollection<Ciudad>();

            var connexio = _context.Database.GetDbConnection();
            // Obrir la connexió a la BD
            if (connexio.State == System.Data.ConnectionState.Closed)
            {
                connexio.Open();
            }

            // Crear una consulta SQL
            using (var consulta = connexio.CreateCommand())
            {
                // Oracle enlaza por posición por defecto; con esto enlaza por nombre
                // (necesario porque :ID_TEXT aparece dos veces)
                if (consulta is Oracle.ManagedDataAccess.Client.OracleCommand oc)
                {
                    oc.BindByName = true;
                }

                // query SQL
                consulta.CommandText = @"select id, nom
                                   from ciutat
                                  where (:ID_TEXT = -1 or id = :ID_TEXT)
                                    and upper(nom) like upper(:NOM_TEXT)
                                  order by id";

                if (rows_per_page > 0)
                {
                    consulta.CommandText += $" offset {offset} rows fetch next {rows_per_page} rows only";
                }

                consulta.Transaction = _context.Database.CurrentTransaction?.GetDbTransaction();

                int id_filtre_int = -1;
                if (!Int32.TryParse(oid_filtr, out id_filtre_int))
                {
                    id_filtre_int = -1;
                }

                Utils.CrearParametre(consulta, id_filtre_int, "ID_TEXT", System.Data.DbType.Int32);
                Utils.CrearParametre(consulta, "%" + nombre_filtro + "%", "NOM_TEXT", System.Data.DbType.String);

                using (var reader = consulta.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = Convert.ToInt32(reader["id"]);
                        string nom = reader.GetString(reader.GetOrdinal("nom"));

                        resultat.Add(new Ciudad(id, nom));
                    }
                }
            }

            return resultat;
        }

        public int GetNumeroCiutats(string id_filtre, string nom_filtre)
        {
            var connexio = _context.Database.GetDbConnection();
            // Obrir la connexió a la BD
            if (connexio.State == System.Data.ConnectionState.Closed)
            {
                connexio.Open();
            }

            using (var consulta = connexio.CreateCommand())
            {
                // Enlazar parámetros por nombre (:ID_TEXT aparece dos veces)
                if (consulta is OracleCommand oc)
                {
                    oc.BindByName = true;
                }

                consulta.CommandText = @"select count(1) from ciutat
                                  where (:ID_TEXT = -1 or id = :ID_TEXT)
                                    and upper(nom) like upper(:NOM_TEXT)";

                consulta.Transaction = _context.Database.CurrentTransaction?.GetDbTransaction(); // IMPORTANT !!!

                int id_filtre_int = -1;
                if (!Int32.TryParse(id_filtre, out id_filtre_int))
                {
                    id_filtre_int = -1;
                }

                Utils.CrearParametre(consulta, id_filtre_int, "ID_TEXT", System.Data.DbType.Int32);
                Utils.CrearParametre(consulta, "%" + nom_filtre + "%", "NOM_TEXT", System.Data.DbType.String);

                // En Oracle count() devuelve decimal, por eso Convert y no un cast directo
                return Convert.ToInt32(consulta.ExecuteScalar());
            }
        }

        public bool InsertCiudad(Ciudad c)
        {
            var connexio = _context.Database.GetDbConnection();

            // Obrir la connexió a la BD (només si fa falta)
            if (connexio.State == System.Data.ConnectionState.Closed)
            {
                connexio.Open();
            }

            // assumirem que la transacció s'inicia pel context del UnitOfWork
            var transaccio = _context.Database.CurrentTransaction;
            if (transaccio == null) throw new Exception("NO TRANSACTION");

            using (var consulta = connexio.CreateCommand())
            {
                if (consulta is OracleCommand oc)
                {
                    oc.BindByName = true;
                }

                // "returning ... into" sustituye a last_insert_id() de MySQL
                consulta.CommandText = @"insert into ciutat (nom)
                                 values (:NOM)
                                 returning id into :NEW_ID";

                consulta.Transaction = transaccio.GetDbTransaction(); // IMPORTANT !!!

                Utils.CrearParametre(consulta, c.Nombre, "NOM", System.Data.DbType.String);

                // Parámetro de salida con el id generado
                var pId = consulta.CreateParameter();
                pId.ParameterName = "NEW_ID";
                pId.DbType = System.Data.DbType.Int32;
                pId.Direction = System.Data.ParameterDirection.Output;
                consulta.Parameters.Add(pId);

                int numeroFilesAfectades = consulta.ExecuteNonQuery();

                if (numeroFilesAfectades != 1)
                {
                    transaccio.Rollback(); // Torna enrera !!!
                    return false;
                }
                else
                {
                    c.Id = Convert.ToInt32(pId.Value); // desem l'ID a la ciutat
                    return true;
                }
            }
        }

        public bool UpdateCiudad(Ciudad c)
        {
            var connexio = _context.Database.GetDbConnection();

            // Obrir la connexió a la BD
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

                consulta.CommandText = @"update ciutat
                                    set nom = :NOM
                                  where id = :ID";

                consulta.Transaction = transaccio.GetDbTransaction(); // IMPORTANT !!!

                Utils.CrearParametre(consulta, c.Nombre, "NOM", System.Data.DbType.String);
                Utils.CrearParametre(consulta, c.Id, "ID", System.Data.DbType.Int32);

                int numeroFilesAfectades = consulta.ExecuteNonQuery();

                if (numeroFilesAfectades != 1)
                {
                    transaccio.Rollback(); // Torna enrera !!!
                    return false;
                }
                else
                {
                    // El commit es fa a fora (UnitOfWork)
                    return true;
                }
            }
        }

        public bool DeleteCiudad(int id)
        {
            var connexio = _context.Database.GetDbConnection();

            // Obrir la connexió a la BD
            if (connexio.State == System.Data.ConnectionState.Closed)
            {
                connexio.Open();
            }

            var transaccio = _context.Database.CurrentTransaction;
            if (transaccio == null) throw new Exception("NO TRANSACTION");

            using (var consulta = connexio.CreateCommand())
            {
                consulta.CommandText = @"delete from ciutat where id = :ID";

                consulta.Transaction = transaccio.GetDbTransaction(); // IMPORTANT !!!

                Utils.CrearParametre(consulta, id, "ID", System.Data.DbType.Int32);

                int numeroFilesAfectades = consulta.ExecuteNonQuery();

                if (numeroFilesAfectades != 1)
                {
                    transaccio.Rollback(); // Torna enrera !!!
                    return false;
                }
                else
                {
                    // El commit es fa a fora (UnitOfWork)
                    return true;
                }
            }
        }
    }

}
