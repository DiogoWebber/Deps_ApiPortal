using Deps_CleanArchitecture.SharedKernel.Util;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

namespace Deps_CleanArchitecture.Infrastructure.Identity
{
    public static class JwtStartupSetup
    {
        public static void RegisterJWT(IServiceCollection services)
        {
            // Carregar as variáveis de ambiente com verificação
            var jwtIssuer = AmbienteUtil.GetValue("JWT_ISSUER");
            var jwtAudience = AmbienteUtil.GetValue("JWT_AUDIENCE");
            var jwtKey = AmbienteUtil.GetValue("JWT_KEY");

            // Verificar se as variáveis são nulas ou vazias e lançar exceções com mensagem apropriada
            if (string.IsNullOrEmpty(jwtIssuer))
            {
                throw new ArgumentNullException("JWT_ISSUER", "JWT_ISSUER não foi definido nas variáveis de ambiente.");
            }

            if (string.IsNullOrEmpty(jwtAudience))
            {
                throw new ArgumentNullException("JWT_AUDIENCE", "JWT_AUDIENCE não foi definido nas variáveis de ambiente.");
            }

            if (string.IsNullOrEmpty(jwtKey))
            {
                throw new ArgumentNullException("JWT_KEY", "JWT_KEY não foi definido nas variáveis de ambiente.");
            }

            services.AddAuthentication(authOptions =>
            {
                authOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                authOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(bearerOptions =>
            {
                bearerOptions.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtIssuer,
                    ValidAudience = jwtAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                };
            });

            // Configuração de autorização com JWT
            services.AddAuthorization(auth =>
            {
                auth.AddPolicy("Bearer", new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser().Build());
            });
        }
    }
}
