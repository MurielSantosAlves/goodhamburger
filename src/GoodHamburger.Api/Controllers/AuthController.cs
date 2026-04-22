using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FluentValidation;
using GoodHamburger.Domain.DTOs;
using GoodHamburger.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace GoodHamburger.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IConfiguration _configuration;
    private readonly IValidator<RegisterDto> _registerValidator;
    private readonly IValidator<LoginDto> _loginValidator;
    
    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IConfiguration configuration,
        IValidator<RegisterDto> registerValidator,
        IValidator<LoginDto> loginValidator)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
        _registerValidator = registerValidator;
        _loginValidator = loginValidator;
    }
    
    // POST api/auth/register - cadastro de novo usuário
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto dto)
    {
        var validation = await _registerValidator.ValidateAsync(dto);
        if (!validation.IsValid)
        {
            return BadRequest(new { errors = validation.Errors.Select(e => e.ErrorMessage) });
        }
        
        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email
        };
        
        var result = await _userManager.CreateAsync(user, dto.Password);
        
        if (!result.Succeeded)
        {
            return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
        }
        
        var token = GenerateJwtToken(user);
        return Ok(token);
    }
    
    // POST api/auth/login - autentica usuário e retorna JWT
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto dto)
    {
        var validation = await _loginValidator.ValidateAsync(dto);
        if (!validation.IsValid)
        {
            return BadRequest(new { errors = validation.Errors.Select(e => e.ErrorMessage) });
        }
        
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
        {
            return Unauthorized(new { message = "Email ou senha inválidos" });
        }
        
        // verifica senha sem bloquear conta
        var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
        
        if (!result.Succeeded)
        {
            return Unauthorized(new { message = "Email ou senha inválidos" });
        }
        
        var token = GenerateJwtToken(user);
        return Ok(token);
    }
    
    // gera token JWT com claims do usuário
    private AuthResponseDto GenerateJwtToken(ApplicationUser user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");
        var expirationMinutes = int.Parse(jwtSettings["ExpirationInMinutes"] ?? "60");
        
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id)
        };
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(expirationMinutes);
        
        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );
        
        return new AuthResponseDto(
            new JwtSecurityTokenHandler().WriteToken(token),
            user.Email ?? string.Empty,
            expires
        );
    }
}
