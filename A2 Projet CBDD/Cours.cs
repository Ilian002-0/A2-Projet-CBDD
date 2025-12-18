using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace A2_Projet_CBDD
{
    internal class Cours
    {
        // Attributs privés
        private int idCours;
        private string nomCours;
        private string description;
        private DateTime dateHeure;
        private int dureeMinutes;
        private string niveauDifficulte;
        private int capaciteMax;
        private int idCoach;
        private int idSalle;

        // Constructeur complet (utile lors de la récupération depuis la BDD)
        public Cours(int id, string nom, string desc, DateTime date, int duree, string niveau, int capacite, int coach, int salle)
        {
            this.idCours = id;
            this.nomCours = nom;
            this.description = desc;
            this.dateHeure = date;
            this.dureeMinutes = duree;
            this.niveauDifficulte = niveau;
            this.capaciteMax = capacite;
            this.idCoach = coach;
            this.idSalle = salle;
        }

        // Constructeur pour la création d'un nouveau cours (sans ID, on le génère après)
        public Cours(string nom, string desc, DateTime date, int duree, string niveau, int capacite, int coach, int salle)
        {
            this.nomCours = nom;
            this.description = desc;
            this.dateHeure = date;
            this.dureeMinutes = duree;
            this.niveauDifficulte = niveau;
            this.capaciteMax = capacite;
            this.idCoach = coach;
            this.idSalle = salle;
        }

        // Propriétés publiques (Getters/Setters)
        public int IdCours { get => idCours; set => idCours = value; }
        public string NomCours { get => nomCours; set => nomCours = value; }
        public string Description { get => description; set => description = value; }
        public DateTime DateHeure { get => dateHeure; set => dateHeure = value; }
        public int DureeMinutes { get => dureeMinutes; set => dureeMinutes = value; }
        public string NiveauDifficulte { get => niveauDifficulte; set => niveauDifficulte = value; }
        public int CapaciteMax { get => capaciteMax; set => capaciteMax = value; }
        public int IdCoach { get => idCoach; set => idCoach = value; }
        public int IdSalle { get => idSalle; set => idSalle = value; }

        // --- MÉTHODES D'ACCÈS AUX DONNÉES (DAO) ---

        // 1. Récupérer tous les cours (Pour l'affichage Membre et Admin)
        public static List<Cours> GetTousLesCours()
        {
            List<Cours> liste = new List<Cours>();
            using (MySqlConnection conn = Connexion.GetConnexionPublic()) // 'AppPublic' peut lire les cours
            {
                if (conn == null) return liste;

                string query = "SELECT * FROM Cours ORDER BY Date_Heure";
                MySqlCommand cmd = new MySqlCommand(query, conn);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        liste.Add(new Cours(
                            reader.GetInt32("Id_Cours"),
                            reader.GetString("Nom_Cours"),
                            reader.IsDBNull(reader.GetOrdinal("Description")) ? "" : reader.GetString("Description"),
                            reader.GetDateTime("Date_Heure"),
                            reader.GetInt32("Duree_Minutes"),
                            reader.IsDBNull(reader.GetOrdinal("Niveau_Difficulte")) ? "" : reader.GetString("Niveau_Difficulte"),
                            reader.GetInt32("Capacite_Max"),
                            reader.GetInt32("Id_Coach"),
                            reader.GetInt32("Id_Salle")
                        ));
                    }
                }
            }
            return liste;
        }

        // 2. Ajouter un cours (Réservé aux Administrateurs)
        public bool AjouterCours()
        {
            // Utiliser la connexion Admin car AppPublic n'a pas le droit d'INSERT dans Cours
            using (MySqlConnection conn = Connexion.GetConnexionAdmin())
            {
                if (conn == null)
                {
                    Console.WriteLine("Erreur : Connexion administrateur requise.");
                    return false;
                }

                try
                {
                    // Génération d'un ID unique (car pas d'Auto-Increment sur la table Cours actuel)
                    this.idCours = GenererIdCoursUnique(conn);

                    string query = "INSERT INTO Cours (Id_Cours, Nom_Cours, Description, Date_Heure, Duree_Minutes, Niveau_Difficulte, Capacite_Max, Id_Coach, Id_Salle) " +
                                   "VALUES (@id, @nom, @desc, @date, @duree, @niveau, @capa, @coach, @salle)";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", this.idCours);
                    cmd.Parameters.AddWithValue("@nom", this.nomCours);
                    cmd.Parameters.AddWithValue("@desc", this.description);
                    cmd.Parameters.AddWithValue("@date", this.dateHeure);
                    cmd.Parameters.AddWithValue("@duree", this.dureeMinutes);
                    cmd.Parameters.AddWithValue("@niveau", this.niveauDifficulte);
                    cmd.Parameters.AddWithValue("@capa", this.capaciteMax);
                    cmd.Parameters.AddWithValue("@coach", this.idCoach);
                    cmd.Parameters.AddWithValue("@salle", this.idSalle);

                    cmd.ExecuteNonQuery();
                    Console.WriteLine("Cours ajouté avec succès !");
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erreur lors de l'ajout du cours : " + ex.Message);
                    return false;
                }
            }
        }

        // Méthode utilitaire pour générer un ID unique
        private int GenererIdCoursUnique(MySqlConnection conn)
        {
            Random rnd = new Random();
            int idTemp;
            bool unique = false;
            while (!unique)
            {
                idTemp = rnd.Next(100, 9999);
                string query = "SELECT COUNT(*) FROM Cours WHERE Id_Cours = @id";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", idTemp);

                int count = Convert.ToInt32(cmd.ExecuteScalar());
                if (count == 0) return idTemp;
            }
            return -1; // Ne devrait jamais arriver
        }

        // Pour un affichage facile dans la console
        public override string ToString()
        {
            return $"{IdCours} | {NomCours} | {DateHeure} | {DureeMinutes}min | Niveau: {NiveauDifficulte} | Places: {CapaciteMax}";
        }
    }
}