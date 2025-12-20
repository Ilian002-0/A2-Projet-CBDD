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

                AfficherRequete(conn, "Durée moyenne des cours", "SELECT AVG(Duree_Minutes) FROM Cours");

                AfficherRequete(conn, "Nombre total de membres", "SELECT COUNT(*) FROM Membre");
                Console.WriteLine("\n[Cours > Durée Moyenne]");
                AfficherListe(conn, "SELECT Nom_Cours FROM Cours WHERE Duree_Minutes > (SELECT AVG(Duree_Minutes) FROM Cours)");
                Console.WriteLine("\n[Membres ayant réservé du Yoga]");
                string sqlYoga = "SELECT Nom FROM Membre WHERE idMembre IN (SELECT idMembre FROM Reservation WHERE Id_Cours IN (SELECT Id_Cours FROM Cours WHERE Nom_Cours LIKE '%Yoga%'))";
                AfficherListe(conn, sqlYoga);
            }
        }
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
        // --- NOUVELLE MÉTHODE POUR LES JOINTURES (LEFT & RIGHT) ---
        public void AfficherJointuresSpeciales()
        {
            using (MySqlConnection conn = Connexion.GetConnexionAdmin())
            {
                if (conn == null) return;

                Console.WriteLine("\n--- ANALYSE TECHNIQUE (JOIN) ---");

                // 1. REQUÊTE LEFT JOIN
                // Objectif : Afficher TOUS les coachs, y compris ceux qui n'ont pas de cours assigné.
                Console.WriteLine("\n[1] Disponibilité des Coachs (LEFT JOIN)");
                Console.WriteLine($"{"COACH",-20} | {"COURS ASSIGNÉ",-20}");
                Console.WriteLine(new string('-', 45));

                string queryLeft = "SELECT c.Nom, c.Prenom, cr.Nom_Cours " +
                                   "FROM Coach c " +
                                   "LEFT JOIN Cours cr ON c.Id_Coach = cr.Id_Coach";

                MySqlCommand cmdLeft = new MySqlCommand(queryLeft, conn);
                using (MySqlDataReader reader = cmdLeft.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string nomCoach = $"{reader.GetString("Nom")} {reader.GetString("Prenom")}";
                        // Si la colonne de droite (Cours) est NULL, c'est que le coach n'a pas de cours
                        string cours = reader.IsDBNull(reader.GetOrdinal("Nom_Cours")) ? ">> AUCUN COURS <<" : reader.GetString("Nom_Cours");

                        Console.WriteLine($"{nomCoach,-20} | {cours,-20}");
                    }
                }

                // 2. REQUÊTE RIGHT JOIN
                // Objectif : Afficher TOUTES les salles, y compris celles qui n'ont aucun cours programmé.
                Console.WriteLine("\n[2] Occupation des Salles (RIGHT JOIN)");
                Console.WriteLine($"{"SALLE",-20} | {"COURS PRÉVU",-20}");
                Console.WriteLine(new string('-', 45));

                string queryRight = "SELECT cr.Nom_Cours, s.Nom_Salle " +
                                    "FROM Cours cr " +
                                    "RIGHT JOIN Salle s ON cr.Id_Salle = s.Id_Salle";

                MySqlCommand cmdRight = new MySqlCommand(queryRight, conn);
                using (MySqlDataReader reader = cmdRight.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string salle = reader.GetString("Nom_Salle");
                        // Si la colonne de gauche (Cours) est NULL, c'est que la salle est vide
                        string cours = reader.IsDBNull(reader.GetOrdinal("Nom_Cours")) ? ">> SALLE VIDE <<" : reader.GetString("Nom_Cours");

                        Console.WriteLine($"{salle,-20} | {cours,-20}");
                    }
                }
            }
        }
    }
}