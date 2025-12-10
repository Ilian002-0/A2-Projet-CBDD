using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2_Projet_CBDD
{
    internal class Salle
    {
        private int idSalle;
        private string nomSalle;
        private int capacite;
        public Salle(int idSalle, string nomSalle, int capacite)
        {
            this.idSalle = idSalle;
            this.nomSalle = nomSalle;
            this.capacite = capacite;
        }
        public int IdSalle { get => idSalle; }
        public string NomSalle { get => nomSalle; }
        public int Capacite { get => capacite; }
    }
}
