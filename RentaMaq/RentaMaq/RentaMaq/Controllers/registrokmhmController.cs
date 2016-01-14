using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RentaMaq.DAL;
using RentaMaq.Models;

namespace RentaMaq.Controllers
{
    public class registrokmhmController : Controller
    {
        private Context db = new Context();
        private int numeroPermiso = 12;

        public ActionResult Problemas() 
        {
            return View(registrokmhm.obtenerIDsProblemas());
        }

        // GET: registrokmhm
        public ActionResult Index(string inicio, string termino, string equipoID)
        {
            DateTime Inicio = DateTime.Now.AddDays(-14);
            DateTime Termino = DateTime.Now;

            String EquipoID = "todos";

            if (inicio != null || termino != null)
            {
                string[] inicioSeparado = inicio.Split('-');
                string[] terminoSeparado = termino.Split('-');

                Inicio = new DateTime(int.Parse(inicioSeparado[2]), int.Parse(inicioSeparado[1]), int.Parse(inicioSeparado[0]));
                Termino = new DateTime(int.Parse(terminoSeparado[2]), int.Parse(terminoSeparado[1]), int.Parse(terminoSeparado[0]));
            }

            DateTime fin = Termino;
            DateTime temp = fin;
            while (temp.Day == Termino.Day)
            {
                fin = temp;
                temp = temp.AddMinutes(1);
            }

            List<registrokmhm> lista = db.registrokmhms.Where(s => s.fecha >= Inicio && s.fecha <= fin).ToList();

            if (equipoID != null && !equipoID.Equals("todos"))
            {
                EquipoID = equipoID;
                int intEquipoID = int.Parse(EquipoID);
                lista = lista.Where(s => s.equipoID == intEquipoID).OrderByDescending(s=>s.fecha).ToList();
            }

            ViewBag.Inicio = Inicio;
            ViewBag.Termino = Termino;
            ViewBag.EquipoID = EquipoID;

            return View(lista);
        }

        // GET: registrokmhm/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            registrokmhm registrokmhm = db.registrokmhms.Find(id);
            if (registrokmhm == null)
            {
                return HttpNotFound();
            }
            return View(registrokmhm);
        }

        // GET: registrokmhm/Create
        public ActionResult Create()
        {
            if (Session["ID"] == null || !roles.tienePermiso(numeroPermiso, int.Parse(Session["ID"].ToString())))
            {
                return RedirectToAction("Index", "registrokmhm");
            }
            return View();
        }

        // POST: registrokmhm/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "registrokmhmID,equipoID,kilometraje,horometro,fecha")] registrokmhm registrokmhm,
            FormCollection post)
        {
            if (Session["ID"] == null || !roles.tienePermiso(numeroPermiso, int.Parse(Session["ID"].ToString())))
            {
                return RedirectToAction("Index", "registrokmhm");
            }
            registrokmhm.fecha = Formateador.fechaFormatoGuardar(post["fecha"]);
            db.registrokmhms.Add(registrokmhm);

            registro Registro = new registro();
            Registro.fecha = DateTime.Now;
            Registro.tipoAccion = "Crear";
            Registro.tipoDato = "registrokmhm";
            Registro.usuario = db.Usuarios.Find(int.Parse(Session["ID"].ToString())).nombreUsuario + " / " + db.Usuarios.Find(int.Parse(Session["ID"].ToString())).nombreCompleto;
            Registro.usuarioID = int.Parse(Session["ID"].ToString());
            Registro.descripcion = Registro.usuario + " Crea nuevo registrohmkm" + registrokmhm.registrokmhmID;
            db.Registros.Add(Registro);

            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: registrokmhm/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["ID"] == null || !roles.tienePermiso(numeroPermiso, int.Parse(Session["ID"].ToString())))
            {
                return RedirectToAction("Index", "registrokmhm");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            registrokmhm registrokmhm = db.registrokmhms.Find(id);
            if (registrokmhm == null)
            {
                return HttpNotFound();
            }
            return View(registrokmhm);
        }

        // POST: registrokmhm/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "registrokmhmID,equipoID,kilometraje,horometro,fecha")] registrokmhm registrokmhm, FormCollection post)
        {
            if (Session["ID"] == null || !roles.tienePermiso(numeroPermiso, int.Parse(Session["ID"].ToString())))
            {
                return RedirectToAction("Index", "registrokmhm");
            }
            registrokmhm.fecha = Formateador.fechaFormatoGuardar(post["fecha"]);
            db.Entry(registrokmhm).State = EntityState.Modified;

            registro Registro = new registro();
            Registro.fecha = DateTime.Now;
            Registro.tipoAccion = "Editar";
            Registro.tipoDato = "registrokmhm";
            Registro.usuario = db.Usuarios.Find(int.Parse(Session["ID"].ToString())).nombreUsuario + " / " + db.Usuarios.Find(int.Parse(Session["ID"].ToString())).nombreCompleto;
            Registro.usuarioID = int.Parse(Session["ID"].ToString());
            Registro.descripcion = Registro.usuario + " Edita registrohmkm " + registrokmhm.registrokmhmID;
            db.Registros.Add(Registro);


            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: registrokmhm/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["ID"] == null || !roles.tienePermiso(numeroPermiso, int.Parse(Session["ID"].ToString())))
            {
                return RedirectToAction("Index", "registrokmhm");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            registrokmhm registrokmhm = db.registrokmhms.Find(id);
            if (registrokmhm == null)
            {
                return HttpNotFound();
            }
            return View(registrokmhm);
        }

        // POST: registrokmhm/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["ID"] == null || !roles.tienePermiso(numeroPermiso, int.Parse(Session["ID"].ToString())))
            {
                return RedirectToAction("Index", "registrokmhm");
            }
            registrokmhm registrokmhm = db.registrokmhms.Find(id);
            db.registrokmhms.Remove(registrokmhm);

            registro Registro = new registro();
            Registro.fecha = DateTime.Now;
            Registro.tipoAccion = "Eliminar";
            Registro.tipoDato = "registrokmhm";
            Registro.usuario = db.Usuarios.Find(int.Parse(Session["ID"].ToString())).nombreUsuario + " / " + db.Usuarios.Find(int.Parse(Session["ID"].ToString())).nombreCompleto;
            Registro.usuarioID = int.Parse(Session["ID"].ToString());
            Registro.descripcion = Registro.usuario + " Elimina registrohmkm " + registrokmhm.registrokmhmID;
            db.Registros.Add(Registro);

            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
