using System;
using Deps_CleanArchitecture.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Deps_CleanArchitecture.Core.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Collections.Generic;

namespace Deps_CleanArchitecture.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConsultaController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        
        public ConsultaController(HttpClient httpClient, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _httpClient = httpClient;
        }

        // POST: api/consulta/Consulta
        [HttpPost("Consulta")]
        [Authorize(Roles = UsersRoles.Admin)] // Requer autenticação
        public async Task<IActionResult> ConsultaCnpjProduto([FromBody] ConsultaRequest request)
        {
            var userId = _userManager.GetUserId(User); 
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Usuário não autenticado.");
            }
    
            var usuarioId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            var idEmpresa = User.FindFirst("IdEmpresa")?.Value;

            if (string.IsNullOrEmpty(usuarioId) || string.IsNullOrEmpty(idEmpresa))
            {
                return Unauthorized("IdEmpresa ou UsuarioId não encontrado no token.");
            }
            
            var produtoRequest = new ProdutoRequest
            {
                idProduto = request.idProduto,
                nomeProduto = "Nome do Produto",
                Credito = 100.00m, 
                Provedores = new List<ProvedoresRequest>
                {
                    new ProvedoresRequest
                    {
                        IdProvedores = "19500", 
                        NomeProvedor = "Provedor Exemplo" 
                    }
                },
                IdEmpresa = idEmpresa 
            };
            
            var payload = new
            {
                Documento = request.documento, 
                Produto = produtoRequest,  
                IdUsuario = usuarioId, // Agora pegando do token
                IdEmpresa = idEmpresa,  // Agora pegando do token
            };

            var jsonPayload = JsonSerializer.Serialize(payload);
            Console.WriteLine(jsonPayload); 

            return Ok(payload);
        }
    }
}
