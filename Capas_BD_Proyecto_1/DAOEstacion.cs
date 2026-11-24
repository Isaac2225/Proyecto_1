using IDAO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Modelo;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace DAO
{
    public class DAOEstacion : IDAOEstacion
    {
        private readonly MyDBContext _context;

        public DAOEstacion(MyDBContext c)
        {
            _context = c;
        }

        public Estacion GetEstacion(string nombre)
        {
            ObservableCollection<Estacion> estaciones = GetEstaciones(nombre);
            if (estaciones.Count == 0) return null;
            return estaciones[0];
        }

        public ObservableCollection<Estacion> GetEstaciones(string nombre_filtro)
        {
            ObservableCollection<Estacion> resultat = new ObservableCollection<Estacion>();

            var connexio = _context.Database.GetDbConnection();
            if (connexio.State == System.Data.ConnectionState.Closed) connexio.Open();

            using (var consulta = connexio.CreateCommand())
            {
                if (consulta is OracleCommand oc) oc.BindByName = true;

                consulta.CommandText = @"select id_es, nom_es, latitud, longitud, id_ciutat
                                   from estacio
                                   where upper(nom_es) like upper(:NOM_TEXT)
                                  order by id_es";

               

                consulta.Transaction = _context.Database.CurrentTransaction?.GetDbTransaction();

                int id_filtre_int = -1;
                

                Utils.CrearParametre(consulta, id_filtre_int, "ID_TEXT", System.Data.DbType.Int32);
                Utils.CrearParametre(consulta, "%" + nombre_filtro + "%", "NOM_TEXT", System.Data.DbType.String);

                using (var reader = consulta.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = Convert.ToInt32(reader["id_es"]);
                        string nom = reader.GetString(reader.GetOrdinal("nom_es"));
                        double latitud = Convert.ToDouble(reader["latitud"]);
                        double longitud = Convert.ToDouble(reader["longitud"]);
                        int ciutat_id = Convert.ToInt32(reader["id_ciutat"]);

                        Ciudad ciudad = new Ciudad(ciutat_id);

                        resultat.Add(new Estacion(id, nom, latitud, longitud, ciudad));
                    }
                }
            }

            return resultat;
        }

        public int GetNumeroEstaciones(string id_filtro, string nombre_filtro)
        {
            var connexio = _context.Database.GetDbConnection();
            if (connexio.State == System.Data.ConnectionState.Closed) connexio.Open();

            using (var consulta = connexio.CreateCommand())
            {
                if (consulta is OracleCommand oc) oc.BindByName = true;

                consulta.CommandText = @"select count(1) from estacio
                                  where 
                                              
                                    upper(nom_es) like upper(:NOM_TEXT)";

                consulta.Transaction = _context.Database.CurrentTransaction?.GetDbTransaction();

                

                Utils.CrearParametre(consulta, "%" + nombre_filtro + "%", "NOM_TEXT", System.Data.DbType.String);

                return Convert.ToInt32(consulta.ExecuteScalar());
            }
        }

        public bool InsertEstacion(Estacion e)
        {
            var connexio = _context.Database.GetDbConnection();
            if (connexio.State == System.Data.ConnectionState.Closed) connexio.Open();

            var transaccio = _context.Database.CurrentTransaction;
            if (transaccio == null) throw new Exception("NO TRANSACTION");

            using (var consulta = connexio.CreateCommand())
            {
                if (consulta is OracleCommand oc) oc.BindByName = true;

                consulta.CommandText = @"insert into estacio (nom_es, latitud, longitud, id_ciutat)
                                 values (:NOM_ES, :LATITUD, :LONGITUD, :ID_CIUTAT)
                                 returning id_es into :NEW_ID";

                consulta.Transaction = transaccio.GetDbTransaction();

                Utils.CrearParametre(consulta, e.Nombre, "NOM_ES", System.Data.DbType.String);
                Utils.CrearParametre(consulta, e.Latitud, "LATITUD", System.Data.DbType.Double);
                Utils.CrearParametre(consulta, e.Lonitud, "LONGITUD", System.Data.DbType.Double);
                Utils.CrearParametre(consulta, e.Ciudad_id?.Id, "ID_CIUTAT", System.Data.DbType.Int32);

                var pId = consulta.CreateParameter();
                pId.ParameterName = "NEW_ID";
                pId.DbType = System.Data.DbType.Int32;
                pId.Direction = System.Data.ParameterDirection.Output;
                consulta.Parameters.Add(pId);

                if (consulta.ExecuteNonQuery() != 1)
                {
                    transaccio.Rollback();
                    return false;
                }

                e.Id = Convert.ToInt32(pId.Value);
                return true;
            }
        }

        public bool UpdateEstacion(Estacion e)
        {
            var connexio = _context.Database.GetDbConnection();
            if (connexio.State == System.Data.ConnectionState.Closed) connexio.Open();

            var transaccio = _context.Database.CurrentTransaction;
            if (transaccio == null) throw new Exception("NO TRANSACTION");

            using (var consulta = connexio.CreateCommand())
            {
                if (consulta is OracleCommand oc) oc.BindByName = true;

                consulta.CommandText = @"update estacio
                                    set nom_es = :NOM_ES,
                                        latitud = :LATITUD,
                                        longitud = :LONGITUD,
                                        id_ciutat = :ID_CIUTAT
                                  where id_es = :ID_ES";

                consulta.Transaction = transaccio.GetDbTransaction();

                Utils.CrearParametre(consulta, e.Nombre, "NOM_ES", System.Data.DbType.String);
                Utils.CrearParametre(consulta, e.Latitud, "LATITUD", System.Data.DbType.Double);
                Utils.CrearParametre(consulta, e.Lonitud, "LONGITUD", System.Data.DbType.Double);
                Utils.CrearParametre(consulta, e.Ciudad_id?.Id, "ID_CIUTAT", System.Data.DbType.Int32);
                Utils.CrearParametre(consulta, e.Id, "ID_ES", System.Data.DbType.Int32);

                if (consulta.ExecuteNonQuery() != 1)
                {
                    transaccio.Rollback();
                    return false;
                }
                return true;
            }
        }

        public bool DeleteEstacion(int id)
        {
            var connexio = _context.Database.GetDbConnection();
            if (connexio.State == System.Data.ConnectionState.Closed) connexio.Open();

            var transaccio = _context.Database.CurrentTransaction;
            if (transaccio == null) throw new Exception("NO TRANSACTION");

            using (var consulta = connexio.CreateCommand())
            {
                consulta.CommandText = @"delete from estacio where id_es = :ID_ES";
                consulta.Transaction = transaccio.GetDbTransaction();
                Utils.CrearParametre(consulta, id, "ID_ES", System.Data.DbType.Int32);

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
