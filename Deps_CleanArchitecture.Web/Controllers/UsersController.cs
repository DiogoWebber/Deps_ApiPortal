using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Deps_CleanArchitecture.Core.Entities;
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

        public UsersController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(string username, string password)
        {
            var user = new ApplicationUser { UserName = username };
            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                // Atribui automaticamente a role 'Usuario'
                await _userManager.AddToRoleAsync(user, "Usuário");
                return Ok("Usuário registrado com sucesso com a role de 'Usuario'.");
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
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
            };

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AmbienteUtil.GetValue("JWT_KEY")));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: AmbienteUtil.GetValue("JWT_ISSUER"),
                audience: AmbienteUtil.GetValue("JWT_AUDIENCE"),
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }