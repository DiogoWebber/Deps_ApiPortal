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

        [HttpPost("registerGestor")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Register(string username, string password, string clienteId)
        {
            // Fetch the Cliente entity based on the provided ClienteId
            var cliente = await _context.Clientes.FindAsync(clienteId);

            // Check if the Cliente exists
            if (cliente == null)
            {
                return NotFound("Cliente not found.");
            }

            // Create the ApplicationUser, associating it with the found Cliente
            var user = new ApplicationUser
            {
                UserName = username,
                ClienteId = cliente.Id, // Associating the user with the Cliente
                Email = $"{username}@example.com", // Example email, adjust as needed
                EmailConfirmed = false,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            // Create the user with the specified password
            var result = await _userManager.CreateAsync(user, password);

            // Check if the user was created successfully
            if (result.Succeeded)
            {
                // Add the user to the 'Gestor' role
                await _userManager.AddToRoleAsync(user, UsersRoles.Gestor);
                return Ok("Usuário registrado com sucesso com a role de 'Gestor'.");
            }

            // Return any errors encountered during user creation
            return BadRequest(result.Errors);
        }


        
        [HttpPost("registerADM")]
        [Authorize(Roles = UsersRoles.Admin)]
        public async Task<IActionResult> RegisterADM(string username, string password)
        {
            var user = new ApplicationUser { UserName = username };
            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, UsersRoles.Admin);
                return Ok("Usuário registrado com sucesso com a role de 'Admin'.");
            }

            return BadRequest(result.Errors);
        }
        
        [HttpPost("registerUsuario")]
        [Authorize(Roles = "Gestor, Administrador")]
        public async Task<IActionResult> RegisterUsuario(string username, string password, string role)
        {
            var roleExists = await _roleManager.RoleExistsAsync(role);
            if (!roleExists)
            {
                return BadRequest($"A role '{role}' não existe.");
            }

            var currentUser = HttpContext.User; 
            if (currentUser.IsInRole(UsersRoles.Gestor) && role.Equals(UsersRoles.Admin, StringComparison.OrdinalIgnoreCase))
            {
                return Forbid("Usuários com a role de 'Gestor' não podem criar usuários com a role de 'Administrador'.");
            }

            var user = new ApplicationUser { UserName = username };
            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, role);
                return Ok($"Usuário registrado com sucesso com a role de '{role}'.");
            }

            return BadRequest(result.Errors);
        }

        [HttpPost("update-role")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> UpdateUserRole(string username, string newRole)
        {
            // Verifica se o usuário autenticado é um Admin
            var currentUser = await _userManager.GetUserAsync(User);
            
            // Verifica se o novo role existe
            var roleExist = await _roleManager.RoleExistsAsync(newRole);
            if (!roleExist)
            {
                return BadRequest("A role especificada não existe.");
            }

            // Busca o usuário pelo username
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return NotFound("Usuário não encontrado.");
            }

            // Remove todas as roles do usuário
            var userRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, userRoles);

            // Atribui a nova role
            await _userManager.AddToRoleAsync(user, newRole);

            return Ok($"Role do usuário '{username}' foi atualizada para '{newRole}'.");
        }

        

        [HttpPost("login")]
        public async Task<IActionResult> Login(string username, string password)
        {
            var result = await _signInManager.PasswordSignInAsync(username, password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(username);
                var token = GenerateJwtToken(user);
                return Ok(new { Token = token });
            }

            return Unauthorized("Invalid login attempt");
        }

        private async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            // Criação da lista de claims com o UserName e UserId
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim("clienteID", user.Id), 
                new Claim("IdEmpresa", user.ClienteId)
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