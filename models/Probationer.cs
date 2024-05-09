using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppManagement.models
{
    class Probationer
    {
        public Probationer(int idFile, int encadreur, string dateEnter, int duration, string theme, string note, InfoProbationer info_stagiaire)
        {
            IdFile = idFile;
            IdEncadreur = encadreur;
            DateEnter = dateEnter;
            Duration = duration;
            Theme = theme;
            Note = note;
            Info_Stagiaire = info_stagiaire;
        }

        [Required(ErrorMessage ="Tous les champs sont requis, veuillez les remplir")]
        public int IdFile { get; }
        [Required(ErrorMessage = "Tous les champs sont requis, veuillez les remplir")]
        public int IdEncadreur { get; }
        [Required(ErrorMessage ="Tous les champs sont requis, veuillez les remplir")]
        public string DateEnter { get; }
        [Required(ErrorMessage ="Tous les champs sont requis, veuillez les remplir")]
        [Range(0, 6, ErrorMessage = "La durée du stage est pas plus de 6 mois")]
        public int Duration { get; }
        [Required(ErrorMessage ="Tous les champs sont requis, veuillez les remplir")]
        public string Theme { get; }
        [Range(0, 20, ErrorMessage = "La note attribuée est comprise entre 0 et 20")]
        public string Note { get; }
        [Required(ErrorMessage ="Tous les champs sont requis, veuillez les remplir")]
        public InfoProbationer Info_Stagiaire { get; }
    }
}
