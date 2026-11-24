using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Modelo;

namespace IDAO
{
    public interface IDAOEstacion
    {
        Estacion GetEstacion(string nombre);
        ObservableCollection<Estacion> GetEstaciones(string nombre_filtro);
        int GetNumeroEstaciones(string id_filtro, string nom_filtre);
        bool InsertEstacion(Estacion e);
        bool UpdateEstacion(Estacion e);
        bool DeleteEstacion(int id);
    }
}
