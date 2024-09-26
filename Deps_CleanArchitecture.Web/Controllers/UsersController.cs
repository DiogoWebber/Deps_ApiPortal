using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Deps_CleanArchitecture.Core.Entities;
using Deps_CleanArchitecture.Infrastructure.Data;
using Deps_CleanArchitecture.SharedKernel.Util;
using Deps_CleanArchitecture.Web.Base;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace Deps_CleanArchitecture.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IdentityContext _context; // Your DbContext

    public UsersController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager, 
        IdentityContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _context = context;
    }
    [HttpPost("register")]
    [Authorize(Roles = UsersRoles.Admin)]
    public async Task<IActionResult> Register([FromBody] RegisterGestorRequest request)
    {
        var cliente = await _context.Clientes.FindAsync(request.ClienteId);

        if (cliente == null)
        {
            return NotFound("Cliente not found.");
        }

        var user = new ApplicationUser
        {
            UserName = request.Username,
            ClienteId = cliente.Id,
            Email = $"{request.Username}@example.com",
            EmailConfirmed = false,
            SecurityStamp = Guid.NewGuid().ToString()
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, UsersRoles.Gestor);
            return Ok("Usuário registrado com sucesso com a role de 'Gestor'.");
        }

        return BadRequest(result.Errors);
    }

    [HttpPost("registerADM")]
    [Authorize(Roles = UsersRoles.Admin)]
    public async Task<IActionResult> RegisterADM([FromBody] RegisterADMRequest request)
    {
        var user = new ApplicationUser { UserName = request.Username };
        var result = await _userManager.CreateAsync(user, request.Password);

        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, UsersRoles.Admin);
            return Ok("Usuário registrado com sucesso com a role de 'Admin'.");
        }

        return BadRequest(result.Errors);
    }

    [HttpPost("registerUsuario")]
    [Authorize(Roles = $"{UsersRoles.Gestor},{UsersRoles.Admin}")]
    public async Task<IActionResult> RegisterUsuario([FromBody] RegisterUsuarioRequest request)
    {
        var roleExists = await _roleManager.RoleExistsAsync(request.Role);
        if (!roleExists)
        {
            return BadRequest($"A role '{request.Role}' não existe.");
        }

        var currentUser = HttpContext.User;

        if (currentUser.IsInRole(UsersRoles.Gestor) && request.Role.Equals(UsersRoles.Admin, StringComparison.OrdinalIgnoreCase))
        {
            return Forbid("Usuários com a role de 'Gestor' não podem criar usuários com a role de 'Administrador'.");
        }

        var clienteIdClaim = currentUser.FindFirst("ClienteId")?.Value;
        if (string.IsNullOrEmpty(clienteIdClaim))
        {
            return BadRequest("Não foi possível encontrar o ClienteId no token do usuário.");
        }

        var user = new ApplicationUser
        {
            UserName = request.Username,
            ClienteId = clienteIdClaim
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, request.Role);
            return Ok($"Usuário registrado com sucesso com a role de '{request.Role}'.");
        }

        return BadRequest(result.Errors);
    }

    [HttpPost("update-role")]
    [Authorize(Roles = $"{UsersRoles.Gestor},{UsersRoles.Admin}")]
    public async Task<IActionResult> UpdateUserRole([FromBody] UpdateUserRoleRequest request)
    {
        var currentUser = await _userManager.GetUserAsync(User);

        var roleExist = await _roleManager.RoleExistsAsync(request.NewRole);
        if (!roleExist)
        {
            return BadRequest("A role especificada não existe.");
        }

        if (User.IsInRole(UsersRoles.Gestor) && request.NewRole.Equals(UsersRoles.Admin, StringComparison.OrdinalIgnoreCase))
        {
            return StatusCode(403, "Usuários com a role de 'Gestor' não podem atribuir a role de 'Administrador'.");
        }

        var user = await _userManager.FindByNameAsync(request.Username);
        if (user == null)
        {
            return NotFound("Usuário não encontrado.");
        }

        var userRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, userRoles);

        await _userManager.AddToRoleAsync(user, request.NewRole);

        return Ok($"Role do usuário '{request.Username}' foi atualizada para '{request.NewRole}'.");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _signInManager.PasswordSignInAsync(request.Username, request.Password, isPersistent: false, lockoutOnFailure: false);

        if (result.Succeeded)
        {
            var user = await _userManager.FindByNameAsync(request.Username);
            var token = GenerateJwtToken(user);
            return Ok(new { Token = token });
        }

        return Unauthorized("Tentativa de login inválida");
    }
    private async Task<string> GenerateJwtToken(ApplicationUser user)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
            new Claim("UserId", user.Id), 
            new Claim("ClienteId", user.ClienteId)
        };

        // Adicionando os roles do usuário como claims
        var roles = await _userManager.GetRolesAsync(user);
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        // Gerando chave simétrica e credenciais de assinatura
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AmbienteUtil.GetValue("JWT_KEY")));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Configurando o token JWT
        var token = new JwtSecurityToken(
            issuer: AmbienteUtil.GetValue("JWT_ISSUER"),
            audience: AmbienteUtil.GetValue("JWT_AUDIENCE"),
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: creds
        );

        // Retorna o token JWT gerado
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}