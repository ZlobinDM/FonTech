using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using FonTech.Application.Resources;
using FonTech.Domain.Dto;
using FonTech.Domain.Dto.User;
using FonTech.Domain.Entity;
using FonTech.Domain.Enum;
using FonTech.Domain.Interfaces.Repositories;
using FonTech.Domain.Interfaces.Services;
using FonTech.Domain.Result;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace FonTech.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBaseRepository<User> _userRepository;
    private readonly IBaseRepository<UserRole> _userRolesRepository;
    private readonly IBaseRepository<Role> _rolesRepository;
    private readonly IBaseRepository<UserToken> _userTokenRepository;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;

    public AuthService(
        IBaseRepository<User> userRepository, 
        IBaseRepository<UserToken> userTokenRepository,
        ITokenService tokenService,
        IMapper mapper,
        ILogger logger, IBaseRepository<Role> rolesRepository, IBaseRepository<UserRole> userRolesRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _userTokenRepository = userTokenRepository;
        _tokenService = tokenService;
        _mapper = mapper;
        _logger = logger;
        _rolesRepository = rolesRepository;
        _userRolesRepository = userRolesRepository;
        _unitOfWork = unitOfWork;
    }
    public async Task<BaseResult<UserDto>> Register(RegisterUserDto dto)
    {
        if (dto.Password != dto.PasswordConfirm)
        {
            return new BaseResult<UserDto>()
            {
                ErrorCode = (int)ErrorCodes.PasswordNotEqualsPasswordConfirm,
                ErrorMessage = ErrorMessage.PasswordNotEqualsPasswordConfirm
            };
        }

        try
        {
            var user = await _userRepository.GetAll().FirstOrDefaultAsync(x => x.Login == dto.Login);
            if (user != null)
            {
                return new BaseResult<UserDto>()
                {
                    ErrorCode = (int)ErrorCodes.UserAlreadyExits,
                    ErrorMessage = ErrorMessage.UserAlreadeExists
                };
            }

            var hashUserPassword = HashPassword(dto.Password);

            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    user = new User()
                    {
                        Login = dto.Login,
                        Password = hashUserPassword
                    };
                    
                    await _unitOfWork.Users.CreateAsync(user);

                    var role = await _rolesRepository.GetAll().FirstOrDefaultAsync(x => x.Name == nameof(Roles.User));

                    if (role == null)
                    {
                        return new BaseResult<UserDto>()
                        {
                            ErrorCode = (int)ErrorCodes.RoleNotFound,
                            ErrorMessage = ErrorMessage.RoleNotFound
                        };
                    }

                    var userRole = new UserRole()
                    {
                        UserId = user.Id,
                        RoleId = role.Id
                    };

                    await _unitOfWork.UserRoles.CreateAsync(userRole);

                    await transaction.CommitAsync();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    await transaction.RollbackAsync();
                }
            }

            
            return new BaseResult<UserDto>()
            {
                Data = _mapper.Map<UserDto>(user)
            };
        }
        catch (Exception e)
        {
            _logger.Error(e, e.Message);
            return new BaseResult<UserDto>()
            {
                ErrorMessage = ErrorMessage.InternalServerError,
                ErrorCode = (int)ErrorCodes.InternalServerError
            };
        }
    }

    public async Task<BaseResult<TokenDto>> Login(LoginUserDto dto)
    {
        try
        {
            var user = await _userRepository.GetAll()
                .Include(x => x.Roles)
                .FirstOrDefaultAsync(x => x.Login == dto.Login);
            if (user != null)
            {
                return new BaseResult<TokenDto>()
                {
                    ErrorCode = (int)ErrorCodes.UserNotFound,
                    ErrorMessage = ErrorMessage.UserNotFound
                };
            }

            var isVerifiedPassword = IsVerifiedPassword(user.Password, dto.Password);
            if (!isVerifiedPassword)
            {
                return new BaseResult<TokenDto>()
                {
                    ErrorCode = (int)ErrorCodes.PasswordIsWrong,
                    ErrorMessage = ErrorMessage.PasswordIsWrong
                };
            }

            var userToken = await _userTokenRepository.GetAll().FirstOrDefaultAsync(x => x.UserId == user.Id);

            var userRoles = user.Roles;
            var claims = userRoles.Select(x => new Claim(ClaimTypes.Role, x.Name)).ToList();
                claims.Add(new Claim(ClaimTypes.Name, user.Login));
            
            var accessToken = _tokenService.GenerateAccessToken(claims);
            var refreshToken = _tokenService.GenerateRefreshToken();

            if (userToken == null)
            {
                userToken = new UserToken()
                {
                    UserId = user.Id,
                    RefreshToken = refreshToken,
                    RefreshTokenExpiredTime = DateTime.UtcNow.AddDays(7)
                };
                await _userTokenRepository.CreateAsync(userToken);
            }
            else
            {
                userToken.RefreshToken = refreshToken;
                userToken.RefreshTokenExpiredTime = DateTime.UtcNow.AddDays(7);
                
                await _userTokenRepository.UpdateAsync(userToken);
            }

            return new BaseResult<TokenDto>()
            {
                Data = new TokenDto()
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                }
            };
        }
        catch (Exception e)
        {
            _logger.Error(e, e.Message);
            return new BaseResult<TokenDto>()
            {
                ErrorMessage = ErrorMessage.InternalServerError,
                ErrorCode = (int)ErrorCodes.InternalServerError
            };
        }
    }

    private string HashPassword(string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }

    private bool IsVerifiedPassword(string userPasswordHash, string userPassword)
    {
        return userPasswordHash == HashPassword(userPassword);
    }
}