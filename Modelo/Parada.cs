using System;

namespace Modelo
{
    public class Parada
    {
        private Trayecto id_trayecto;
        private int idEstacion;
        private int orden;
        private string nombre;
        private DateTime? horaLlegada;
        private DateTime? horaSalida;

        public Parada(Trayecto id_trayecto, int idEstacion, int orden, string nombre,
                      DateTime? horaLlegada, DateTime? horaSalida)
        {
            Id_trayecto = id_trayecto;
            IdEstacion = idEstacion;
            Orden = orden;
            Nombre = nombre;
            HoraLlegada = horaLlegada;
            HoraSalida = horaSalida;
        }

        public Trayecto Id_trayecto { get => id_trayecto; set => id_trayecto = value; }
        public int IdEstacion { get => idEstacion; set => idEstacion = value; }
        public int Orden { get => orden; set => orden = value; }
        public string Nombre { get => nombre; set => nombre = value; }
        public DateTime? HoraLlegada { get => horaLlegada; set => horaLlegada = value; }
        public DateTime? HoraSalida { get => horaSalida; set => horaSalida = value; }
        public DateTime? HoraPaso => HoraSalida ?? HoraLlegada;
    }
}