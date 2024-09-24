using System.Collections.Generic;

namespace Deps_CleanArchitecture.Core.DTO
{
    public class ProvedoresRequest
    {
        public string IdProvedores { get; set; }
        public string NomeProvedor { get; set; }
    }
}

namespace Deps_CleanArchitecture.Core.DTO
{
    public class ProdutoRequest
    {
        public string idProduto { get; set; }
        public string nomeProduto { get; set; }
        public decimal Credito { get; set; }
        public List<ProvedoresRequest> Provedores { get; set; }
        public string IdEmpresa { get; set; }

    }
}