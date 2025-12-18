using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace A2_Projet_CBDD
{
    internal class Program
    {
        static void DisplayMenu()
        {
            int choix = 0;
            Connexion conn=null;
            Membre user=null;
            Console.WriteLine("Menu Principal:\nQue souhaitez-vous faire ?\n"
            +"1. Je suis membre et je me connecte\n"
            +"2. Je m'inscris\n"
            +"3. Je suis administrateur\n"
            +"4. Quitter");

            while (choix < 1 || choix > 4)
            {
                string a = Console.ReadLine();
                int.TryParse(a, out choix);
            }
            switch (choix)
            {
                case 1:
                    string mail="", mdp="";
                    while (mail == ""|| mdp == "" || conn.SQL_connexion(mail, mdp) == false)
                    {
                        Console.WriteLine("Mail :\n");
                        mail = Console.ReadLine();
                        Console.WriteLine("Mot de passe :\n");
                        mdp = Console.ReadLine();
                        conn.SQL_connexion(mail, mdp);
                        Console.WriteLine(conn.GetMessage);
                        System.Threading.Thread.Sleep(2000);
                    }
                    MySqlConnection maConnexion = Connexion.Get_Connexion(mail, mdp);
                    user = new Membre(Membre.Get_ID(mail, maConnexion), maConnexion);
                    Console.Clear();
                    break;
                case 2:

                    break;
                case 3:

                    break;
                case 4:

                    break;
                default:

                    break;
            }

        }
        static void Main(string[] args)
        {
            DisplayMenu();
        }
    }
}
