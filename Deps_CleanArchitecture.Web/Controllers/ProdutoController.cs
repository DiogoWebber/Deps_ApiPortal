using Deps_CleanArchitecture.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Deps_CleanArchitecture.Core.DTO;

namespace Deps_CleanArchitecture.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProdutoController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        [HttpPost("CriarProduto")]
        [Authorize(Roles = UsersRoles.Admin)]
        public async Task<IActionResult> CriarProduto([FromBody] ProdutoRequest produtoRequest)
        {
            // Construir o objeto Produto
            var payload = new
            {
                idProvedores = produtoRequest.IdProvedores,
                Preco = produtoRequest.Credito
            };

            // Serializar o objeto em JSON
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            // Enviar a requisição POST para criar o produto na API externa
            var response = await _httpClient.PostAsync("https://external-api.com/api/produtos", content);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                return Ok(responseBody); // Produto criado com sucesso
            }

            return BadRequest("Erro ao criar o produto");
        }
    }
}
