using IDAO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Modelo;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.ObjectModel;

namespace DAO
{
    public class DAOMedioTransporte : IDAOMedioTransporte
    {
        private readonly MyDBContext _context;

        public DAOMedioTransporte(MyDBContext c)
        {
            _context = c;
        }

        public MedioTransporte GetMedioTransporte(int id)
        {
            ObservableCollection<MedioTransporte> medios = GetMediosTransporte(id.ToString(), "");
            if (medios.Count == 0) return null;
            return medios[0];
        }

        public ObservableCollection<MedioTransporte> GetMediosTransporte(string idFiltro, string nombreFiltro, int offset = 0, int rows_per_page = -1)
        {
            ObservableCollection<MedioTransporte> resultat = new ObservableCollection<MedioTransporte>();

            var connexio = _context.Database.GetDbConnection();
            if (connexio.State == System.Data.ConnectionState.Closed) connexio.Open();

            using (var consulta = connexio.CreateCommand())
            {
                if (consulta is OracleCommand oc) oc.BindByName = true;

                consulta.CommandText = @"select id_medi, nom_medi
                                   from medi_transport
                                  where (:ID_TEXT = -1 or id_medi = :ID_TEXT)
                                    and upper(nom_medi) like upper(:NOM_TEXT)
                                  order by id_medi";

                if (rows_per_page > 0)
                {
                    consulta.CommandText += $" offset {offset} rows fetch next {rows_per_page} rows only";
                }

                consulta.Transaction = _context.Database.CurrentTransaction?.GetDbTransaction();

                int id_filtre_int = -1;
                if (!Int32.TryParse(idFiltro, out id_filtre_int)) id_filtre_int = -1;

                Utils.CrearParametre(consulta, id_filtre_int, "ID_TEXT", System.Data.DbType.Int32);
                Utils.CrearParametre(consulta, "%" + nombreFiltro + "%", "NOM_TEXT", System.Data.DbType.String);

                using (var reader = consulta.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = Convert.ToInt32(reader["id_medi"]);
                        string nom = reader.GetString(reader.GetOrdinal("nom_medi"));

                        MedioTransporte medio = new MedioTransporte(id);
                        medio.Nombre = nom;

                        resultat.Add(medio);
                    }
                }
            }

            return resultat;
        }

        public int GetNumeroMediosTransporte(string idFiltro, string nombreFiltro)
        {
            var connexio = _context.Database.GetDbConnection();
            if (connexio.State == System.Data.ConnectionState.Closed) connexio.Open();

            using (var consulta = connexio.CreateCommand())
            {
                if (consulta is OracleCommand oc) oc.BindByName = true;

                consulta.CommandText = @"select count(1) from medi_transport
                                  where (:ID_TEXT = -1 or id_medi = :ID_TEXT)
                                    and upper(nom_medi) like upper(:NOM_TEXT)";

                consulta.Transaction = _context.Database.CurrentTransaction?.GetDbTransaction();

                int id_filtre_int = -1;
                if (!Int32.TryParse(idFiltro, out id_filtre_int)) id_filtre_int = -1;

                Utils.CrearParametre(consulta, id_filtre_int, "ID_TEXT", System.Data.DbType.Int32);
                Utils.CrearParametre(consulta, "%" + nombreFiltro + "%", "NOM_TEXT", System.Data.DbType.String);

                return Convert.ToInt32(consulta.ExecuteScalar());
            }
        }
    }
}