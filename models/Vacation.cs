using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppManagement.models
{
    class Vacation
    {
        public Vacation(string immatricule, string dateDebut, int nbrJour, string dateFin, string motif, string substitute, string lieu)
        {
            Immatricule = immatricule;
            DateDebut = dateDebut;
            NbrJour = nbrJour;
            DateFin = dateFin;
            Motif = motif;
            Substitute = substitute;
            Lieu = lieu;
        }
        [Required(ErrorMessage = "Tous les champs sont requis, veuillez les remplir")]
        public string Immatricule { get; set; }
        [Required(ErrorMessage = "Tous les champs sont requis, veuillez les remplir")]
        public string DateDebut { get; set; }
        [Required(ErrorMessage = "Tous les champs sont requis, veuillez les remplir")]
        [Range(1, 15, ErrorMessage = "Le nombre maximum est de 15 jours, veuillez le corriger")]
        public int NbrJour { get; set; }
        [Required(ErrorMessage = "Tous les champs sont requis, veuillez les remplir")]
        public string DateFin { get; set; }
        [Required(ErrorMessage = "Tous les champs sont requis, veuillez les remplir")]
        public string Motif { get; set; }
        [Required(ErrorMessage = "Tous les champs sont requis, veuillez les remplir")]
        public string Substitute { get; set; }
        [Required(ErrorMessage = "Tous les champs sont requis, veuillez les remplir")]
        public string Lieu { get; set; }
    }
}
