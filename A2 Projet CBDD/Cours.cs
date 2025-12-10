using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2_Projet_CBDD
{
    internal class Cours
    {
        private int Id_Cours;
        private string Nom_Cours;
        private string Description;
        private DateTime Date_Heure;
        private int Duree_Minutes;
        private string Niveau_Difficulte;
        private int Capacite_Max;
        private int Id_Coach;
        private int Id_Salle;

        public Cours(string Nom_Cours, string Description, DateTime Date_Heure, int Duree_Minutes, string Niveau_Difficulte, int Capacite_Max, int Id_Coach, int Id_Salle)
        {
            this.Nom_Cours = Nom_Cours;
            this.Description = Description;
            this.Date_Heure = Date_Heure;
            this.Duree_Minutes = Duree_Minutes;
            this.Niveau_Difficulte = Niveau_Difficulte;
            this.Capacite_Max = Capacite_Max;
            this.Id_Coach = Id_Coach;
            this.Id_Salle = Id_Salle;
        }

        public string Get_Nom_Cours
        {
            get { return Nom_Cours; }
            set { Nom_Cours = value; }
        }
        public string Get_Description
        {
            get { return Description; }
            set { Description = value; }
        }
        public DateTime Get_Date_Heure
        {
            get { return Date_Heure; }
            set { Date_Heure = value; }
        }
        public int Get_Duree_Minutes
        {
            get { return Duree_Minutes; }
            set { Duree_Minutes = value; }
        }
        public string Get_Niveau_Difficulte
        {
            get { return Niveau_Difficulte; }
            set { Niveau_Difficulte = value; }
        }
        public int Get_Capacite_Max
        {
            get { return Capacite_Max; }
            set { Capacite_Max = value; }
        }
        public int Get_Coach_Max
        {
            get { return Id_Coach; }
            set { Id_Coach = value; }
        }
        public int Get_Salle_Max
        {
            get { return Id_Salle; }
            set { Id_Salle = value; }
        }

    }
}