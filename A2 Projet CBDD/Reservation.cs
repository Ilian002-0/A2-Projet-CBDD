using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2_Projet_CBDD
{
    internal class Reservation
    {

        private int Id_Reservation;
        private int idMembre;
        private int Id_Cours;
        private DateTime Date_Reservation;
        private string Statut_Reservation;

        public Reservation(int idMembre, int id_Cours, string statut_Reservation)
        {
            DateTime date_Reservation = DateTime.Now;
            this.idMembre = idMembre;
            this.Id_Cours = id_Cours;
            this.Date_Reservation = date_Reservation;
            this.Statut_Reservation = statut_Reservation;
        }

        public int Get_IdMembre
        {
            get { return idMembre; }
            set { idMembre = value; }
        }
        public int Get_Id_Cours
        {
            get { return Id_Cours; }
            set { Id_Cours = value; }
        }
        public string Get_Statut_Reservation
        {
            get { return Statut_Reservation; }
            set { Statut_Reservation = value; }
        }
        public DateTime Get_Date_Reservation
        {
            get { return Date_Reservation; }
            set { Date_Reservation = value; }
        }
    }
}