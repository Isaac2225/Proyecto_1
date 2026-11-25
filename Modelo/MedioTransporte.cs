using System;
using System.Collections.Generic;
using System.Text;

namespace Modelo
{
    public class MedioTransporte
    {
        private int id;
        private string nombre;

        public string Icono
        {
            get
            {
                return Id switch
                {
                    1 => "\uE806",
                    2 => "\uE7C0", 
                    3 => "\uE804",
                    4 => "\uE726", 
                    5 => "\uE709",
                    6 => "\uE7E3",
                    _ => "\uE946"  
                };
            }
        }

        public MedioTransporte(int id)
        {
            Id = id;
        }

        public MedioTransporte(string nombre)
        {
            Id = 0;
            Nombre = nombre;
        }

        public int Id { get => id; set => id = value; }
        public string Nombre { get => nombre; set => nombre = value; }
    }
}
