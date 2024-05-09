using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppManagement.controllers;
using MySql.Data.MySqlClient;

namespace AppManagement.models
{
    class Attribution
    {
        public int Id { get; set; }

        [Required(ErrorMessage ="Ce champ est requis, veuillez le remplir")]
        [MinLength(10, ErrorMessage ="Veuillez saisir une attribution valide")]
        public string Attribut { get; set; }

        public Attribution()
        {
        }

        public Attribution(string attribut)
        {
            this.Attribut = attribut;
        }

        public Attribution(int id, string attribut)
        {
            this.Id = id;
            this.Attribut = attribut;
        }
    }
}
