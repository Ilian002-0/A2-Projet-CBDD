using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2_Projet_CBDD
{
    internal class Administrateur
    {
        private int ID_Admin;
        private string Identifiant;
        private string MotDePasse;
        private string Role_Admin;

        public Administrateur(int id_Admin, string identifiant, string motDePasse, string role_Admin)
        {
            ID_Admin = id_Admin;
            Identifiant = identifiant;
            MotDePasse = motDePasse;
            Role_Admin = role_Admin;
        }

        public int Get_ID_Admin
        {
            get { return ID_Admin; }
            set { ID_Admin = value; }
        }
        public string Get_Identifiant
        {
            get { return Identifiant; }
            set { Identifiant = value; }
        }
        public string Get_MotDePasse
        {
            get { return MotDePasse; }
            set { MotDePasse = value; }
        }
        public string Get_Role_Admin
        {
            get { return Role_Admin; }
            set { Role_Admin = value; }
        }
    }
}