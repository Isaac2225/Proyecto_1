using System;
using System.Collections.ObjectModel;
using Modelo;

namespace IDAO
{
    public interface IDAOTrayecto
    {
        Trayecto GetTrayecto(string nombre);

        ObservableCollection<Trayecto> GetTrayectos(
            string origen = "",
            string destino = "",
            int offset = 0,
            int rows_per_page = -1,
            TimeSpan? horaSalida = null,
            TimeSpan? horaLlegada = null);

        int GetNumeroTrayectos(string id_filtre, string nom_filtre);

        bool InsertTrayecto(Trayecto t);

        bool UpdateTrayecto(Trayecto t);

        bool DeleteTrayecto(int id);
    }
}