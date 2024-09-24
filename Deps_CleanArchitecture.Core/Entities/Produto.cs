using System.Collections.Generic;

namespace Deps_CleanArchitecture.Core.Entities
{
    public class Produto
    {
        public string IdProduto { get; set; } 
        public string NomeProduto { get; set; }
        public decimal Credito { get; set; }
        public string IdEmpresa { get; set; }
        
        public ICollection<ProdutoProvedor> ProdutoProvedores { get; set; } // Tabela de junção
    }
}