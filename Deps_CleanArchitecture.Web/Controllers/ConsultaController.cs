using System;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Deps_CleanArchitecture.Core.DTO;
using Deps_CleanArchitecture.Core.Entities;
using Deps_CleanArchitecture.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
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

        public ConsultaController(HttpClient httpClient, UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager,
            IdentityContext context)
        {
            _httpClient = httpClient;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
        }

        [HttpPost("Consulta")]
        [Authorize]
        public async Task<IActionResult> ConsultaCnpjProduto([FromBody] ConsultaRequest request)
        {
            var userId = User.FindFirst("UserId")?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Usuário não autenticado.");
            }

            var clienteId = User.FindFirst("ClienteId")?.Value;

            if (string.IsNullOrEmpty(clienteId))
            {
                return Unauthorized("ClienteId não encontrado no token.");
            }

            // Recupera o usuário e verifica o crédito
            var usuario = await _context.Users
                .Where(u => u.Id == userId && u.ClienteId == clienteId)
                .FirstOrDefaultAsync();

            if (usuario == null)
            {
                return Unauthorized("Usuário não encontrado ou cliente não autorizado.");
            }

            // Recupera o produto para obter o custo da consulta
            var produto = await _context.Produtos
                .Where(p => p.IdProduto == request.idProduto && p.ClienteId == clienteId)
                .Include(p => p.ProdutoProvedores)
                .ThenInclude(pp => pp.Provedor)
                .FirstOrDefaultAsync();

            if (produto == null)
            {
                return NotFound("Produto não encontrado para o ClienteId especificado.");
            }

            // Verifica se o usuário possui créditos suficientes para cobrir o custo da consulta do produto
            if (usuario.Credito < produto.Credito) // Usa diretamente o campo Creditos do produto
            {
                return BadRequest("Créditos insuficientes para realizar esta busca.");
            }

            // Deduz o custo da consulta do crédito do usuário
            usuario.Credito -= produto.Credito;
            _context.Users.Update(usuario);
            await _context.SaveChangesAsync();

            // Cria o payload com as informações necessárias
            var payload = new
            {
                Documento = request.documento,
                Produto = new
                {
                    idProduto = produto.IdProduto,
                    nomeProduto = produto.NomeProduto,
                    Provedores = produto.ProdutoProvedores.Select(pp => new
                    {
                        IdProvedores = pp.ProvedorId,
                        NomeProvedor = pp.Provedor.NomeProvedor
                    }).ToList(),
                    ClienteId = produto.ClienteId
                },
                UserId = userId
            };
            return Ok(payload);
        }
    }
}
