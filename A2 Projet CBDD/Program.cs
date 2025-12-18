using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace A2_Projet_CBDD
{
    internal class Program
    {
        /*
id : admin_main | mdp :'1234'
id : admin_assistant | mdp : '1234'
 */
        static void Main(string[] args)
        {
            Console.Title = "GYM GESTION - Application de Gestion de Salle de Sport";
            bool quitter = false;

            while (!quitter)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("=============================================");
                Console.WriteLine("       BIENVENUE CHEZ GYM GESTION");
                Console.WriteLine("=============================================");
                Console.ResetColor();
                Console.WriteLine("1. Espace Membre (Connexion)");
                Console.WriteLine("2. S'inscrire (Nouveau Membre)");
                Console.WriteLine("3. Espace Administrateur");
                Console.WriteLine("4. Quitter l'application");
                Console.WriteLine("=============================================");
                Console.Write("Votre choix : ");

                string choix = Console.ReadLine();

                switch (choix)
                {
                    case "1":
                        GestionConnexionMembre();
                        break;
                    case "2":
                        GestionInscription();
                        break;
                    case "3":
                        GestionConnexionAdmin();
                        break;
                    case "4":
                        quitter = true;
                        break;
                    default:
                        AfficherMessage("Choix invalide.", ConsoleColor.Red);
                        break;
                }
            }
        }

        #region --- GESTION MEMBRE ---

        static void GestionInscription()
        {
            Console.Clear();
            Console.WriteLine("--- INSCRIPTION ---");

            Console.Write("Nom : "); string nom = Console.ReadLine();
            Console.Write("Prénom : "); string prenom = Console.ReadLine();
            Console.Write("Adresse : "); string adr = Console.ReadLine();
            Console.Write("Téléphone : "); string tel = Console.ReadLine();
            Console.Write("Email : "); string mail = Console.ReadLine();
            Console.Write("Mot de passe : "); string mdp = Console.ReadLine();

            // Vérification basique
            if (string.IsNullOrWhiteSpace(mail) || string.IsNullOrWhiteSpace(mdp))
            {
                AfficherMessage("Erreur : Email et mot de passe obligatoires.", ConsoleColor.Red);
                return;
            }

            // Génération ID Unique via la méthode optimisée (si ajoutée dans Membre.cs)
            // Sinon utiliser : new Random().Next(1, 10000);
            int id = new Random().Next(100, 10000);

            Membre nouveau = new Membre(id, nom, prenom, adr, tel, mail);

            if (nouveau.Inscrire(mdp))
            {
                AfficherMessage("Inscription réussie ! Votre compte est en attente de validation par un admin.", ConsoleColor.Green);
            }
            else
            {
                AfficherMessage("Erreur lors de l'inscription (Email peut-être déjà pris).", ConsoleColor.Red);
            }
        }
        static void GestionConnexionMembre()
        {
            Console.Clear();
            Console.WriteLine("--- CONNEXION MEMBRE ---");
            Console.Write("Email : "); string mail = Console.ReadLine();
            Console.Write("Mot de passe : "); string mdp = Console.ReadLine();

            Membre membre = Membre.SeConnecter(mail, mdp);

            if (membre != null)
            {
                AfficherMessage($"Connexion réussie ! Bonjour {membre.Prenom}.", ConsoleColor.Green);
                MenuEspaceMembre(membre);
            }
            else
            {
                AfficherMessage("Email ou mot de passe incorrect.", ConsoleColor.Red);
            }
        }
        static void MenuEspaceMembre(Membre membre)
        {
            bool retour = false;
            while (!retour)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"--- ESPACE DE {membre.Prenom.ToUpper()} (Statut: {membre.Statut}) ---");
                Console.ResetColor();
                Console.WriteLine("1. Consulter le planning des cours");
                Console.WriteLine("2. Réserver un cours");
                Console.WriteLine("3. Annuler une réservation"); // AJOUTÉ
                Console.WriteLine("4. Mes Réservations (Historique)");
                Console.WriteLine("5. Mes Informations");
                Console.WriteLine("6. Déconnexion");
                Console.Write("Choix : ");

                string choix = Console.ReadLine();
                switch (choix)
                {
                    case "1":
                        AfficherTousLesCours();
                        break;
                    case "2":
                        ProcessusReservation(membre);
                        break;
                    case "3":
                        ProcessusAnnulation(membre); // AJOUTÉ
                        break;
                    case "4":
                        AfficherHistorique(membre);
                        break;
                    case "5":
                        Console.WriteLine($"\nNom: {membre.Nom}\nEmail: {membre.Mail}\nDate inscription: {membre.DateInscription}");
                        Console.ReadKey();
                        break;
                    case "6":
                        retour = true;
                        break;
                }
            }
        }
        static void AfficherTousLesCours()
        {
            Console.Clear();
            Console.WriteLine("--- PLANNING DES COURS ---");
            List<Cours> cours = Cours.GetTousLesCours();
            foreach (var c in cours)
            {
                Console.WriteLine(c.ToString());
            }
            Console.WriteLine("\nAppuyez sur une touche pour revenir...");
            Console.ReadKey();
        }
        static void ProcessusReservation(Membre membre)
        {
            // Vérifier statut
            if (membre.Statut != "Valide")
            {
                AfficherMessage("Votre compte n'a pas encore été validé par un administrateur. Réservation impossible.", ConsoleColor.Yellow);
                return;
            }

            AfficherTousLesCours();
            Console.Write("\nEntrez l'ID du cours à réserver : ");
            if (int.TryParse(Console.ReadLine(), out int idCours))
            {
                if (membre.ReserverCours(idCours))
                {
                    AfficherMessage("Réservation confirmée !", ConsoleColor.Green);
                }
                else
                {
                    AfficherMessage("Échec de la réservation (Cours complet ou erreur technique).", ConsoleColor.Red);
                }
            }
        }
        static void AfficherHistorique(Membre membre)
        {
            Console.Clear();
            Console.WriteLine("--- MON HISTORIQUE ---");
            List<Reservation> resas = Reservation.GetHistorique(membre.IdMembre);

            if (resas.Count == 0) Console.WriteLine("Aucune réservation trouvée.");

            foreach (var r in resas)
            {
                Console.WriteLine(r.ToString());
            }
            Console.ReadKey();
        }

        #endregion

        #region --- GESTION ADMIN ---

        static void GestionConnexionAdmin()
        {
            Console.Clear();
            Console.WriteLine("--- ACCÈS ADMINISTRATEUR ---");
            Console.Write("Identifiant : ");
            string id = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(id)) return;
            Console.Write("Mot de passe : ");
            string mdp = Console.ReadLine();
            Administrateur admin = Administrateur.SeConnecter(id, mdp);
            if (admin != null)
            {
                AfficherMessage($"Bienvenue Admin ({admin.Role})", ConsoleColor.Magenta);
                MenuEspaceAdmin(admin);
            }
            else
            {
                AfficherMessage("Identifiants incorrects ou erreur de connexion.", ConsoleColor.Red);
            }
        }

        static void MenuEspaceAdmin(Administrateur admin)
        {
            bool retour = false;
            while (!retour)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("--- ADMINISTRATION ---");
                Console.ResetColor();
                Console.WriteLine("1. Gérer les Membres (Valider, Supprimer, Lister)");
                Console.WriteLine("2. Gérer les Coachs (Ajouter, Lister)"); // Modifié
                Console.WriteLine("3. Ajouter une Salle");
                Console.WriteLine("4. Ajouter un Cours");
                Console.WriteLine("5. Rapport Statistique");
                Console.WriteLine("6. Déconnexion");
                Console.Write("Choix : ");

                switch (Console.ReadLine())
                {
                    case "1":
                        SousMenuMembres(admin);
                        break;
                    case "2":
                        SousMenuCoachs(); // Nouvelle fonction ci-dessous
                        break;
                    // ... le reste ne change pas ...
                    case "3": AjouterSalleUI(); break;
                    case "4": AjouterCoursUI(); break;
                    case "5": admin.AfficherRapportStatistique(); Console.ReadKey(); break;
                    case "6": retour = true; break;
                }
            }
        }

        static void SousMenuMembres(Administrateur admin)
        {
            bool retour = false;
            while (!retour)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("--- GESTION DES MEMBRES ---");
                Console.ResetColor();
                Console.WriteLine("1. Valider un membre (Liste des 'En Attente')");
                Console.WriteLine("2. Supprimer un membre (Liste complète)");
                Console.WriteLine("3. Afficher TOUS les membres");
                Console.WriteLine("4. Retour au menu principal");
                Console.Write("Votre choix : ");

                string choixMenu = Console.ReadLine();

                switch (choixMenu)
                {
                    case "1": // --- VALIDER ---
                        List<Membre> enAttente = Membre.GetMembresEnAttente();
                        SelectionnerEtAgir(enAttente, "valider", (m) =>
                        {
                            if (admin.ValiderInscription(m.IdMembre))
                                AfficherMessage($"{m.Prenom} a été validé !", ConsoleColor.Green);
                            else
                                AfficherMessage("Erreur validation.", ConsoleColor.Red);
                        });
                        break;

                    case "2": // --- SUPPRIMER ---
                        List<Membre> tousPourSuppression = Membre.GetTousLesMembres();
                        SelectionnerEtAgir(tousPourSuppression, "supprimer DÉFINITIVEMENT", (m) =>
                        {
                            if (admin.SupprimerMembre(m.IdMembre))
                                AfficherMessage($"{m.Prenom} a été supprimé.", ConsoleColor.Green);
                            else
                                AfficherMessage("Erreur suppression.", ConsoleColor.Red);
                        });
                        break;

                    case "3": // --- AFFICHER TOUT ---
                        List<Membre> tous = Membre.GetTousLesMembres();
                        Console.WriteLine("\n--- LISTE COMPLÈTE DES MEMBRES ---");
                        Console.WriteLine($"{"NOM",-20} | {"MAIL",-30} | {"STATUT",-10}");
                        Console.WriteLine(new string('-', 65));
                        foreach (var m in tous)
                        {
                            Console.WriteLine($"{m.Nom + " " + m.Prenom,-20} | {m.Mail,-30} | {m.Statut,-10}");
                        }
                        Console.WriteLine("\nAppuyez sur une touche...");
                        Console.ReadKey();
                        break;

                    case "4":
                        retour = true;
                        break;
                }
            }
        }
        static void SelectionnerEtAgir(List<Membre> liste, string actionNom, Action<Membre> action)// Fonction utilitaire pour éviter de répéter le code du menu de sélection (1, 2, 3...)
        {
            if (liste.Count == 0)
            {
                AfficherMessage("Aucun membre trouvé pour cette action.", ConsoleColor.Yellow);
                return;
            }

            Console.Clear();
            Console.WriteLine($"--- SÉLECTIONNER UN MEMBRE À {actionNom.ToUpper()} ---");
            for (int i = 0; i < liste.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {liste[i].Nom} {liste[i].Prenom} ({liste[i].Statut})");
            }
            Console.WriteLine("0. Annuler");
            Console.Write($"\nNuméro du membre à {actionNom} : ");

            if (int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= liste.Count)
            {
                Membre selectionne = liste[index - 1];
                Console.Write($"Êtes-vous sûr de vouloir {actionNom} {selectionne.Nom} ? (o/n) : ");
                if (Console.ReadLine().ToLower() == "o")
                {
                    action(selectionne); // Exécute l'action (Valider ou Supprimer)
                }
            }
        }

        static void AjouterCoachUI()
        {
            Console.WriteLine("\n--- AJOUT COACH ---");
            Console.Write("Nom : "); string n = Console.ReadLine();
            Console.Write("Prénom : "); string p = Console.ReadLine();
            Console.Write("Spécialité : "); string s = Console.ReadLine();
            Console.Write("Tel : "); string t = Console.ReadLine();

            Coach c = new Coach(n, p, s, t);
            if (c.Ajouter()) AfficherMessage("Coach ajouté !", ConsoleColor.Green);
        }

        static void AjouterSalleUI()
        {
            Console.WriteLine("\n--- AJOUT SALLE ---");
            Console.Write("Nom Salle : "); string n = Console.ReadLine();
            Console.Write("Capacité : ");
            int.TryParse(Console.ReadLine(), out int cap);

            Salle s = new Salle(n, cap);
            if (s.Ajouter()) AfficherMessage("Salle ajoutée !", ConsoleColor.Green);
        }

        static void AjouterCoursUI()
        {
            Console.Clear();
            Console.WriteLine("--- CRÉATION D'UN COURS ---");
            // Afficher les aides
            Console.WriteLine("Liste des Coachs :");
            foreach (var co in Coach.GetTousLesCoachs()) Console.WriteLine(co.ToString());

            Console.WriteLine("\nListe des Salles :");
            foreach (var sa in Salle.GetToutesLesSalles()) Console.WriteLine(sa.ToString());

            Console.WriteLine("\n--- Détails du cours ---");
            Console.Write("Nom du cours : "); string nom = Console.ReadLine();
            Console.Write("Description : "); string desc = Console.ReadLine();
            Console.Write("Date (AAAA-MM-JJ HH:MM:SS) : ");
            DateTime.TryParse(Console.ReadLine(), out DateTime date);
            Console.Write("Durée (minutes) : "); int.TryParse(Console.ReadLine(), out int duree);
            Console.Write("Difficulté : "); string diff = Console.ReadLine();
            Console.Write("Capacité Max : "); int.TryParse(Console.ReadLine(), out int capa);
            Console.Write("ID du Coach : "); int.TryParse(Console.ReadLine(), out int idC);
            Console.Write("ID de la Salle : "); int.TryParse(Console.ReadLine(), out int idS);

            Cours nouveauCours = new Cours(nom, desc, date, duree, diff, capa, idC, idS);
            if (nouveauCours.AjouterCours())
            {
                AfficherMessage("Cours créé avec succès !", ConsoleColor.Green);
            }
            else
            {
                AfficherMessage("Erreur lors de la création.", ConsoleColor.Red);
            }
        }

        #endregion

        static void SousMenuCoachs()
        {
            Console.Clear();
            Console.WriteLine("1. Ajouter un coach");
            Console.WriteLine("2. Voir la liste des coachs");
            Console.Write("Choix : ");

            string choix = Console.ReadLine();
            if (choix == "1")
            {
                AjouterCoachUI(); // Votre fonction existante
            }
            else if (choix == "2")
            {
                List<Coach> coachs = Coach.GetTousLesCoachs();
                Console.WriteLine("\n--- ÉQUIPE DES COACHS ---");
                foreach (var c in coachs)
                {
                    // Assurez-vous d'avoir une méthode ToString() dans Coach.cs ou affichez comme ci-dessous
                    Console.WriteLine($"- {c.Nom} {c.Prenom} : {c.Specialite} (Tel: {c.Telephone})");
                }
                Console.WriteLine("\nAppuyez sur une touche...");
                Console.ReadKey();
            }
        }
        static void AfficherMessage(string message, ConsoleColor couleur)
        {
            Console.ForegroundColor = couleur;
            Console.WriteLine("\n" + message);
            Console.ResetColor();
            System.Threading.Thread.Sleep(1500); // Pause pour lire
        }
        static void ProcessusAnnulation(Membre membre)
        {
            Console.Clear();
            Console.WriteLine("--- ANNULATION ---");
            // On affiche d'abord l'historique pour aider l'utilisateur
            List<Reservation> resas = Reservation.GetHistorique(membre.IdMembre);
            if (resas.Count == 0)
            {
                AfficherMessage("Vous n'avez aucune réservation à annuler.", ConsoleColor.Yellow);
                return;
            }

            foreach (var r in resas)
            {
                // Note: Il faudrait idéalement stocker l'ID du cours dans l'objet Reservation pour faciliter la saisie
                Console.WriteLine(r.ToString());
            }

            Console.WriteLine("\n(Pour annuler, vous aurez besoin de l'ID du cours. Consultez le planning si besoin)");
            Console.Write("Entrez l'ID du cours à annuler : ");

            if (int.TryParse(Console.ReadLine(), out int idCours))
            {
                if (membre.AnnulerReservation(idCours))
                {
                    AfficherMessage("C'est noté, votre place est libérée.", ConsoleColor.Green);
                }
            }
        }
    }
}