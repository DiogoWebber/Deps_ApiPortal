using System.Collections.Generic;
using Deps_CleanArchitecture.Core.Entities;

public class Provedores
{
    public string IdProvedores { get; set; } // Chave primária
    public string NomeProvedor { get; set; }
    
    public ICollection<ProdutoProvedor> ProdutoProvedores { get; set; } // Tabela de junção
}