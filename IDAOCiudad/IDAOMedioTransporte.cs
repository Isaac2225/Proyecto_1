using Modelo;
using System.Collections.ObjectModel;

namespace IDAO
{
    public interface IDAOMedioTransporte
    {
        MedioTransporte GetMedioTransporte(int id);
        ObservableCollection<MedioTransporte> GetMediosTransporte(string idFiltro, string nombreFiltro, int offset = 0, int rows_per_page = -1);
        int GetNumeroMediosTransporte(string idFiltro, string nombreFiltro);
    }
}