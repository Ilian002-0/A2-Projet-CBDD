using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2_Projet_CBDD
{
    internal class Coach
    {
        private int Id_Coach;
        private string Nom;
        private string Prenom;
        private string Specialite;
        private string Telephone;

        public Coach(string nom, string prenom, string specialite, string telephone)
        {
            this.Nom = nom;
            this.Prenom = prenom;
            this.Specialite = specialite;
            this.Telephone = telephone;
        }

        public string Get_Nom
        {
            get { return Nom; }
            set { Nom = value; }
        }
        public string Get_Prenom
        {
            get { return Prenom; }
            set { Prenom = value; }
        }
        public string Get_Specialite
        {
            get { return Specialite; }
            set { Specialite = value; }
        }
        public string Get_Telephone
        {
            get { return Telephone; }
            set { Telephone = value; }
        }
    }
}