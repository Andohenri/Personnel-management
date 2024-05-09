using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppManagement.models
{
    class Personnel
    {
        public Personnel(int id, string nom)
        {
            ID = id;
            Nom = nom;
        }

        public Personnel(string immatricule, string date_entree, string statut, string region, int id_attribut, InfoPerso infoPerso, Cin cin)
        {
            Immatricule = immatricule;
            Date_entree = date_entree;
            Statut = statut;
            Region = region;
            Id_attribut = id_attribut;
            InfoPerso = infoPerso;
            Cin = cin;
        }

        [Required(ErrorMessage = "Tous les champs sont requis, veuillez les remplir")] 
        public string Immatricule { get; set; }
        [Required(ErrorMessage = "Tous les champs sont requis, veuillez les remplir")]
        public string Date_entree { get; set; }
        [Required(ErrorMessage = "Tous les champs sont requis, veuillez les remplir")]
        public string Statut { get; set; }
        [Required(ErrorMessage = "Tous les champs sont requis, veuillez les remplir")]
        public string Region { get; set; }
        [Required(ErrorMessage = "Tous les champs sont requis, veuillez les remplir")]
        public int Id_attribut { get; set; }
        [Required(ErrorMessage = "Tous les champs sont requis, veuillez les remplir")]
        public InfoPerso InfoPerso { get; set; }
        [Required(ErrorMessage = "Tous les champs sont requis, veuillez les remplir")]
        public Cin Cin { get; set; }


        public int ID { get; set; }
        public string Nom { get; set; }
    }
}
