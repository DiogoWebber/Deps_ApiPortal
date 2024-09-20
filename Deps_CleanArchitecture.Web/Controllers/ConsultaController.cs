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
    public class ConsultaController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public ConsultaController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // POST: api/consulta/Consulta
        [HttpPost("Consulta")]
        [Authorize(Roles = UsersRoles.Admin)] // Requer autenticação
        public async Task<IActionResult> ConsultaCnpjProduto([FromBody] ConsultaRequest request)
        {
            // Captura o ID do usuário autenticado
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Usuário não autenticado.");
            }

            // Construir o objeto a ser enviado para a outra API
            var payload = new
            {
                CNPJ = request.CNPJ,
                Produto = request.Produto,
                IdProvedores = request.IdProvedores,
                UsuarioId = userId // Incluir o ID do usuário na consulta
            };

            // Serializar o objeto em JSON
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            // Enviar a requisição POST para outra API
            var response = await _httpClient.PostAsync("https://external-api.com/api/consulta", content);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                return Ok(responseBody); // Retornar a resposta da API externa
            }

            return BadRequest("Erro ao enviar dados para a API externa");
        }
    }
}
