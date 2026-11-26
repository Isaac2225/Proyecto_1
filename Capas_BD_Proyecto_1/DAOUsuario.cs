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

        public ObservableCollection<Usuario> GetUsuarios(
    string email = null,
    string pwd = null,
    bool? bloqueado = null,
    int offset = 0,
    int rows_per_page = -1)
        {
            ObservableCollection<Usuario> resultat = new ObservableCollection<Usuario>();

            var connexio = _context.Database.GetDbConnection();
            if (connexio.State != System.Data.ConnectionState.Open) connexio.Open();

            using (var consulta = connexio.CreateCommand())
            {
                if (consulta is OracleCommand oc) oc.BindByName = true;

                var sql = new StringBuilder(@"select ID, EMAIL, NOM, PASSWORD, TIPUS, DATA_REGISTRE
                                        from usuari
                                       where 1 = 1");

                if (!string.IsNullOrWhiteSpace(email))
                {
                    sql.Append(" and EMAIL = :EMAIL_TEXT");
                    Utils.CrearParametre(consulta, email, "EMAIL_TEXT", System.Data.DbType.String);
                }

                if (!string.IsNullOrWhiteSpace(pwd))
                {
                    sql.Append(" and PASSWORD = :PWD_TEXT");
                    Utils.CrearParametre(consulta, pwd, "PWD_TEXT", System.Data.DbType.String);
                }

                if (bloqueado.HasValue)
                {
                    // Cambia VALIDAT por BLOQ si esa es la columna que quieres filtrar en Oracle
                    sql.Append(" and VALIDAT = :VALIDAT_VAL");
                    Utils.CrearParametre(consulta, bloqueado.Value ? 1 : 0, "VALIDAT_VAL", System.Data.DbType.Int32);
                }

                sql.Append(" order by ID");

                if (rows_per_page > 0)
                {
                    sql.Append($" offset {offset} rows fetch next {rows_per_page} rows only");
                }

                consulta.CommandText = sql.ToString();
                consulta.Transaction = _context.Database.CurrentTransaction?.GetDbTransaction();

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
            if (connexio.State != System.Data.ConnectionState.Open)
            {
                connexio.Open();
            }

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

        public bool UpdateUsuario(
    // Filtros para el WHERE (pasa al menos uno)
    int? id = null,
    string email = null,
    string nom = null,
    char? tipo = null,
    bool? bloqueado = null,
    // Valores nuevos para el SET (pasa los que quieras modificar)
    string nuevoEmail = null,
    string nuevoNom = null,
    string nuevaPwd = null,
    char? nuevoTipo = null,
    DateTime? nuevaFecha = null,
    bool? nuevoBloq = null)
        {
            var connexio = _context.Database.GetDbConnection();
            if (connexio.State != System.Data.ConnectionState.Open) connexio.Open();

            var transaccio = _context.Database.CurrentTransaction;
            if (transaccio == null) throw new Exception("NO TRANSACTION");

            using (var consulta = connexio.CreateCommand())
            {
                if (consulta is OracleCommand oc) oc.BindByName = true;

                var setClauses = new List<string>();
                var whereClauses = new List<string>();

                // --- CAMPOS A ACTUALIZAR (SET) ---
                if (!string.IsNullOrWhiteSpace(nuevoEmail))
                {
                    setClauses.Add("EMAIL = :NEW_EMAIL");
                    Utils.CrearParametre(consulta, nuevoEmail, "NEW_EMAIL", System.Data.DbType.String);
                }
                if (!string.IsNullOrWhiteSpace(nuevoNom))
                {
                    setClauses.Add("NOM = :NEW_NOM");
                    Utils.CrearParametre(consulta, nuevoNom, "NEW_NOM", System.Data.DbType.String);
                }
                if (!string.IsNullOrWhiteSpace(nuevaPwd))
                {
                    setClauses.Add("PASSWORD = :NEW_PWD");
                    Utils.CrearParametre(consulta, nuevaPwd, "NEW_PWD", System.Data.DbType.String);
                }
                if (nuevoTipo.HasValue)
                {
                    setClauses.Add("TIPUS = :NEW_TIPUS");
                    Utils.CrearParametre(consulta, nuevoTipo.Value.ToString(), "NEW_TIPUS", System.Data.DbType.String);
                }
                if (nuevaFecha.HasValue)
                {
                    setClauses.Add("DATA_REGISTRE = :NEW_FECHA");
                    Utils.CrearParametre(consulta, nuevaFecha.Value, "NEW_FECHA", System.Data.DbType.Date);
                }
                if (nuevoBloq.HasValue)
                {
                    setClauses.Add("BLOQ = :NEW_BLOQ");
                    Utils.CrearParametre(consulta, nuevoBloq.Value ? 1 : 0, "NEW_BLOQ", System.Data.DbType.Int32);
                }

                // --- FILTROS (WHERE) ---
                if (id.HasValue)
                {
                    whereClauses.Add("ID = :WHERE_ID");
                    Utils.CrearParametre(consulta, id.Value, "WHERE_ID", System.Data.DbType.Int32);
                }
                if (!string.IsNullOrWhiteSpace(email))
                {
                    whereClauses.Add("EMAIL = :WHERE_EMAIL");
                    Utils.CrearParametre(consulta, email, "WHERE_EMAIL", System.Data.DbType.String);
                }
                if (!string.IsNullOrWhiteSpace(nom))
                {
                    whereClauses.Add("NOM = :WHERE_NOM");
                    Utils.CrearParametre(consulta, nom, "WHERE_NOM", System.Data.DbType.String);
                }
                if (tipo.HasValue)
                {
                    whereClauses.Add("TIPUS = :WHERE_TIPUS");
                    Utils.CrearParametre(consulta, tipo.Value.ToString(), "WHERE_TIPUS", System.Data.DbType.String);
                }
                if (bloqueado.HasValue)
                {
                    whereClauses.Add("BLOQ = :WHERE_BLOQ");
                    Utils.CrearParametre(consulta, bloqueado.Value ? 1 : 0, "WHERE_BLOQ", System.Data.DbType.Int32);
                }

                // Seguridad: evitar UPDATE sin campos o sin WHERE (que machacaría toda la tabla)
                if (setClauses.Count == 0 || whereClauses.Count == 0)
                {
                    return false;
                }

                consulta.CommandText = $"UPDATE USUARI SET {string.Join(", ", setClauses)} WHERE {string.Join(" AND ", whereClauses)}";
                consulta.Transaction = transaccio.GetDbTransaction();

                if (consulta.ExecuteNonQuery() < 1)
                {
                    transaccio.Rollback();
                    return false;
                }

                return true;
            }
        }
    }
}