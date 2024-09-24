namespace Deps_CleanArchitecture.Core.Entities;

public class ProdutoProvedor
{
    public string ProdutoId { get; set; } // Chave estrangeira para Produto
    public Produto Produto { get; set; }

    public string ProvedorId { get; set; } // Chave estrangeira para Provedor
    public Provedores Provedor { get; set; }
}