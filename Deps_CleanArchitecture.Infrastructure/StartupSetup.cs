using Deps_CleanArchitecture.Core.Entities;
using Deps_CleanArchitecture.Infrastructure.Data;
using Deps_CleanArchitecture.Infrastructure.Identity;
using Deps_CleanArchitecture.SharedKernel.Util;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Deps_CleanArchitecture.Infrastructure
{
    public static class StartupSetup
    {
        public static void AddDbContext(this IServiceCollection services) {
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(AmbienteUtil.GetValue("POSTGRES_CONNECTION")));
            services.AddDbContext<IdentityContext>(options =>
                options.UseNpgsql(AmbienteUtil.GetValue("POSTGRES_CONNECTION")));
            
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<IdentityContext>()
                .AddDefaultTokenProviders();
        }

        public static void ConfigureJwt(this IServiceCollection services) => JwtStartupSetup.RegisterJWT(services);
    }
}
