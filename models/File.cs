using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppManagement.models
{
    class File
    {
        public File(int id, string nom_fichier)
        {
            ID = id;
            Name = nom_fichier;
        }

        public File(string nomFichier, byte[] contenu)
        {
            Name = nomFichier;
            Content = contenu;
        }

        public int ID { get; set; }
        public string Name { get; set; }
        public byte[] Content { get; set; }
    }
}
