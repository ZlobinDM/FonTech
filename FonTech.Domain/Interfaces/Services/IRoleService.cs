using FonTech.Domain.Dto.Role;
using FonTech.Domain.Dto.UserRole;
using FonTech.Domain.Entity;
using FonTech.Domain.Result;

namespace FonTech.Domain.Interfaces.Services;

public interface IRoleService
{
    Task<BaseResult<RoleDto>> CreateRoleAsync(CreateRoleDto dto);
    
    Task<BaseResult<RoleDto>> DeleteRoleAsync(long id);
    Task<BaseResult<RoleDto>> UpdateRoleAsync(RoleDto dto);
    
    
    Task<BaseResult<UserRoleDto>> AddRoleForUserAsync(UserRoleDto dto);
    Task<BaseResult<UserRoleDto>> UpdateRoleForUserAsync(UpdateUserRoleDto dto);
    
    Task<BaseResult<UserRoleDto>> DeleteRoleForUserAsync(DeleteUserRoleDto dto);
}