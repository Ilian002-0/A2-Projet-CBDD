using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private List<int> ID_existant= new List<int>();
        private MySqlConnection maConnexion = null;

        public Membre(int ID, MySqlConnection maConnexion)
        {
            string requete = "SELECT * FROM Membre WHERE idMembre=@ID;";
            MySqlCommand command2 = maConnexion.CreateCommand();
            command2.CommandText = requete;
            command2.Parameters.AddWithValue("@ID", ID);
            MySqlDataReader reader = command2.ExecuteReader();
            while (reader.Read())
            {
                this.idMembre = reader.GetInt32("idMembre");
                this.nom = reader.GetString("Nom");
                this.prenom = reader.GetString("Prenom");
                this.adresse = reader.GetString("Adresse");
                this.tel = reader.GetString("Tel");
                this.mail = reader.GetString("Mail");
                this.Date_Inscription = reader.GetDateTime("Date_Inscription");
                this.MotDePasse = reader.GetString("MotDePasse");
            }
            reader.Close();
            command2.Dispose();
        }
        public Membre(string nom, string prenom, string adresse, string tel, string mail, string motDePasse, MySqlConnection maConnexion)
        {
            DateTime date_now = DateTime.Now;
            this.nom = nom;
            this.prenom = prenom;
            this.adresse = adresse;
            this.tel = tel;
            this.mail = mail;
            Date_Inscription = date_now;
            MotDePasse = motDePasse;
            ID_existant = Get_ID_existant();
            this.maConnexion = maConnexion;
        }

        public int IdMembre { get => idMembre; set => idMembre = value; }
        public string Nom { get => nom; set => nom = value; }
        public string Prenom { get => prenom; set => prenom = value; }
        public string Adresse { get => adresse; set => adresse = value; }
        public string Tel { get => tel; set => tel = value; }
        public string Mail { get => mail; set => mail = value; }
        public DateTime DateInscription { get => Date_Inscription; set => Date_Inscription = value; }
        public string Get_MotDePasse { get => MotDePasse; set => MotDePasse = value; }

        private List<int> Get_ID_existant()
        {
            string requete = "SELECT idMembre FROM Membre;";
            MySqlCommand command2 = maConnexion.CreateCommand();
            command2.CommandText = requete;

            List<int> res = new List<int>();
            MySqlDataReader reader = command2.ExecuteReader();

            while (reader.Read())
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    res.Add(reader.GetInt32(i));
                }
            }
            reader.Close();
            command2.Dispose();
            return res;
        }
        public int Generer_ID()
        {
            Random rnd = new Random();
            int new_ID = rnd.Next(1, 1000);
            while (ID_existant.Contains(new_ID))
            {
                new_ID = rnd.Next(1, 1000);
            }
            ID_existant.Add(new_ID);
            return new_ID;
        }
        public static int Get_ID(string mail, MySqlConnection maConnexion)
        {
            string requete = "SELECT idMembre FROM Membre WHERE Mail=@Mail;";
            MySqlCommand command2 = maConnexion.CreateCommand();
            command2.CommandText = requete;
            command2.Parameters.AddWithValue("@Mail", mail);
            MySqlDataReader reader = command2.ExecuteReader();
            int res = -1;
            if (reader.Read())
            {
                res = reader.GetInt32("idMembre");
            }
            reader.Close();
            command2.Dispose();
            return res;
        }
    }
}
