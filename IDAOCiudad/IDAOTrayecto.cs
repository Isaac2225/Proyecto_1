using System;
using System.Collections.ObjectModel;
using Modelo;

namespace IDAO
{
    public interface IDAOTrayecto
    {
        // Obtiene un único trayecto por su ID
        Trayecto GetTrayecto(int id);

        // Obtiene una lista filtrada y paginada de trayectos
        ObservableCollection<Trayecto> GetTrayectos(
            string origen = "",
            string destino = "",
            int offset = 0,
            int rows_per_page = -1,
            TimeSpan? horaSalida = null,
            TimeSpan? horaLlegada = null);

        // Obtiene el número total de trayectos según los filtros (útil para la paginación)
        int GetNumeroTrayectos(string id_filtre, string nom_filtre);

        // Inserta un nuevo trayecto en la base de datos
        bool InsertTrayecto(Trayecto t);

        // Actualiza un trayecto existente
        bool UpdateTrayecto(Trayecto t);

        // Elimina un trayecto por su ID
        bool DeleteTrayecto(int id);
    }
}