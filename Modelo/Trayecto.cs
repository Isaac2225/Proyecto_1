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
        private Usuario id_usuario;
        private MedioTransporte medio;
        private Estacion origen;
        private Estacion destino;

        public Trayecto(int id, string nombre, DateTime fechaCreacion, Usuario id_usuario, MedioTransporte medio)
        {
            Id = id;
            Nombre = nombre;
            FechaCreacion = fechaCreacion;
            Validado = false;
            Id_usuario = id_usuario;
            Medio = medio;
        }

        public int Id { get => id; set => id = value; }
        public string Nombre { get => nombre; set => nombre = value; }
        public DateTime FechaCreacion { get => fechaCreacion; set => fechaCreacion = value; }
        public bool Validado { get => validado; set => validado = value; }
        public Usuario Id_usuario { get => id_usuario; set => id_usuario = value; }
        public MedioTransporte Medio { get => medio; set => medio = value; }
        public Estacion Origen { get => origen; set => origen = value; }
        public Estacion Destino { get => destino; set => destino = value; }
    }
}
