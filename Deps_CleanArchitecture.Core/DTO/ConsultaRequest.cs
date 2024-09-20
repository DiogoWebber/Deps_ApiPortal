using System.Collections.Generic;

namespace Deps_CleanArchitecture.Core.DTO
{
    public class ConsultaRequest
    {
        public string CNPJ { get; set; }
        public string Produto { get; set; }
        public List<int> IdProvedores { get; set; }
        
        public string UsuarioId { get; set; } 
    }
}