using RentaMaq.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace RentaMaq.Models
{
    public class registrokmhmReport {
        public string equipoID { get; set; }
        public int kilometraje { get; set; }
        public int horometro { get; set; }
        public string fecha { get; set; }

        public static List<registrokmhmReport> convertirDatos(List<registrokmhm> Datos) 
        {
            Context db = new Context();
            List<registrokmhmReport> Retorno = new List<registrokmhmReport>();

            foreach (registrokmhm Dato in Datos)
            {
                registrokmhmReport nuevo = new registrokmhmReport();

                nuevo.equipoID = db.Equipos.Find(Dato.equipoID).numeroAFI;
                nuevo.fecha = formatearString.fechaSinHoraDiaPrimero(Dato.fecha);
                nuevo.horometro = Dato.horometro;
                nuevo.kilometraje = Dato.kilometraje;

                Retorno.Add(nuevo);
            }

            return Retorno;
        }
    }

    public class registrokmhm
    {
        public int registrokmhmID { get; set; }
        [Display(Name = "Equipo")]
        public int equipoID { get; set; }
        [Display(Name = "Kilometraje")]
        public int kilometraje { get; set; }
        [Display(Name = "Horómetro")]
        public int horometro { get; set; }
        [Display(Name = "Fecha")]
        public DateTime fecha { get; set; }

        public static registrokmhm obtenerUltimo(int ID)
        {
            registrokmhm retorno = new registrokmhm();
            SqlConnection con = conexion.crearConexion();
            con.Open();

            using (SqlCommand command = new SqlCommand("SELECT TOP 1 registrokmhmID FROM registrokmhm "
                + "WHERE equipoID=@equipoID ORDER BY fecha DESC", con))
            {
                command.Parameters.Add("@equipoID", SqlDbType.Int).Value = ID;
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        retorno = new Context().registrokmhms.Find(int.Parse(reader[0].ToString()));
                    }
                }
            }

            con.Close();
            return retorno;
        }

        public static void actualizarRegistroKmHm(int equipoID, DateTime fecha, int horometro, int kilometraje)
        {
            Context db = new Context();
            db.registrokmhms.RemoveRange(db.registrokmhms.Where(s=>s.equipoID==equipoID && s.fecha==fecha));

            db.SaveChanges();

            registrokmhm nuevo = new registrokmhm();
            nuevo.equipoID = equipoID;
            nuevo.fecha=fecha;
            nuevo.horometro = horometro;
            nuevo.kilometraje = kilometraje;

            db.registrokmhms.Add(nuevo);
            db.SaveChanges();
        }

        public static void actualizarRegistroKmHm(registrokmhm nuevo)
        {
            actualizarRegistroKmHm(nuevo.equipoID, nuevo.fecha, nuevo.horometro, nuevo.kilometraje);
        }

        internal static registrokmhm obtenerUltimo(int equipoID, DateTime fin)
        {
            registrokmhm retorno = new registrokmhm();
            SqlConnection con = conexion.crearConexion();
            con.Open();

            using (SqlCommand command = new SqlCommand("SELECT TOP 1 registrokmhmID FROM registrokmhm "
                + "WHERE equipoID=@equipoID AND fecha<=@fecha ORDER BY fecha DESC", con))
            {
                command.Parameters.Add("@equipoID", SqlDbType.Int).Value = equipoID;
                command.Parameters.Add("@fecha", SqlDbType.DateTime).Value = fin;
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        retorno = new Context().registrokmhms.Find(int.Parse(reader[0].ToString()));
                    }
                }
            }

            con.Close();
            return retorno;
        }

        internal static registrokmhm obtenerEstimado(int equipoID, DateTime fecha)
        {
            registrokmhm anterior = obtenerUltimo(equipoID, fecha);
            registrokmhm siguiente = obtenerSiguiente(equipoID, fecha);
            registrokmhm masAnterior = new registrokmhm();
            registrokmhm masSiguiente = new registrokmhm();

            registrokmhm resultado = new registrokmhm();
            resultado.equipoID = equipoID;
            resultado.fecha = fecha;

            if (siguiente.fecha.Equals(new registrokmhm().fecha))
            {
                masAnterior = obtenerUltimo(equipoID, fecha.AddMonths(-3));

                TimeSpan diferencia = anterior.fecha - masAnterior.fecha;
                int diferenciaHorometro = anterior.horometro - masAnterior.horometro;
                int diferenciaKilometraje = anterior.kilometraje - masAnterior.kilometraje;

                resultado.horometro = (int)(masAnterior.horometro + (resultado.fecha - masAnterior.fecha).TotalDays
                    * diferenciaHorometro / diferencia.TotalDays);

                resultado.horometro = (int)(masAnterior.horometro + (resultado.fecha - masAnterior.fecha).TotalDays
                    * diferenciaHorometro / diferencia.TotalDays);

                resultado.kilometraje = (int)(masAnterior.kilometraje + (resultado.fecha - masAnterior.fecha).TotalDays
                    * diferenciaKilometraje / diferencia.TotalDays);
            }
            else if (anterior.fecha.Equals(new registrokmhm().fecha))
            {
                masSiguiente = obtenerUltimo(equipoID, fecha.AddMonths(3));

                TimeSpan diferencia = masSiguiente.fecha - siguiente.fecha;
                int diferenciaHorometro = masSiguiente.horometro - siguiente.horometro;
                int diferenciaKilometraje = masSiguiente.kilometraje - siguiente.kilometraje;

                resultado.horometro = (int)(siguiente.horometro - (siguiente.fecha - resultado.fecha).TotalDays
                    * diferenciaHorometro / diferencia.TotalDays);

                resultado.kilometraje = (int)(siguiente.kilometraje + (siguiente.fecha - resultado.fecha).TotalDays
                    * diferenciaKilometraje / diferencia.TotalDays);
            }
            else
            {
                TimeSpan diferencia = siguiente.fecha - anterior.fecha;
                int diferenciaHorometro = siguiente.horometro - anterior.horometro;
                int diferenciaKilometraje = siguiente.kilometraje - anterior.kilometraje;

                resultado.horometro = (int)(anterior.horometro + (resultado.fecha - anterior.fecha).TotalDays
                    * diferenciaHorometro / diferencia.TotalDays);

                resultado.kilometraje = (int)(anterior.kilometraje + (resultado.fecha - anterior.fecha).TotalDays
                    * diferenciaKilometraje / diferencia.TotalDays);
            }
            return resultado;
        }

        private static registrokmhm obtenerSiguiente(int equipoID, DateTime fin)
        {
            registrokmhm retorno = new registrokmhm();
            SqlConnection con = conexion.crearConexion();
            con.Open();

            using (SqlCommand command = new SqlCommand("SELECT TOP 1 registrokmhmID FROM registrokmhm "
                + "WHERE equipoID=@equipoID AND fecha>@fecha ORDER BY fecha ASC", con))
            {
                command.Parameters.Add("@equipoID", SqlDbType.Int).Value = equipoID;
                command.Parameters.Add("@fecha", SqlDbType.DateTime).Value = fin;
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        retorno = new Context().registrokmhms.Find(int.Parse(reader[0].ToString()));
                    }
                }
            }

            con.Close();
            return retorno;
        }

        public static int obtenerCantidadIDsProblemas() 
        {
            int retorno = 0;
            string consulta = "SELECT COUNT(DISTINCT(a.equipoID)) "
                  + "FROM [dbo].[registrokmhm] as a, [dbo].[registrokmhm] as b"
                  + " WHERE a.fecha<b.fecha AND a.equipoID=b.equipoID AND "
                  + " (a.horometro>b.horometro OR a.kilometraje>b.kilometraje)";
            SqlConnection con = conexion.crearConexion();
            con.Open();

            using (SqlCommand command = new SqlCommand(consulta, con))
            {
                retorno = (Int32)command.ExecuteScalar();
            }

            con.Close();
            return retorno;
        }

        public static List<int> obtenerIDsProblemas()
        {
            List<int> retorno = new List<int>();
            string consulta = "SELECT DISTINCT(a.equipoID) "
                  + "FROM [dbo].[registrokmhm] as a, [dbo].[registrokmhm] as b"
                  + " WHERE a.fecha<b.fecha AND a.equipoID=b.equipoID AND "
                  + " (a.horometro>b.horometro OR a.kilometraje>b.kilometraje)";
            SqlConnection con = conexion.crearConexion();
            con.Open();

            using (SqlCommand command = new SqlCommand(consulta, con))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        retorno.Add(int.Parse(reader[0].ToString()));
                    }
                }
            }

            con.Close();
            return retorno;
        }
    }
}