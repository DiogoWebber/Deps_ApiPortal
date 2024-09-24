using System;
using Deps_CleanArchitecture.Core.DTO;
using Deps_CleanArchitecture.Core.Entities;
using Deps_CleanArchitecture.SharedKernel.Util;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Deps_CleanArchitecture.Infrastructure.Data;

public class IdentityContext : IdentityDbContext<ApplicationUser>
{
    public IdentityContext(DbContextOptions<IdentityContext> options)
        : base(options)
    {
    }

    public DbSet<Produto> Produtos { get; set; } 
    public DbSet<Provedores> Provedores { get; set; } 
    public DbSet<ProdutoProvedor> ProdutoProvedor { get; set; } 


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        // Configuração do usuário admin
        var adminUsername = AmbienteUtil.GetValue("DefaultAdmin:Username") ?? "Admin";
        var adminEmail = AmbienteUtil.GetValue("DefaultAdmin:Email") ?? "admin@mail.com";
        var adminPassword = AmbienteUtil.GetValue("DefaultAdmin:Password") ?? "Admin@123";
        var IdEmpresa = AmbienteUtil.GetValue("DefaultAdmin:IdEmpresa") ?? "19500";

        var adminRoleId = Guid.NewGuid().ToString();

        builder.Entity<IdentityRole>().HasData(
            UsersRoles.GetIdentityRole(UsersRoles.Admin, adminRoleId),
            UsersRoles.GetIdentityRole(UsersRoles.Gestor, Guid.NewGuid().ToString()),
            UsersRoles.GetIdentityRole(UsersRoles.Usuario, Guid.NewGuid().ToString())
        );

        var hasher = new PasswordHasher<ApplicationUser>();
        var adminUser = new ApplicationUser
        {
            Id = Guid.NewGuid().ToString(),
            UserName = adminUsername,
            NormalizedUserName = adminUsername.ToUpper(),
            Email = adminEmail,
            NormalizedEmail = adminEmail.ToUpper(),
            EmailConfirmed = true,
            PasswordHash = hasher.HashPassword(null, adminPassword),
            SecurityStamp = Guid.NewGuid().ToString(),
            Credito = 0,
            IdEmpresa = IdEmpresa.ToUpper()
        };

        builder.Entity<ApplicationUser>().HasData(adminUser);

        builder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
        {
            RoleId = adminRoleId,
            UserId = adminUser.Id
        });
        
        builder.Entity<Produto>()
            .HasKey(p => p.IdProduto); 
        
        builder.Entity<Provedores>()
            .HasKey(p => p.IdProvedores); // Define a chave primária

        // Configuração do relacionamento muitos para muitos
        builder.Entity<ProdutoProvedor>()
            .HasKey(pp => new { pp.ProdutoId, pp.ProvedorId });

        builder.Entity<ProdutoProvedor>()
            .HasOne(pp => pp.Produto)
            .WithMany(p => p.ProdutoProvedores)
            .HasForeignKey(pp => pp.ProdutoId);

        builder.Entity<ProdutoProvedor>()
            .HasOne(pp => pp.Provedor)
            .WithMany(p => p.ProdutoProvedores)
            .HasForeignKey(pp => pp.ProvedorId);
        builder.Entity<Provedores>().HasData(
            new Provedores
            {
                IdProvedores = "98b68a84-905e-4f26-80d5-13eb9e843d44",
                NomeProvedor = "Deps"
            }
        );
        builder.Entity<Provedores>().HasData(
            new Provedores
            {
                IdProvedores = "c7765432-4386-4b87-80e7-b535f3540781", 
                NomeProvedor = "DadosPublicos"
            }
        );
    }
}
