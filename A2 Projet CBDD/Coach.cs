using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace A2_Projet_CBDD
{
    internal class Coach
    {
        public int IdCoach { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Specialite { get; set; }
        public string Telephone { get; set; }

        public Coach(int id, string nom, string prenom, string spe, string tel)
        {
            IdCoach = id; Nom = nom; Prenom = prenom; Specialite = spe; Telephone = tel;
        }

        // Constructeur pour création
        public Coach(string nom, string prenom, string spe, string tel)
        {
            Nom = nom; Prenom = prenom; Specialite = spe; Telephone = tel;
        }

        // Récupérer tous les coachs (Pour listes déroulantes ou affichage)
        public static List<Coach> GetTousLesCoachs()
        {
            List<Coach> liste = new List<Coach>();
            using (MySqlConnection conn = Connexion.GetConnexionPublic())
            {
                if (conn == null) return liste;
                string query = "SELECT * FROM Coach";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        liste.Add(new Coach(
                            reader.GetInt32("Id_Coach"),
                            reader.GetString("Nom"),
                            reader.GetString("Prenom"),
                            reader.GetString("Specialite"),
                            reader.GetString("Telephone")
                        ));
                    }
                }
            }
            return liste;
        }

        // Admin : Ajouter un coach
        public bool Ajouter()
        {
            using (MySqlConnection conn = Connexion.GetConnexionAdmin())
            {
                try
                {
                    // Génération ID (Manuel car pas d'auto-increment)
                    int newId = new Random().Next(10, 1000);
                    // Note: Dans un vrai projet, vérifiez l'unicité comme pour Cours.cs

                    string query = "INSERT INTO Coach VALUES (@id, @nom, @prenom, @spe, @tel)";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", newId);
                    cmd.Parameters.AddWithValue("@nom", Nom);
                    cmd.Parameters.AddWithValue("@prenom", Prenom);
                    cmd.Parameters.AddWithValue("@spe", Specialite);
                    cmd.Parameters.AddWithValue("@tel", Telephone);

                    cmd.ExecuteNonQuery();
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erreur ajout coach : " + ex.Message);
                    return false;
                }
            }
        }

        public override string ToString()
        {
            return $"{IdCoach} - {Nom} {Prenom} ({Specialite})";
        }
    }
}