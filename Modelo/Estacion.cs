using System;
using System.Collections.Generic;
using System.Text;

namespace Modelo
{
    public class Estacion
    {
        int id;
        private Ciudad ciudad_id;
        private string nombre;
        private double latitud;
        private double lonitud;

        public Estacion(int id, string nombre, double latitud, double lonitud, Ciudad ciudad_id)
        {
            Id = id;
            Nombre = nombre;
            Latitud = latitud;
            Lonitud = lonitud;
            Ciudad_id = ciudad_id;
        }

        public int Id { get => id; set => id = value; }
        public string Nombre { get => nombre; set => nombre = value; }
        public double Latitud { get => latitud; set => latitud = value; }
        public double Lonitud { get => lonitud; set => lonitud = value; }
        public Ciudad Ciudad_id { get => ciudad_id; set => ciudad_id = value; }
    }
}
