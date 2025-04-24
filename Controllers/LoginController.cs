using DBExamen3.Clases; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace DBExamen3.Controllers
{
    [RoutePrefix("api/login")]
    public class LoginController : ApiController
    {
        [HttpPost]
        [Route("Ingresar")]
        [AllowAnonymous]
        [ResponseType(typeof(LoginResponse))]
        public IHttpActionResult Ingresar([FromBody] LoginRequest credenciales)
        {
            if (credenciales == null || !ModelState.IsValid)
            {
                return BadRequest("Los datos proporcionados para el login son inválidos.");
            }

            try
            {
                clsLogin _login = new clsLogin();
                LoginResponse resultado = _login.Autenticar(credenciales);
                if (resultado.Autenticado)
                {
                    return Ok(resultado);
                }
                else
                {
                    return Content(HttpStatusCode.Unauthorized, resultado);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("Error en LoginController.Ingresar: " + ex.ToString());
                return InternalServerError(new Exception("Ocurrió un error inesperado durante el proceso de login. Por favor, intente más tarde."));
            }
        }
    }
}