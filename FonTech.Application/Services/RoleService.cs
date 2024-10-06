using AutoMapper;
using FonTech.Application.Resources;
using FonTech.Domain.Dto.Role;
using FonTech.Domain.Dto.UserRole;
using FonTech.Domain.Entity;
using FonTech.Domain.Enum;
using FonTech.Domain.Interfaces.Repositories;
using FonTech.Domain.Interfaces.Services;
using FonTech.Domain.Result;
using Microsoft.EntityFrameworkCore;

namespace FonTech.Application.Services;

public class RoleService : IRoleService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBaseRepository<User> _userRepository;
    private readonly IBaseRepository<Role> _roleRepository;
    private readonly IBaseRepository<UserRole> _userRoleRepository;
    private readonly IMapper _mapper;

    public RoleService(IBaseRepository<User> userRepository, IBaseRepository<Role> roleRepository, IMapper mapper, IBaseRepository<UserRole> userRoleRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _userRoleRepository = userRoleRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<BaseResult<RoleDto>> CreateRoleAsync(CreateRoleDto dto)
    {
        var role = await _roleRepository.GetAll().FirstOrDefaultAsync(x => x.Name == dto.Name);

        if (role is not null)
        {
            return new BaseResult<RoleDto>()
            {
                ErrorMessage = ErrorMessage.RoleIsAreadyExists,
                ErrorCode = (int)ErrorCodes.RoleIsAsreadyExists
            };
        }
        
        role = new Role()
        {
            Name = dto.Name
        };

        await _roleRepository.CreateAsync(role);
        
        return new BaseResult<RoleDto>()
        {
            Data = _mapper.Map<RoleDto>(role)
        };
    }

    public async Task<BaseResult<RoleDto>> DeleteRoleAsync(long id)
    {
        var role = await _roleRepository.GetAll().FirstOrDefaultAsync(x => x.Id == id);

        if (role is null)
        {
            return new BaseResult<RoleDto>()
            {
                ErrorMessage = ErrorMessage.RoleNotFound,
                ErrorCode = (int)ErrorCodes.RoleNotFound
            };
        }

        await _roleRepository.DeleteAsync(role);

        return new BaseResult<RoleDto>()
        {
            Data = _mapper.Map<RoleDto>(role)
        };
    }

    public async Task<BaseResult<RoleDto>> UpdateRoleAsync(RoleDto dto)
    {
        var role = await _roleRepository.GetAll().FirstOrDefaultAsync(x => x.Id == dto.Id);

        if (role is null)
        {
            return new BaseResult<RoleDto>()
            {
                ErrorMessage = ErrorMessage.RoleNotFound,
                ErrorCode = (int)ErrorCodes.RoleNotFound
            };
        }

        role.Name = dto.Name;

        await _roleRepository.UpdateAsync(role);

        return new BaseResult<RoleDto>()
        {
            Data = _mapper.Map<RoleDto>(role)
        };
    }

    public async Task<BaseResult<UserRoleDto>> AddRoleForUserAsync(UserRoleDto dto)
    {
        var user = await _userRepository.GetAll()
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(x => x.Login == dto.Login);

        if (user == null)
        {
            return new BaseResult<UserRoleDto>()
            {
                ErrorCode = (int)ErrorCodes.UserNotFound,
                ErrorMessage = ErrorMessage.UserNotFound
            };
        }

        var roles = user.Roles.Select(x => x.Name).ToArray();
        if (!roles.Any(x => x == dto.RoleName))
        {
            var role = await _roleRepository.GetAll().FirstOrDefaultAsync(x => x.Name == dto.RoleName);
            if (role is null)
            {
                return new BaseResult<UserRoleDto>()
                {
                    ErrorCode = (int)ErrorCodes.RoleNotFound,
                    ErrorMessage = ErrorMessage.RoleNotFound
                };
            }
            
            UserRole userRole = new UserRole()
            {
                RoleId = role.Id,
                UserId = user.Id
            };
            
            await _userRoleRepository.CreateAsync(userRole);

            return new BaseResult<UserRoleDto>()
            {
                Data = new UserRoleDto()
                {
                    Login = user.Login,
                    RoleName = role.Name
                }
            };
        }
        
        return new BaseResult<UserRoleDto>()
        {
            ErrorMessage = ErrorMessage.UserAlreadyHasThisRole,
            ErrorCode = (int)ErrorCodes.UserAlreadyHasThisRole
        };
    }
    
    
    public async Task<BaseResult<UserRoleDto>> DeleteRoleForUserAsync(DeleteUserRoleDto dto)
    {
        var user = await _userRepository.GetAll()
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(x => x.Login == dto.Login);

        if (user == null)
        {
            return new BaseResult<UserRoleDto>()
            {
                ErrorCode = (int)ErrorCodes.UserNotFound,
                ErrorMessage = ErrorMessage.UserNotFound
            };
        }

        var role = user.Roles.FirstOrDefault(x => x.Id == dto.RoleId);
        if (role is null)
        {
            return new BaseResult<UserRoleDto>()
            {
                ErrorCode = (int)ErrorCodes.RoleNotFound,
                ErrorMessage = ErrorMessage.RoleNotFound
            };
        }

        var userRole = await _userRoleRepository.GetAll()
            .FirstOrDefaultAsync(x => x.UserId == user.Id && x.RoleId == role.Id);

        if (userRole is not null)
          await _userRoleRepository.DeleteAsync(userRole);
            
        return new BaseResult<UserRoleDto>()
        {
            Data = _mapper.Map<UserRoleDto>(userRole)
        };
    }
    
    public async Task<BaseResult<UserRoleDto>> UpdateRoleForUserAsync(UpdateUserRoleDto dto)
    {
        var user = await _userRepository.GetAll()
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(x => x.Login == dto.Login);

        if (user == null)
        {
            return new BaseResult<UserRoleDto>()
            {
                ErrorCode = (int)ErrorCodes.UserNotFound,
                ErrorMessage = ErrorMessage.UserNotFound
            };
        }
        
        var role = await _roleRepository.GetAll().FirstOrDefaultAsync(x => x.Id == dto.FromRoleId);

        if (role is null)
        {
            return new BaseResult<UserRoleDto>()
            {
                ErrorMessage = ErrorMessage.RoleNotFound,
                ErrorCode = (int)ErrorCodes.RoleNotFound
            };
        }
        
        var newRole = await _roleRepository.GetAll().FirstOrDefaultAsync(x => x.Id == dto.ToRoleId);

        if (newRole is null)
        {
            return new BaseResult<UserRoleDto>()
            {
                ErrorMessage = ErrorMessage.RoleNotFound,
                ErrorCode = (int)ErrorCodes.RoleNotFound
            };
        }

  
        using (var transaction = await _unitOfWork.BeginTransactionAsync())
        {
            try
            {
                var userRole = await _unitOfWork.UserRoles.GetAll()
                    .FirstOrDefaultAsync(x => x.UserId == user.Id && x.RoleId == role.Id);
                
                await _unitOfWork.UserRoles.DeleteAsync(userRole);


                var newUserRole = new UserRole()
                {
                    UserId = user.Id,
                    RoleId = newRole.Id
                };

                await _unitOfWork.UserRoles.CreateAsync(newUserRole);
                
                await _unitOfWork.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
            }
        }



  
        return new BaseResult<UserRoleDto>()
        {
            Data = new UserRoleDto()
            {
                Login = user.Login,
                RoleName = newRole.Name
            }
        };
    }
}