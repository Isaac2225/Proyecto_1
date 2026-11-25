using IDAO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Modelo;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.ObjectModel;

namespace DAO
{
    public class DAOUsuario : IDAOUsuario
    {
        private readonly MyDBContext _context;

        public DAOUsuario(MyDBContext c)
        {
            _context = c;
        }

        public Usuario GetUsuario(string email, string pwd)
        {
            // Llamamos a GetUsuarios pasándole el email y la contraseña
            ObservableCollection<Usuario> usuarios = GetUsuarios(email, pwd);
            if (usuarios.Count == 0) return null;
            return usuarios[0];
        }

        public ObservableCollection<Usuario> GetUsuarios(string email, string pwd, int offset = 0, int rows_per_page = -1)
        {
            ObservableCollection<Usuario> resultat = new ObservableCollection<Usuario>();

            var connexio = _context.Database.GetDbConnection();
            if (connexio.State == System.Data.ConnectionState.Closed) connexio.Open();

            using (var consulta = connexio.CreateCommand())
            {
                if (consulta is OracleCommand oc) oc.BindByName = true;

                // Actualizamos la consulta para filtrar por EMAIL y PASSWORD
                consulta.CommandText = @"select ID, EMAIL, NOM, PASSWORD, TIPUS, DATA_REGISTRE 
                                   from usuari 
                                  where EMAIL = :EMAIL_TEXT 
                                    and PASSWORD = :PWD_TEXT 
                                  order by ID";

                if (rows_per_page > 0)
                {
                    consulta.CommandText += $" offset {offset} rows fetch next {rows_per_page} rows only";
                }

                consulta.Transaction = _context.Database.CurrentTransaction?.GetDbTransaction();

                // Creamos los parámetros para email y password (ambos como String)
                Utils.CrearParametre(consulta, email, "EMAIL_TEXT", System.Data.DbType.String);
                Utils.CrearParametre(consulta, pwd, "PWD_TEXT", System.Data.DbType.String);

                using (var reader = consulta.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = Convert.ToInt32(reader["ID"]);
                        string readerEmail = reader.GetString(reader.GetOrdinal("EMAIL"));
                        string nom = reader.GetString(reader.GetOrdinal("NOM"));
                        string readerPassword = reader.GetString(reader.GetOrdinal("PASSWORD"));
                        char tipo = reader.GetString(reader.GetOrdinal("TIPUS"))[0];
                        DateTime fecha = reader.GetDateTime(reader.GetOrdinal("DATA_REGISTRE"));

                        resultat.Add(new Usuario(id, readerEmail, nom, readerPassword, tipo, fecha));
                    }
                }
            }

            return resultat;
        }

        public int GetNumeroUsuarios(string idFiltro, string nombreFiltro)
        {
            var connexio = _context.Database.GetDbConnection();
            if (connexio.State == System.Data.ConnectionState.Closed) connexio.Open();

            using (var consulta = connexio.CreateCommand())
            {
                if (consulta is OracleCommand oc) oc.BindByName = true;

                consulta.CommandText = @"select count(1) from usuari
                                  where (:ID_TEXT = -1 or ID = :ID_TEXT)
                                    and upper(NOM) like upper(:NOM_TEXT)";

                consulta.Transaction = _context.Database.CurrentTransaction?.GetDbTransaction();

                int id_filtre_int = -1;
                if (!Int32.TryParse(idFiltro, out id_filtre_int)) id_filtre_int = -1;

                Utils.CrearParametre(consulta, id_filtre_int, "ID_TEXT", System.Data.DbType.Int32);
                Utils.CrearParametre(consulta, "%" + nombreFiltro + "%", "NOM_TEXT", System.Data.DbType.String);

                return Convert.ToInt32(consulta.ExecuteScalar());
            }
        }

        public int GetNumeroUsuariosByUsrPwd(string usr, string pwd)
        {
            var connexio = _context.Database.GetDbConnection();
            //if (connexio.State == System.Data.ConnectionState.Closed) connexio.Open();

            using (var consulta = connexio.CreateCommand())
            {
                if (consulta is OracleCommand oc) oc.BindByName = true;

                consulta.Transaction = _context.Database.CurrentTransaction?.GetDbTransaction();

                if (usr == null)
                {
                    consulta.CommandText = @"select count(*) 
                                       from usuari 
                                      where PASSWORD = :PWD_TEXT";

                    Utils.CrearParametre(consulta, pwd, "PWD_TEXT", System.Data.DbType.String);
                }
                else if (pwd == null)
                {
                    consulta.CommandText = @"select count(*) 
                                       from usuari 
                                      where EMAIL = :EMAIL_TEXT";

                    Utils.CrearParametre(consulta, usr, "EMAIL_TEXT", System.Data.DbType.String);
                }
                else
                {
                    consulta.CommandText = @"select count(*) 
                                       from usuari 
                                      where EMAIL = :EMAIL_TEXT 
                                        and PASSWORD = :PWD_TEXT";

                    Utils.CrearParametre(consulta, usr, "EMAIL_TEXT", System.Data.DbType.String);
                    Utils.CrearParametre(consulta, pwd, "PWD_TEXT", System.Data.DbType.String);
                }

                return Convert.ToInt32(consulta.ExecuteScalar());
            }
        }

        public bool insertUsuario(string nombre, string mail, string pwd, DateTime fecha)
        {
            var connexio = _context.Database.GetDbConnection();
            if (connexio.State == System.Data.ConnectionState.Closed) connexio.Open();

            var transaccio = _context.Database.CurrentTransaction;
            if (transaccio == null) throw new Exception("NO TRANSACTION");

            using (var consulta = connexio.CreateCommand())
            {
                if (consulta is OracleCommand oc) oc.BindByName = true;

                consulta.Transaction = _context.Database.CurrentTransaction?.GetDbTransaction();

                consulta.CommandText = @"INSERT INTO USUARI (id, email, nom, password, data_registre)
                         VALUES ((SELECT NVL(MAX(id), 0) + 1 FROM usuari),
                                 :EMAIL_TEXT,
                                 :NOMBRE_TEXT,
                                 :PASSWORD_TEXT,
                                 :FECHA_TEXT)";

                Utils.CrearParametre(consulta, nombre, "NOMBRE_TEXT", System.Data.DbType.String);
                Utils.CrearParametre(consulta, pwd, "PASSWORD_TEXT", System.Data.DbType.String);
                Utils.CrearParametre(consulta, mail, "EMAIL_TEXT", System.Data.DbType.String);
                Utils.CrearParametre(consulta, fecha, "FECHA_TEXT", System.Data.DbType.Date);

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