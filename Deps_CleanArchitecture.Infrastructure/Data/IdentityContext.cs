using System;
using Deps_CleanArchitecture.Core.Entities;
using Deps_CleanArchitecture.SharedKernel.Util;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Deps_CleanArchitecture.Infrastructure.Data;

public class IdentityContext : IdentityDbContext<IdentityUser> 
{
    
    public IdentityContext(DbContextOptions<IdentityContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        var adminUsername = AmbienteUtil.GetValue("DefaultAdmin:Username") ?? "Admin";
        var adminEmail = AmbienteUtil.GetValue("DefaultAdmin:Email") ?? "admin@mail.com";
        var adminPassword = AmbienteUtil.GetValue("DefaultAdmin:Password") ?? "Admin@123";
        // var adminUsername = AmbienteUtil.GetValue("ADMIN_USERNAME");
        // var adminEmail = AmbienteUtil.GetValue("ADMIN_USERNAME");
        // var adminPassword = AmbienteUtil.GetValue("ADMIN_USERNAME");
        var adminRoleId = Guid.NewGuid().ToString();
        
        builder.Entity<IdentityRole>().HasData(
            UsersRoles.GetIdentityRole(UsersRoles.Admin, adminRoleId),
            UsersRoles.GetIdentityRole(UsersRoles.Gestor, Guid.NewGuid().ToString()),
            UsersRoles.GetIdentityRole(UsersRoles.Usuario, Guid.NewGuid().ToString())
        );
        
        var hasher = new PasswordHasher<IdentityUser>();
        var adminUser = new IdentityUser
        {
            Id = Guid.NewGuid().ToString(),
            UserName = adminUsername,
            NormalizedUserName = adminUsername.ToUpper(),
            Email = adminEmail,
            NormalizedEmail = adminEmail.ToUpper(),
            EmailConfirmed = true,
            PasswordHash = hasher.HashPassword(null, adminPassword), // Set a strong password here
            SecurityStamp = Guid.NewGuid().ToString()
        };
        
        builder.Entity<IdentityUser>().HasData(adminUser);
        
        builder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
        {
            RoleId = adminRoleId,
            UserId = adminUser.Id
        });
    }
}