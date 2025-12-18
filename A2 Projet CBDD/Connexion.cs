using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
namespace A2_Projet_CBDD
{
    internal class Connexion
    {
        // Identifiants définis dans votre fichier SQL (Fichier 3)
        // Note: Dans un vrai projet, ne jamais stocker les mots de passe en clair ici.
        private const string ConnectionStringPublic = "SERVER=localhost;PORT=3306;DATABASE=GymGestion;UID=AppPublic;PASSWORD=AppPublicPassword!3;";
        private const string ConnectionStringAdmin = "SERVER=localhost;PORT=3306;DATABASE=GymGestion;UID=PrincipalAdmin;PASSWORD=PrincipalAdminPassword!1;";

        public static MySqlConnection GetConnexionPublic()
        {
            try
            {
                MySqlConnection conn = new MySqlConnection(ConnectionStringPublic);
                conn.Open();
                return conn;
            }
            catch (MySqlException e)
            {
                Console.WriteLine("Erreur de connexion (Public) : " + e.Message);
                return null;
            }
        }

        public static MySqlConnection GetConnexionAdmin()
        {
            try
            {
                MySqlConnection conn = new MySqlConnection(ConnectionStringAdmin);
                conn.Open();
                return conn;
            }
            catch (MySqlException e)
            {
                Console.WriteLine("Erreur de connexion (Admin) : " + e.Message);
                return null;
            }
        }

    }
}