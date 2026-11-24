using System;
using System.Collections.Generic;
using System.Text;

namespace Modelo
{
    public class Ciudad
    {
        private int id;
        private string nombre;

        public Ciudad(int id)
        {
            Id = id;
        }

        //NO SE SI LO USARÉ. VAMOS VIENDO
        public Ciudad(int id, string nombre)
        {
            Id = id;
            Nombre = nombre;
        }

        public int Id { get => id; set => id = value; }
        public string Nombre { get => nombre; set => nombre = value; }

        //ToString TEMPORAL !!!!  BORRAR ANTES DE DISEÑAR LA INTERFAZ
        public override string ToString()
        {
            return Id.ToString();
        }
    }
}
