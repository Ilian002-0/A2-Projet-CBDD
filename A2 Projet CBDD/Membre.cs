using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace A2_Projet_CBDD
{
    internal class Membre
    {
        public int IdMembre { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Adresse { get; set; }
        public string Tel { get; set; }
        public string Mail { get; set; }
        public DateTime DateInscription { get; set; }
        public string Statut { get; set; } // 'EnAttente' ou 'Valide'

        public Membre() { }

        // Constructeur pour créer un membre depuis l'interface
        public Membre(int id, string nom, string prenom, string adresse, string tel, string mail)
        {
            this.IdMembre = id;
            this.Nom = nom;
            this.Prenom = prenom;
            this.Adresse = adresse;
            this.Tel = tel;
            this.Mail = mail;
            this.Statut = "EnAttente";
            this.DateInscription = DateTime.Now;
        }

        // Méthode statique pour se connecter (Login)
        public static Membre SeConnecter(string mail, string motDePasse)
        {
            using (MySqlConnection conn = Connexion.GetConnexionPublic())
            {
                if (conn == null) return null;

                string query = "SELECT * FROM Membre WHERE mail = @mail AND MotDePasse = @mdp";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@mail", mail);
                cmd.Parameters.AddWithValue("@mdp", motDePasse);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Membre
                        {
                            IdMembre = reader.GetInt32("idMembre"),
                            Nom = reader.GetString("nom"),
                            Prenom = reader.GetString("prenom"),
                            Adresse = reader.GetString("adresse"),
                            Tel = reader.GetString("tel"),
                            Mail = reader.GetString("mail"),
                            DateInscription = reader.GetDateTime("Date_Inscription"),
                            Statut = reader.GetString("Statut")
                        };
                    }
                }
            }
            return null; // Échec connexion
        }

        public bool Inscrire(string motDePasse)
        {
            using (MySqlConnection conn = Connexion.GetConnexionPublic())
            {
                if (conn == null) return false;

                try
                {
                    // Note: IdMembre n'est pas AUTO_INCREMENT dans votre SQL, il faut le gérer.
                    // Ici on utilise une méthode simple ou aléatoire pour l'exemple, mais l'idéal est de changer le SQL en AUTO_INCREMENT.

                    string query = "INSERT INTO Membre (idMembre, nom, prenom, adresse, tel, mail, Date_Inscription, MotDePasse, Statut) " +
                                   "VALUES (@id, @nom, @prenom, @adr, @tel, @mail, @date, @mdp, 'EnAttente')";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", this.IdMembre);
                    cmd.Parameters.AddWithValue("@nom", this.Nom);
                    cmd.Parameters.AddWithValue("@prenom", this.Prenom);
                    cmd.Parameters.AddWithValue("@adr", this.Adresse);
                    cmd.Parameters.AddWithValue("@tel", this.Tel);
                    cmd.Parameters.AddWithValue("@mail", this.Mail);
                    cmd.Parameters.AddWithValue("@date", DateTime.Now);
                    cmd.Parameters.AddWithValue("@mdp", motDePasse);

                    cmd.ExecuteNonQuery();
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erreur inscription : " + ex.Message);
                    return false;
                }
            }
        }
        public bool ReserverCours(int idCours)
        {
            if (this.Statut != "Valide")
            {
                Console.WriteLine("Votre compte doit être validé par un administrateur pour réserver.");
                return false;
            }

            using (MySqlConnection conn = Connexion.GetConnexionPublic())
            {
                if (conn == null) return false;

                try
                {
                    // ÉTAPE 1 : Vérifier la capacité restante
                    // On récupère la capacité max du cours ET le nombre d'inscrits actuel
                    string queryCheck = @"SELECT 
                                            (SELECT Capacite_Max FROM Cours WHERE Id_Cours = @id) as Max,
                                            (SELECT COUNT(*) FROM Reservation WHERE Id_Cours = @id) as Inscrits";

                    MySqlCommand cmdCheck = new MySqlCommand(queryCheck, conn);
                    cmdCheck.Parameters.AddWithValue("@id", idCours);

                    using (MySqlDataReader reader = cmdCheck.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Gestion du cas où le cours n'existe pas (Max est NULL)
                            if (reader.IsDBNull(0))
                            {
                                Console.WriteLine("Ce cours n'existe pas.");
                                return false;
                            }

                            int max = reader.GetInt32("Max");
                            int inscrits = reader.GetInt32("Inscrits");

                            // CONTRAINTE RESPECTÉE : Si c'est plein, on bloque
                            if (inscrits >= max)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine($"\nERREUR : Ce cours est complet ({inscrits}/{max} places).");
                                Console.ResetColor();
                                return false;
                            }
                        }
                    } // Le reader est fermé ici, on peut faire une nouvelle requête

                    // ÉTAPE 2 : Procéder à la réservation
                    int idResa = new Random().Next(10000, 99999);
                    string queryInsert = "INSERT INTO Reservation (Id_Reservation, idMembre, Id_Cours, Date_Reservation, Statut_Reservation) " +
                                         "VALUES (@idResa, @idMembre, @idCours, @date, 'Confirme')";

                    MySqlCommand cmdInsert = new MySqlCommand(queryInsert, conn);
                    cmdInsert.Parameters.AddWithValue("@idResa", idResa);
                    cmdInsert.Parameters.AddWithValue("@idMembre", this.IdMembre);
                    cmdInsert.Parameters.AddWithValue("@idCours", idCours);
                    cmdInsert.Parameters.AddWithValue("@date", DateTime.Now);

                    cmdInsert.ExecuteNonQuery();
                    return true;
                }
                catch (MySqlException ex)
                {
                    // Gestion du doublon (si le membre est déjà inscrit à ce cours)
                    if (ex.Number == 1062) Console.WriteLine("Vous avez déjà réservé ce cours !");
                    else Console.WriteLine("Erreur technique : " + ex.Message);

                    return false;
                }
            }
        }
        public bool AnnulerReservation(int idCours)
        {
            using (MySqlConnection conn = Connexion.GetConnexionPublic())
            {
                if (conn == null) return false;

                try
                {
                    string query = "DELETE FROM Reservation WHERE idMembre = @idM AND Id_Cours = @idC";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@idM", this.IdMembre);
                    cmd.Parameters.AddWithValue("@idC", idCours);

                    int rows = cmd.ExecuteNonQuery();
                    if (rows > 0)
                    {
                        Console.WriteLine("Réservation annulée avec succès.");
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("Aucune réservation trouvée pour ce cours.");
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erreur annulation : " + ex.Message);
                    return false;
                }
            }
        }
        public static int GenererIdUnique()
        {
            Random rnd = new Random();
            int nouvelId = 0;
            bool estUnique = false;

            using (MySqlConnection conn = Connexion.GetConnexionPublic())
            {
                if (conn == null) return -1; // Gestion d'erreur

                while (!estUnique)
                {
                    nouvelId = rnd.Next(1, 10000); // Génère un ID entre 1 et 10000

                    string query = "SELECT COUNT(*) FROM Membre WHERE idMembre = @id";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", nouvelId);

                    // Si le résultat est 0, l'ID est libre
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    if (count == 0)
                    {
                        estUnique = true;
                    }
                }
            }
            return nouvelId;
        }
        public static List<Membre> GetMembresEnAttente()
        {
            List<Membre> liste = new List<Membre>();
            using (MySqlConnection conn = Connexion.GetConnexionPublic())
            {
                if (conn == null) return liste;

                string query = "SELECT * FROM Membre WHERE Statut = 'EnAttente'";
                MySqlCommand cmd = new MySqlCommand(query, conn);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        liste.Add(new Membre
                        {
                            IdMembre = reader.GetInt32("idMembre"),
                            Nom = reader.GetString("nom"),
                            Prenom = reader.GetString("prenom"),
                            Mail = reader.GetString("mail"),
                            DateInscription = reader.GetDateTime("Date_Inscription"),
                            Statut = reader.GetString("Statut")
                        });
                    }
                }
            }
            return liste;
        }
        public static List<Membre> GetTousLesMembres()
        {
            List<Membre> liste = new List<Membre>();
            using (MySqlConnection conn = Connexion.GetConnexionPublic())
            {
                if (conn == null) return liste;
                string query = "SELECT * FROM Membre ORDER BY Nom";
                MySqlCommand cmd = new MySqlCommand(query, conn);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        liste.Add(new Membre
                        {
                            IdMembre = reader.GetInt32("idMembre"),
                            Nom = reader.GetString("nom"),
                            Prenom = reader.GetString("prenom"),
                            Mail = reader.GetString("mail"),
                            Tel = reader.GetString("tel"),
                            DateInscription = reader.GetDateTime("Date_Inscription"),
                            Statut = reader.GetString("Statut")
                        });
                    }
                }
            }
            return liste;
        }
    }
}