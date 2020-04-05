using System;
using System.Collections.Generic;
using System.Text;

namespace Pdf.Models
{
    public class ToolsCustomItem
    {
        private int id;
        private string libelle;
        private string detail;
        private string image;

        public int Id
        {
            get
            {
                return id;
            }

            set
            {
                id = value;
            }
        }

        public string Libelle
        {
            get
            {
                return libelle;
            }
            set
            {
                libelle = value;
            }
        }

        public string Detail
        {
            get
            {
                return detail;
            }

            set
            {
                detail = value;
            }
        }

        public string Image
        {
            get
            {
                return image;
            }

            set
            {
                image = value;
            }
        }

        public ToolsCustomItem(int id, string image, string libelle = null,  string detail = null)
        {
            this.id = id;
            this.libelle = libelle;
            this.image = image;
            this.detail = detail;
        }
    }
}
