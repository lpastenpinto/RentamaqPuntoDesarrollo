using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RentaMaq.Models
{
    public class registro
    {
        public int registroID { get;set;}
        public DateTime fecha { get; set; }
        public string descripcion { get; set; }
        public string tipoAccion{ get; set; }
        public string tipoDato { get; set; }
        public string usuario { get; set; }
        public int usuarioID { get; set; }
    }
}