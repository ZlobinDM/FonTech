using FonTech.Domain.Dto;
using FonTech.Domain.Dto.User;
using FonTech.Domain.Interfaces.Services;
using FonTech.Domain.Result;
using Microsoft.AspNetCore.Mvc;

namespace FonTech.Api.Controllers;

[ApiController]
public class AuthController : Controller
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost("register")]
    public async Task<ActionResult<BaseResult>> Register([FromBody] RegisterUserDto dto)
    {
        var responce = await _authService.Register(dto);
        if(responce.IsSuccess)
            return Ok(responce);

        return BadRequest(responce);
    }    
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost("login")]
    public async Task<ActionResult<TokenDto>> Login([FromBody] LoginUserDto dto)
    {
        var responce = await _authService.Login(dto);
        if(responce.IsSuccess)
            return Ok(responce);

        return BadRequest(responce);
    }
}