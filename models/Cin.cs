using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppManagement.models
{
    class Cin
    {
        public Cin(string num_cin, string date_delivrance, string lieu_delivrance, string duplicata, string lieu_duplicata)
        {
            Num_cin = num_cin;
            Date_delivrance = date_delivrance;
            Lieu_delivrance = lieu_delivrance;
            Duplicata = duplicata;
            Lieu_duplicata = lieu_duplicata;
        }

        [Required(ErrorMessage = "Tous les champs sont requis, veuillez les remplir")]
        [MinLength(12, ErrorMessage = "Ce champ doit comporter 12 caractères")]
        [StringLength(12, ErrorMessage = "Ce champ doit comporter 12 caractères")]
        public string Num_cin { get; set; }
        [Required(ErrorMessage = "Tous les champs sont requis, veuillez les remplir")]
        public string Date_delivrance { get; set; }
        [Required(ErrorMessage = "Tous les champs sont requis, veuillez les remplir")]
        public string Lieu_delivrance { get; set; }
        [Required(ErrorMessage = "Tous les champs sont requis, veuillez les remplir")]
        public string Duplicata { get; set; }
        [Required(ErrorMessage = "Tous les champs sont requis, veuillez les remplir")]
        public string Lieu_duplicata { get; set; }
    }
}
