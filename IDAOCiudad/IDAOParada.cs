using Modelo;
using System.Collections.ObjectModel;

namespace IDAO
{
    public interface IDAOParada
    {
        Parada GetParada(string nommbre);

        ObservableCollection<Parada> GetParadas(
                    string idTrayectoFiltro,
                    string nombreFiltro,
                    int offset = 0,
                    int rows_per_page = -1);
        int GetNumeroParadas(string idTrayectoFiltro, string nombreFiltro);
        bool InsertParada(Parada p);
        bool UpdateParada(Parada p);
        bool DeleteParada(int idTrayecto, int orden);
    }
}