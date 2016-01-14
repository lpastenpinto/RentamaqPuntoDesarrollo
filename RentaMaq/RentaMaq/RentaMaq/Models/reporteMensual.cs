using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RentaMaq.Models;
using RentaMaq.DAL;
namespace RentaMaq.Models
{
    public class reporteMensual
    {
    }

    public class costosEquipos {
        public string afiEquipo { set; get; }
        public int idEquipo { set; get; }
        public double costosTotalesActivos { set; get; }
        public double cantidadHoras { set; get; }
        public double cantidadKilometros { set; get; }
        public double resultadoIndicador { set; get; }
        public List<IndicadoresCostosActivosOT> listaIndicadores { get; set; }
        public List<ordenDeTrabajoGeneral> listaOrdenesTrabajo { set; get; }
        public double horometroInicial { set; get; }
        public double horometroFinal { set; get; }
        public double kilometrosInicial { set; get; }
        public double kilometrosFinal { set; get; }

        public costosEquipos(int id,string numeroAfi, DateTime inicio, DateTime termino) {
            this.horometroInicial = 0;
            this.horometroFinal = 0;
            this.kilometrosInicial = 0;
            this.kilometrosFinal = 0;

            costosTotalesActivos = 0;
            this.cantidadKilometros = 0;
            Context db = new Context();
            this.afiEquipo = numeroAfi;
            this.idEquipo = id;
            string idEquipo = id.ToString();
            listaIndicadores = new List<IndicadoresCostosActivosOT>();
            this.listaOrdenesTrabajo =
                db.ordenDeTrabajoGenerals.Where(s => s.idEquipo == idEquipo & s.horasMantenimientoFecha >=inicio & s.horasMantenimientoFecha<=termino).ToList();         
            
            foreach (ordenDeTrabajoGeneral OT in listaOrdenesTrabajo)
            {
                listaIndicadores.Add(new IndicadoresCostosActivosOT(OT));
                costosTotalesActivos += listaIndicadores[listaIndicadores.Count - 1].totalCostos;
            }

            this.horometroFinal = registrokmhm.obtenerEstimado(id, termino).horometro;
            this.horometroInicial = registrokmhm.obtenerEstimado(id, inicio).horometro;
            this.cantidadHoras = this.horometroFinal - this.horometroInicial;
            if (this.cantidadHoras <= 0)
            {
                this.kilometrosFinal = registrokmhm.obtenerEstimado(id, termino).kilometraje;
                this.horometroInicial=registrokmhm.obtenerEstimado(id, inicio).kilometraje;
                this.cantidadKilometros = this.kilometrosFinal - this.kilometrosInicial;
                this.resultadoIndicador = costosTotalesActivos / cantidadKilometros;
            }
            else {
                this.resultadoIndicador = costosTotalesActivos / cantidadHoras;
           }                                         
        }
    
    }
}