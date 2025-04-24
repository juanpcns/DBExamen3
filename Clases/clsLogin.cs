using DBExamen3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DBExamen3.Clases
{
    public class LoginRequest
    {
        public string Usuario { get; set; }
        public string Clave { get; set; }
    }
    public class LoginResponse
    {
        public bool Autenticado { get; set; }
        public string Mensaje { get; set; }
        public string Token { get; set; }
        public string Usuario { get; set; } 
    }
    public class clsLogin
    {
        private DBExamenEntities1 db = new DBExamenEntities1();
        public LoginResponse Autenticar(LoginRequest credenciales)
        {
            LoginResponse respuesta = new LoginResponse { Autenticado = false, Token = null };

            if (string.IsNullOrEmpty(credenciales?.Usuario) || string.IsNullOrEmpty(credenciales.Clave))
            {
                respuesta.Mensaje = "El Usuario y la Clave son obligatorios.";
                return respuesta;
            }

            try
            {
                var admin = db.AdministradorITMs.FirstOrDefault(a =>
                                a.Usuario == credenciales.Usuario &&
                                a.Clave == credenciales.Clave);

                if (admin != null)
                {
                    respuesta.Autenticado = true;
                    respuesta.Mensaje = "Autenticación exitosa.";
                    respuesta.Usuario = admin.Usuario; 

                    respuesta.Token = TokenGenerator.GenerateTokenJwt(admin.Usuario);
                }
                else
                {
                    respuesta.Mensaje = "Credenciales inválidas. Verifique el usuario y la clave.";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("Error en clsLogin.Autenticar: " + ex.ToString());

                respuesta.Mensaje = "Ocurrió un error inesperado durante el proceso de autenticación.";
            }

            return respuesta;
        }
    }
}