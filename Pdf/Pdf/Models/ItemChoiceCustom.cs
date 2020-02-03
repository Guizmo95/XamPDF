using System;
using System.Collections.Generic;
using System.Text;

namespace Pdf.Models
{
    public class ItemChoiceCustom
    {
        private int id;
        private string libelle;
        private string detail;

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
    }
}
