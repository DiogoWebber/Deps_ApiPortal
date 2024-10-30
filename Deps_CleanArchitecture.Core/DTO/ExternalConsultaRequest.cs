using System.Collections.Generic;
using Newtonsoft.Json;

namespace Deps_CleanArchitecture.Core.DTO
{
    public class ExternalConsultaRequest
    {
        [JsonProperty("documento")]
        public string Documento { get; set; }

        [JsonProperty("produto")]
        public ProdutoRequest Produto { get; set; }

        [JsonProperty("userId")]
        public string UserId { get; set; }
    }
    
    public class ProvedorRequest
    {
        [JsonProperty("idProvedores")]
        public int IdProvedores { get; set; }

        [JsonProperty("nomeProvedor")]
        public string NomeProvedor { get; set; }
    }
}