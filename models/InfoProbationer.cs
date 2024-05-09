using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppManagement.models
{
    class InfoProbationer
    {
        public InfoProbationer(string nom, string prenom, string contact, string filiere, string niveau, string etablissement)
        {
            Nom = nom;
            Prenom = prenom;
            Contact = contact;
            Filiere = filiere;
            Niveau = niveau;
            Etablissement = etablissement;
        }

        [Required(ErrorMessage ="Tous les champs sont requis, veuillez les remplir")]
        public string Nom { get; set; }
        [Required(ErrorMessage ="Tous les champs sont requis, veuillez les remplir")]
        public string Prenom { get; set; }
        [Required(ErrorMessage ="Tous les champs sont requis, veuillez les remplir")]
        [DataType(DataType.PhoneNumber, ErrorMessage ="Veuillez rentrer un contact valide")]
        public string Contact { get; set; }
        [Required(ErrorMessage ="Tous les champs sont requis, veuillez les remplir")]
        public string Filiere { get; set; }
        [Required(ErrorMessage ="Tous les champs sont requis, veuillez les remplir")]
        public string Niveau { get; set; }
        [Required(ErrorMessage ="Tous les champs sont requis, veuillez les remplir")]
        public string Etablissement { get; set; }
    }
}
