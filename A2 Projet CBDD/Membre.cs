using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace A2_Projet_CBDD
{
    internal class Membre
    {
        private int idMembre;
        private string nom;
        private string prenom;
        private string adresse;
        private string tel;
        private string mail;
        private DateTime Date_Inscription;
        private string MotDePasse;

        public Membre(int idMembre, string nom, string prenom, string adresse, string tel, string mail, string motDePasse)
        {
            DateTime date_now = DateTime.Now;
            this.idMembre = idMembre;
            this.nom = nom;
            this.prenom = prenom;
            this.adresse = adresse;
            this.tel = tel;
            this.mail = mail;
            Date_Inscription = date_now;
            MotDePasse = motDePasse;
        }

        public int IdMembre { get => idMembre; set => idMembre = value; }
        public string Nom { get => nom; set => nom = value; }
        public string Prenom { get => prenom; set => prenom = value; }
        public string Adresse { get => adresse; set => adresse = value; }
        public string Tel { get => tel; set => tel = value; }
        public string Mail { get => mail; set => mail = value; }
        public DateTime DateInscription { get => Date_Inscription; set => Date_Inscription = value; }
        public string Get_MotDePasse { get => MotDePasse; set => MotDePasse = value; }
    }
}
