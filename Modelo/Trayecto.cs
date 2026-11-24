using System;
using System.Collections.Generic;
using System.Text;

namespace Modelo
{
    public class Trayecto
    {
        private int id;
        private string nombre;
        private DateTime fechaCreacion;
        private bool validado;
        private MedioTransporte medio;
        private Estacion origen;
        private Estacion destino;

        public Trayecto(string nombre, DateTime fechaCreacion, MedioTransporte medio)
        {
            Id = 0;
            Nombre = nombre;
            FechaCreacion = fechaCreacion;
            Validado = false;
            Medio = medio;
        }

        public int Id { get => id; set => id = value; }
        public string Nombre { get => nombre; set => nombre = value; }
        public DateTime FechaCreacion { get => fechaCreacion; set => fechaCreacion = value; }
        public bool Validado { get => validado; set => validado = value; }
        public MedioTransporte Medio { get => medio; set => medio = value; }
        public Estacion Origen { get => origen; set => origen = value; }
        public Estacion Destino { get => destino; set => destino = value; }
    }
}
