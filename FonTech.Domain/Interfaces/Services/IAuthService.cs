using FonTech.Domain.Dto;
using FonTech.Domain.Dto.User;
using FonTech.Domain.Result;

namespace FonTech.Domain.Interfaces.Services;

/// <summary>
/// User Registration Service
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Users Registration
    /// </summary>
    /// <param name="???"></param>
    /// <returns></returns>
    Task<BaseResult<UserDto>> Register(RegisterUserDto dto);
    
    /// <summary>
    /// User Auth
    /// </summary>
    /// <param name="???"></param>
    /// <returns></returns>
    Task<BaseResult<TokenDto>> Login(LoginUserDto dto);
}