using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppManagement.controllers;

namespace AppManagement.models
{
    class InfoPerso
    {
        public InfoPerso(string nom, string prenoms, string sexe, string adresse, string contact)
        {
            Nom = nom;
            Prenoms = prenoms;
            Sexe = sexe;
            Adresse = adresse;
            Contact = contact;
        }

        [Required(ErrorMessage = "Tous les champs sont requis, veuillez les remplir")]
        public string Nom { get; set; }
        [Required(ErrorMessage = "Tous les champs sont requis, veuillez les remplir")]
        public string Prenoms { get; set; }
        [Required(ErrorMessage = "Tous les champs sont requis, veuillez les remplir")]
        public string Sexe { get; set; }
        [Required(ErrorMessage = "Tous les champs sont requis, veuillez les remplir")]
        public string Adresse { get; set; }
        [Required(ErrorMessage = "Tous les champs sont requis, veuillez les remplir")]
        public string Contact { get; set; }
    }
}
