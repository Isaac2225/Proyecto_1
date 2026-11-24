
using System.Collections.ObjectModel;
using Modelo;

namespace IDAOCiudad
{
    public interface IDAOCiutat
    {
        Ciudad GetCiudad(int id);

        int GetNumeroCiutats(string id_filtre, string nom_filtre);

        ObservableCollection<Ciudad> GetCiudades(string oid_filtr, string nombre_filtro,
                                                int offset = 0, int rows_per_page = -1);

        bool InsertCiudad(Ciudad c);

        bool UpdateCiudad(Ciudad c);

        bool DeleteCiudad(int id);
    }

}
