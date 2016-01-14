using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RentaMaq.Models;
using RentaMaq.DAL;

namespace RentaMaq.Controllers
{
    public class reporteMensualController : Controller
    {
        private Context db = new Context();
        // GET: reporteMensual
        public ActionResult costosActivos(string anio, string mes)
        {
            DateTime Inicio = new DateTime();
            DateTime Termino = new DateTime();

            if (anio == null || mes == null)
            {

                DateTime fechaAnterior = DateTime.Today.AddMonths(-1);

                int diasEnMesAnterior = DateTime.DaysInMonth(fechaAnterior.Year, fechaAnterior.Month);

                Inicio = new DateTime(fechaAnterior.Year, fechaAnterior.Month, 1);
                Termino = new DateTime(fechaAnterior.Year, fechaAnterior.Month, diasEnMesAnterior);
            }
            else {
                int nuevoAnio = Convert.ToInt32(anio);
                int nuevoMes = Convert.ToInt32(mes);
                int diasEnMesAnterior = DateTime.DaysInMonth(nuevoAnio, nuevoMes);

                Inicio = new DateTime(nuevoAnio, nuevoMes, 1);
                Termino = new DateTime(nuevoAnio, nuevoMes, diasEnMesAnterior);
            }


            ViewBag.AnioActual = DateTime.Today.Year;
            ViewBag.Anio = DateTime.Today.Year - 1;
            ViewBag.AnioEscogio = Termino.Year;
            ViewBag.Mes = Termino.Month;
            List<equipos> Equipos = new Context().Equipos.ToList();

            List<costosEquipos> lista = new List<costosEquipos>();
            foreach (equipos Equipo in Equipos)
            {
                lista.Add(new costosEquipos(Equipo.ID, Equipo.numeroAFI, Inicio, Termino));
            }
            
            return View(lista);            
        }

        public ActionResult costosActivosDetalle(int id,string anio, string mes)
        {
            DateTime Inicio = new DateTime();
            DateTime Termino = new DateTime();

            if (anio == null || mes == null)
            {

                DateTime fechaAnterior = DateTime.Today.AddMonths(-1);

                int diasEnMesAnterior = DateTime.DaysInMonth(fechaAnterior.Year, fechaAnterior.Month);

                Inicio = new DateTime(fechaAnterior.Year, fechaAnterior.Month, 1);
                Termino = new DateTime(fechaAnterior.Year, fechaAnterior.Month, diasEnMesAnterior);
            }
            else
            {
                int nuevoAnio = Convert.ToInt32(anio);
                int nuevoMes = Convert.ToInt32(mes);
                int diasEnMesAnterior = DateTime.DaysInMonth(nuevoAnio, nuevoMes);

                Inicio = new DateTime(nuevoAnio, nuevoMes, 1);
                Termino = new DateTime(nuevoAnio, nuevoMes, diasEnMesAnterior);
            }


            ViewBag.AnioActual = DateTime.Today.Year;
            ViewBag.Anio = DateTime.Today.Year - 1;
            ViewBag.AnioEscogio = Termino.Year;
            ViewBag.Mes = Termino.Month;
            equipos Equipo = equipos.Obtener(id);

            costosEquipos CostoEquipo = new costosEquipos(Equipo.ID, Equipo.numeroAFI, Inicio, Termino);            
            
            List<double> costosOTs= new List<double>();
            foreach (IndicadoresCostosActivosOT costo in CostoEquipo.listaIndicadores) {
                double resultadoCosto = 0;
                foreach (materialesUtilizadosOT matUt in costo.materialesUtilizados) {
                    resultadoCosto+= matUt.precioActual * matUt.cantidad;                                   
                }
                costosOTs.Add(resultadoCosto);
            }
            ViewData["CostosOTs"] = costosOTs;
            return View(CostoEquipo);
        }

    }
}