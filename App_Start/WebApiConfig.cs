// Asegúrate de tener el using para la carpeta donde está tu TokenValidationHandler
using DBExamen3.Clases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace DBExamen3
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Configuración y servicios de Web API

            // --- REGISTRO DEL HANDLER ---
            // Añade una instancia de nuestro TokenValidationHandler a la
            // colección de manejadores de mensajes globales de la API.
            // Esto asegura que se ejecute para cada solicitud ANTES de que
            // llegue al controlador (si la ruta coincide).
            config.MessageHandlers.Add(new TokenValidationHandler());
            // --- FIN REGISTRO DEL HANDLER ---

            // Rutas de Web API
            // Habilita el enrutamiento basado en atributos (como [RoutePrefix], [Route], [HttpGet], etc.)
            // Esto es importante porque nuestros controladores (TorneosController y LoginController) lo usan.
            config.MapHttpAttributeRoutes();

            // Ruta por defecto (opcional si solo usas Attribute Routing, pero es bueno tenerla)
            // Define la plantilla de ruta estándar "api/{controller}/{id}"
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional } // El 'id' es opcional
            );
        }
    }
}