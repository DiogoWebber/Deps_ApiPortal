using System.Collections.Generic;

namespace Deps_CleanArchitecture.Core.DTO;

public class ProdutoRequest
{
    public List<int> IdProvedores { get; set; }
    public decimal Credito { get; set; }
}