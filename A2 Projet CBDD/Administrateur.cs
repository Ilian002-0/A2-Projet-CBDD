using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace A2_Projet_CBDD
{
    internal class Administrateur
    {

        public int IdAdmin { get; set; }
        public string Identifiant { get; set; }
        public string MotDePasse { get; set; }
        public string Role { get; set; } // 'Principal' ou 'Secondaire'

        // Connexion Admin : Vérifie l'identité dans la table Administrateur
        public static Administrateur SeConnecter(string identifiant, string mdp)
        {
            if (string.IsNullOrWhiteSpace(identifiant) || string.IsNullOrWhiteSpace(mdp))
            {
                return null;
            }
            try
            {
                using (MySqlConnection conn = Connexion.GetConnexionPublic())
                {
                    if (conn == null) return null; // Si la connexion elle-même échoue

                    string requet = "SELECT * FROM Administrateur WHERE Identifiant = @id AND MotDePasse = @mdp";
                    MySqlCommand cmd = new MySqlCommand(requet, conn);
                    cmd.Parameters.AddWithValue("@id", identifiant);
                    cmd.Parameters.AddWithValue("@mdp", mdp);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Administrateur
                            {
                                IdAdmin = reader.GetInt32("Id_Admin"),
                                Identifiant = reader.GetString("Identifiant"),
                                MotDePasse = reader.GetString("MotDePasse"),
                                Role = reader.GetString("Role_Admin")
                            };
                        }
                    }
                }
            }
            catch (MySqlException e)
            {
                // On attrape l'erreur ici pour éviter le crash
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nErreur technique lors de la connexion Admin : " + e.Message);
                Console.ResetColor();
                // Optionnel : Ajouter une pause pour lire l'erreur
                // System.Threading.Thread.Sleep(2000);
            }
            catch (Exception e)
            {
                Console.WriteLine("Erreur générique : " + e.Message);
            }

            return null; // Retourne null si échec ou erreur, le programme continue
        }

        // Action : Valider l'inscription d'un membre
        public bool ValiderInscription(int idMembre)
        {
            using (MySqlConnection conn = Connexion.GetConnexionAdmin()) // Requiert privilèges Admin
            {
                try
                {
                    string requet = "UPDATE Membre SET Statut = 'Valide' WHERE idMembre = @id";
                    MySqlCommand cmd = new MySqlCommand(requet, conn);
                    cmd.Parameters.AddWithValue("@id", idMembre);

                    int rows = cmd.ExecuteNonQuery();
                    return rows > 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erreur validation : " + ex.Message);
                    return false;
                }
            }
        }

        // Action : Supprimer un membre (et ses réservations associées)
        public bool SupprimerMembre(int idMembre)
        {
            using (MySqlConnection conn = Connexion.GetConnexionAdmin())
            {
                try
                {
                    // 1. Supprimer d'abord les réservations du membre (Contrainte Clé Étrangère)
                    string requetResa = "DELETE FROM Reservation WHERE idMembre = @id";
                    MySqlCommand cmdResa = new MySqlCommand(requetResa, conn);
                    cmdResa.Parameters.AddWithValue("@id", idMembre);
                    cmdResa.ExecuteNonQuery();

                    // 2. Supprimer le membre
                    string requetMembre = "DELETE FROM Membre WHERE idMembre = @id";
                    MySqlCommand cmdMembre = new MySqlCommand(requetMembre, conn);
                    cmdMembre.Parameters.AddWithValue("@id", idMembre);

                    cmdMembre.ExecuteNonQuery();
                    Console.WriteLine("Membre supprimé avec succès.");
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erreur suppression membre : " + ex.Message);
                    return false;
                }
            }
        }

        public void AfficherRapportStatistique()
        {
            using (MySqlConnection conn = Connexion.GetConnexionAdmin())
            {
                if (conn == null) return;

                Console.WriteLine("\n--- RAPPORT STATISTIQUE (Requis par le cahier des charges) ---");

                // 1. Moyenne durée des cours (Agrégation)
                AfficherRequete(conn, "Durée moyenne des cours", "SELECT AVG(Duree_Minutes) FROM Cours");

                // 2. Nombre total de membres (Agrégation)
                AfficherRequete(conn, "Nombre total de membres", "SELECT COUNT(*) FROM Membre");

                // 3. Cours plus longs que la moyenne (Sous-requête)
                Console.WriteLine("\n[Cours > Durée Moyenne]");
                AfficherListe(conn, "SELECT Nom_Cours FROM Cours WHERE Duree_Minutes > (SELECT AVG(Duree_Minutes) FROM Cours)");

                // 4. Membres intéressés par le Yoga (Sous-requête imbriquée)
                Console.WriteLine("\n[Membres ayant réservé du Yoga]");
                string sqlYoga = "SELECT Nom FROM Membre WHERE idMembre IN (SELECT idMembre FROM Reservation WHERE Id_Cours IN (SELECT Id_Cours FROM Cours WHERE Nom_Cours LIKE '%Yoga%'))";
                AfficherListe(conn, sqlYoga);
            }
        }

        // Helpers pour l'affichage rapide
        private void AfficherRequete(MySqlConnection conn, string titre, string requet)
        {
            MySqlCommand cmd = new MySqlCommand(requet, conn);
            object result = cmd.ExecuteScalar();
            Console.WriteLine($"{titre} : {result}");
        }

        private void AfficherListe(MySqlConnection conn, string requet)
        {
            MySqlCommand cmd = new MySqlCommand(requet, conn);
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read()) Console.WriteLine($"- {reader[0]}");
            }
        }
    }
}