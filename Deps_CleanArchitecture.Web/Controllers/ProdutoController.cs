using System;
using System.Collections.Generic;
using System.Linq;
using Deps_CleanArchitecture.Core.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using Deps_CleanArchitecture.Core.Entities;
using Deps_CleanArchitecture.Infrastructure.Data;

namespace Deps_CleanArchitecture.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProdutoController : ControllerBase
    {
        private readonly IdentityContext _context;

        public ProdutoController(HttpClient httpClient, IdentityContext context)
        {
            _context = context;
        }

        [HttpPost("CriarProduto")]
        [Authorize(Roles = UsersRoles.Admin)]
        public async Task<IActionResult> CriarProduto([FromBody] ProdutoRequest produtoRequest)
        {
            if (produtoRequest == null)
            {
                return BadRequest("Produto não pode ser nulo.");
            }

            // Validação adicional se necessário
            if (string.IsNullOrEmpty(produtoRequest.nomeProduto) || produtoRequest.Credito <= 0)
            {
                return BadRequest("Nome do produto e crédito devem ser informados e válidos.");
            }

            var produto = new Produto
            {
                IdProduto = Guid.NewGuid().ToString(),
                NomeProduto = produtoRequest.nomeProduto,
                Credito = produtoRequest.Credito,
                ClienteId = produtoRequest.ClienteId,
                ProdutoProvedores = produtoRequest.Provedores.Select(p => new ProdutoProvedor
                {
                    ProvedorId = p.IdProvedores,
                }).ToList()
            };

            try
            {
                _context.Produtos.Add(produto);
                await _context.SaveChangesAsync();

                var produtoResponse = new
                {
                    IdProduto = produto.IdProduto,
                    NomeProduto = produto.NomeProduto,
                    Credito = produto.Credito,
                    Provedores = produto.ProdutoProvedores.Select(pp => new
                    {
                        ProvedorId = pp.ProvedorId,
                        NomeProvedor = produtoRequest.Provedores.FirstOrDefault(p => p.IdProvedores == pp.ProvedorId)?.NomeProvedor
                    }).ToList(),
                    ClienteId = produto.ClienteId
                };

                return CreatedAtAction(nameof(CriarProduto), new { id = produto.IdProduto }, produtoResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao criar produto: {ex.Message}");
            }
        }
    }
}