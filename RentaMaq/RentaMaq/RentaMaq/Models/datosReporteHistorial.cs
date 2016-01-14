using RentaMaq.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RentaMaq.Models
{
    public class datosReporteHistorial
    {
        public string equipo { get; set; }
        public string fechaInicio { get; set; }
        public string fechaTermino { get; set; }

        internal static List<datosReporteHistorial> generarDatos(int equipoID, DateTime Inicio, DateTime Termino)
        {
            Context db = new Context();
            List<datosReporteHistorial> retorno = new List<datosReporteHistorial>();

            datosReporteHistorial nuevo = new datosReporteHistorial();
            nuevo.equipo = db.Equipos.Find(equipoID).numeroAFI;
            nuevo.fechaInicio = formatearString.fechaSinHoraDiaPrimero(Inicio);
            nuevo.fechaTermino = formatearString.fechaSinHoraDiaPrimero(Termino);

            retorno.Add(nuevo);

            return retorno;
        }
    }
}