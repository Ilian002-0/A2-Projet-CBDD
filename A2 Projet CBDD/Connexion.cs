using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace A2_Projet_CBDD
{
    internal class Connexion
    {
        private string mail;
        private string mdp;
        private string Message;
        public Connexion(string mail, string mdp)
        {
            this.mail = mail;
            this.mdp = mdp;
        }
        public bool SQL_connexion(string mail, string mdp)
        {
            MySqlConnection maConnexion = null;
            try
            {
                string connexionString = "SERVER=localhost;PORT=3306;" +
                                         "DATABASE=GymGestion;" +
                                         $"UID={mail};PASSWORD={mdp}";

                maConnexion = new MySqlConnection(connexionString);
                maConnexion.Open();
                Message = "Connexion établie !";
                return true;
            }
            catch (MySqlException e)
            {
                switch (e.Number)
                {
                    case 0:
                        Message = "Erreur de connexion au serveur";
                        break;
                    case 1045:
                        Message = "Erreur uid/password";
                        break;
                    default:
                        Message = " ErreurConnexion : " + e.ToString();
                        break;
                }
                return false;
            }
        }
        public string GetMessage
        {
            get { return Message; }
        }
    }
}
