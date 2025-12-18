using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace A2_Projet_CBDD
{
    internal class Reservation
    {
        public int IdReservation { get; set; }
        public int IdMembre { get; set; }
        public string NomCours { get; set; } // On stocke le nom pour l'affichage
        public DateTime DateReservation { get; set; }
        public string Statut { get; set; }

        // Récupérer l'historique des réservations d'un membre (avec Jointure SQL)
        public static List<Reservation> GetHistorique(int idMembre)
        {
            List<Reservation> liste = new List<Reservation>();
            using (MySqlConnection conn = Connexion.GetConnexionPublic())
            {
                if (conn == null) return liste;

                // Jointure pour avoir le nom du cours directement
                string query = "SELECT r.Id_Reservation, r.Date_Reservation, r.Statut_Reservation, c.Nom_Cours " +
                               "FROM Reservation r " +
                               "JOIN Cours c ON r.Id_Cours = c.Id_Cours " +
                               "WHERE r.idMembre = @id";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", idMembre);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        liste.Add(new Reservation
                        {
                            IdReservation = reader.GetInt32("Id_Reservation"),
                            DateReservation = reader.GetDateTime("Date_Reservation"),
                            Statut = reader.GetString("Statut_Reservation"),
                            NomCours = reader.GetString("Nom_Cours")
                        });
                    }
                }
            }
            return liste;
        }

        public override string ToString()
        {
            return $"{DateReservation.ToShortDateString()} - {NomCours} ({Statut})";
        }
    }
}