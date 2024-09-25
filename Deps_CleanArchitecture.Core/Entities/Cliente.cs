using System.Collections.Generic;

namespace Deps_CleanArchitecture.Core.Entities;

public class Cliente
{
    public string Id { get; set; } // Chave primária
    public string Nome { get; set; }
    public string Email { get; set; }
    public string Telefone { get; set; }
    public ICollection<ApplicationUser> Produtos { get; set; }
}
