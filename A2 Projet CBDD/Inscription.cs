using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static Mysqlx.Expect.Open.Types.Condition.Types;

namespace A2_Projet_CBDD
{
    internal class Inscription
    {
            private int idInscript;
            private int idMembre;
            private int idSalle;
            private DateTime dateInscrit;
            private DateTime dateFin;
            private string statut;
        public Inscription(int idInscript, int idMembre, int idSalle)
            {
            this.idInscript = idInscript;
            this.idMembre = idMembre;
            this.idSalle = idSalle;
            this.statut = "EnAttente"; // "EnAttente", "Active", "Terminee", "Refuse"
        }

        public int IdInscript { get => idInscript;}
        public int IdMembre { get => idMembre;}
        public int IdSalle { get => idSalle;}
        public DateTime DateInscrit { get => dateInscrit;}
        public DateTime DateFin { get => dateFin;}
        public string Statut { get => statut;}

        public void Inscription_validee()
        {
            this.dateInscrit = DateTime.Now;
            this.statut = "Active";
        }
        public void Inscription_Termnie()
        {
            this.dateFin = DateTime.Now;
            this.statut = "Terminee";
        }
        public void Inscription_Refuse()
        {
            this.statut = "Refuse";
        }
    }
}
