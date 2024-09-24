using System.Collections.Generic;

namespace Deps_CleanArchitecture.Core.DTO
{
    public class ConsultaRequest
    {
        public string documento { get; set; }
        public string idProduto { get; set; }
        public string IdUsuario { get; set; } 
        public string IdEmpresa { get; set; }  
        
    }
}