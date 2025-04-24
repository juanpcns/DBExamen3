using DBExamen3.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;

namespace DBExamen3.Clases
{
    public class clsTorneo
    {
        private DBExamenEntities1 db = new DBExamenEntities1();

        public string Insertar(Torneo torneoAInsertar, int idAdministrador)
        {
            try
            {
                torneoAInsertar.idAdministradorITM = idAdministrador;

                db.Torneos.Add(torneoAInsertar);
                db.SaveChanges();
                return "Torneo '" + torneoAInsertar.NombreTorneo + "' insertado correctamente.";
            }
            catch (Exception ex)
            {
                return "Error al insertar el torneo: " + ex.Message;
            }
        }

        public string Actualizar(Torneo torneoAActualizar)
        {
            try
            {
                var torneoExistente = db.Torneos.Find(torneoAActualizar.idTorneos);
                if (torneoExistente == null)
                {
                    return "Error: No se encontró el torneo con ID " + torneoAActualizar.idTorneos;
                }

                db.Entry(torneoExistente).CurrentValues.SetValues(torneoAActualizar);
                db.Entry(torneoExistente).State = EntityState.Modified;

                db.SaveChanges();
                return "Torneo '" + torneoAActualizar.NombreTorneo + "' actualizado correctamente.";
            }
            catch (Exception ex)
            {
                return "Error al actualizar el torneo: " + ex.Message;
            }
        }

        public string Eliminar(int id)
        {
            try
            {
                Torneo torneoAEliminar = db.Torneos.Find(id);
                if (torneoAEliminar == null)
                {
                    return "Error: No se encontró el torneo con ID " + id;
                }

                db.Torneos.Remove(torneoAEliminar);
                db.SaveChanges();
                return "Torneo con ID " + id + " eliminado correctamente.";
            }
            catch (Exception ex)
            {
                return "Error al eliminar el torneo: " + ex.Message;
            }
        }
        public int? ObtenerIdAdminPorUsuario(string username)
        {
            try
            {
                var admin = db.AdministradorITMs.FirstOrDefault(a => a.Usuario == username);
                return admin?.idAministradorITM;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener ID de admin para usuario {username}: " + ex.Message);
                return null;
            }
        }

        public object Consultar(int id)
        {
            try
            {
                return db.Torneos
                         .Where(t => t.idTorneos == id)
                         .Select(t => new
                         {
                             IdTorneo = t.idTorneos,
                             TipoTorneo = t.TipoTorneo,
                             NombreTorneo = t.NombreTorneo,
                             NombreEquipo = t.NombreEquipo,
                             ValorInscripcion = t.ValorInscripcion,
                             FechaTorneo = t.FechaTorneo,
                             Integrantes = t.Integrantes
                         })
                         .FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al consultar torneo con ID {id}: " + ex.Message);
                return null;
            }
        }


        public List<object> ConsultarTodos()
        {
            try
            {
                return db.Torneos
                         .OrderByDescending(t => t.FechaTorneo)
                         .Select(t => new
                         {
                             IdTorneo = t.idTorneos,
                             TipoTorneo = t.TipoTorneo,
                             NombreTorneo = t.NombreTorneo,
                             NombreEquipo = t.NombreEquipo,
                             ValorInscripcion = t.ValorInscripcion,
                             FechaTorneo = t.FechaTorneo,
                             Integrantes = t.Integrantes
                         })
                         .ToList<object>();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al consultar todos los torneos: " + ex.Message);
                return new List<object>();
            }
        }


        public List<object> ConsultarFiltrado(string tipoTorneo, string nombreTorneo, DateTime? fechaTorneo)
        {
            try
            {
                IQueryable<Torneo> query = db.Torneos;

                if (!string.IsNullOrEmpty(tipoTorneo))
                {
                    query = query.Where(t => t.TipoTorneo == tipoTorneo);
                }
                if (!string.IsNullOrEmpty(nombreTorneo))
                {
                    query = query.Where(t => t.NombreTorneo == nombreTorneo);
                }
                if (fechaTorneo.HasValue)
                {
                    DateTime fechaSinHora = fechaTorneo.Value.Date;
                    query = query.Where(t => DbFunctions.TruncateTime(t.FechaTorneo) == fechaSinHora);
                }

                return query.OrderByDescending(t => t.FechaTorneo)
                         .Select(t => new
                         {
                             IdTorneo = t.idTorneos,
                             TipoTorneo = t.TipoTorneo,
                             NombreTorneo = t.NombreTorneo,
                             NombreEquipo = t.NombreEquipo,
                             ValorInscripcion = t.ValorInscripcion,
                             FechaTorneo = t.FechaTorneo,
                             Integrantes = t.Integrantes
                         })
                         .ToList<object>();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al consultar torneos con filtro: " + ex.Message);
                return new List<object>();
            }
        }
    }
}