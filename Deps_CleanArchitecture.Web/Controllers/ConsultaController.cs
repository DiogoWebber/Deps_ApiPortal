using System;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Deps_CleanArchitecture.Core.DTO;
using Deps_CleanArchitecture.Core.Entities;
using Deps_CleanArchitecture.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Deps_CleanArchitecture.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConsultaController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IdentityContext _context;

        public ConsultaController(HttpClient httpClient, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager, IdentityContext context)
        {
            _httpClient = httpClient;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
        }

        // POST: api/consulta/Consulta
        [HttpPost("Consulta")]
        [Authorize]
        public async Task<IActionResult> ConsultaCnpjProduto([FromBody] ConsultaRequest request)
        {
            // Obtém o UserId do token JWT, geralmente com a claim sub ou NameIdentifier
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Usuário não autenticado.");
            }

            // Obtém o ClienteId do token, a claim precisa ser configurada na geração do token
            var clienteId = User.FindFirst("ClienteId")?.Value;

            if (string.IsNullOrEmpty(clienteId))
            {
                return Unauthorized("ClienteId não encontrado no token.");
            }

            // Busca o produto no banco de dados usando o idProduto
            var produto = await _context.Produtos
                .Where(p => p.IdProduto == request.idProduto && p.ClienteId == clienteId)
                .Select(p => new ProdutoRequest
                {
                    idProduto = p.IdProduto,
                    nomeProduto = p.NomeProduto,
                    Credito = p.Credito,
                    Provedores = p.ProdutoProvedores.Select(pp => new ProvedoresRequest
                    {
                        IdProvedores = pp.ProvedorId,
                        NomeProvedor = pp.Provedor.NomeProvedor
                    }).ToList(),
                    ClienteId = p.ClienteId
                })
                .FirstOrDefaultAsync();


            if (produto == null)
            {
                return NotFound("Produto não encontrado para o ClienteId especificado.");
            }

            // Cria o payload para ser enviado ou retornado
            var payload = new
            {
                Documento = request.documento,
                Produto = produto,
                UserId = userId, // Usando o UserId extraído do token
                ClienteId = clienteId // Usando o ClienteId extraído do token
            };

            // Serializa o payload para JSON, apenas para verificação
            var jsonPayload = JsonSerializer.Serialize(payload);
            Console.WriteLine(jsonPayload); // Para fins de debug, remova em produção

            return Ok(payload);
        }
    }
}
