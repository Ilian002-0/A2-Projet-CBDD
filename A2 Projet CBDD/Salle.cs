using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace A2_Projet_CBDD
{
    internal class Salle
    {
        public int IdSalle { get; set; }
        public string NomSalle { get; set; }
        public int Capacite { get; set; }

        public Salle(int id, string nom, int capa)
        {
            IdSalle = id; NomSalle = nom; Capacite = capa;
        }

        // Pour création
        public Salle(string nom, int capa) { NomSalle = nom; Capacite = capa; }

        public static List<Salle> GetToutesLesSalles()
        {
            List<Salle> liste = new List<Salle>();
            using (MySqlConnection conn = Connexion.GetConnexionPublic())
            {
                if (conn == null) return liste;
                string query = "SELECT * FROM Salle";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        liste.Add(new Salle(
                            reader.GetInt32("Id_Salle"),
                            reader.GetString("Nom_Salle"),
                            reader.GetInt32("Capacite")
                        ));
                    }
                }
            }
            return liste;
        }

        // Admin : Ajouter Salle
        public bool Ajouter()
        {
            using (MySqlConnection conn = Connexion.GetConnexionAdmin())
            {
                try
                {
                    int newId = new Random().Next(200, 1000); // ID manuel

                    string query = "INSERT INTO Salle VALUES (@id, @nom, @capa)";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", newId);
                    cmd.Parameters.AddWithValue("@nom", NomSalle);
                    cmd.Parameters.AddWithValue("@capa", Capacite);

                    cmd.ExecuteNonQuery();
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erreur ajout salle : " + ex.Message);
                    return false;
                }
            }
        }

        public override string ToString()
        {
            return $"{IdSalle} - {NomSalle} (Cap: {Capacite})";
        }
    }
}