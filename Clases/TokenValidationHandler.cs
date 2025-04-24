using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace DBExamen3.Clases
{
    public class TokenValidationHandler : DelegatingHandler
    {
        private static bool TryRetrieveToken(HttpRequestMessage request, out string token)
        {
            token = null;
            IEnumerable<string> authzHeaders;
            if (!request.Headers.TryGetValues("Authorization", out authzHeaders) || authzHeaders.Count() > 1)
            {
                return false;
            }
            var bearerToken = authzHeaders.ElementAt(0);
            if (bearerToken.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                token = bearerToken.Substring("Bearer ".Length).Trim();
                return !string.IsNullOrEmpty(token);
            }
            return false;
        }
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpStatusCode statusCode;
            string token;

            if (!TryRetrieveToken(request, out token))
            {
                return base.SendAsync(request, cancellationToken);
            }
            try 
            {
                
                var secretKey = ConfigurationManager.AppSettings["JWT_SECRET_KEY"];
                var audienceToken = ConfigurationManager.AppSettings["JWT_AUDIENCE_TOKEN"];
                var issuerToken = ConfigurationManager.AppSettings["JWT_ISSUER_TOKEN"];
                var securityKey = new SymmetricSecurityKey(System.Text.Encoding.Default.GetBytes(secretKey));

                TokenValidationParameters validationParameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true, 
                    ValidateLifetime = true,      
                    ClockSkew = TimeSpan.Zero,   

                    ValidAudience = audienceToken,
                    ValidIssuer = issuerToken,     
                    IssuerSigningKey = securityKey
                };

                SecurityToken validatedToken;
                var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                ClaimsPrincipal principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);

                Thread.CurrentPrincipal = principal;
                if (HttpContext.Current != null)
                {
                    HttpContext.Current.User = principal;
                }
            }
            catch (SecurityTokenValidationException stvex)
            {
                statusCode = HttpStatusCode.Unauthorized;
                return Task.FromResult(request.CreateResponse(statusCode, "Token inválido: " + stvex.Message));
            }
            catch (Exception ex)
            {
                statusCode = HttpStatusCode.InternalServerError; 
                return Task.FromResult(request.CreateResponse(statusCode, "Error interno al validar el token: " + ex.Message));
            }
            return base.SendAsync(request, cancellationToken);
        }
    }
}