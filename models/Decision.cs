using AppManagement.controllers;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppManagement.models
{
    class Decision
    {
        public Decision(string numDecision, string immatricule, string decisionDate, int nbrJourAcc, string id_annee, byte[] proof)
        {
            NumDecision = numDecision;
            Immatricule = immatricule;
            DecisionDate = decisionDate;
            NbrJourAcc = nbrJourAcc;
            IdAnnee = id_annee;
            Proof = proof;
        }

        [Required(ErrorMessage = "Tous les champs sont requis, veuillez les remplir")]
        public string NumDecision { get; set; }
        [Required(ErrorMessage = "Tous les champs sont requis, veuillez les remplir")]
        public string Immatricule { get; set; }
        [Required(ErrorMessage = "Tous les champs sont requis, veuillez les remplir")]
        public string DecisionDate { get; set; }
        [Required(ErrorMessage = "Tous les champs sont requis, veuillez les remplir")]
        [Range(1,30, ErrorMessage = "Le nombre de jour de congé ne doit pas depasser de 30 jours")]
        public int NbrJourAcc { get; set; }
        public string IdAnnee { get; set; }
        [Required(ErrorMessage = "Veuillez choisir un fichier (Image ou PDF)")]
        public byte[] Proof { get; set; }
    }
}
