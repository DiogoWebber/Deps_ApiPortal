using System.Threading.Tasks;
using Deps_CleanArchitecture.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Deps_CleanArchitecture.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CreditoController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public CreditoController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpPost("add-credito")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> AddCredito(string username, decimal amount)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return NotFound("Usuário não encontrado.");
            }

            user.Credito += amount;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return Ok($"Crédito de {amount} adicionado ao usuário '{username}'.");
            }

            return BadRequest("Erro ao adicionar crédito.");
        }
    }
}