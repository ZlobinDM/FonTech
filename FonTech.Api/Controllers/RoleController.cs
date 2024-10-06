using System.Net.Mime;
using FonTech.Domain.Dto.Role;
using FonTech.Domain.Dto.UserRole;
using FonTech.Domain.Entity;
using FonTech.Domain.Interfaces.Services;
using FonTech.Domain.Result;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace FonTech.Api.Controllers;

/// <inheritdoc />
///
[Consumes(MediaTypeNames.Application.Json)]
public class RoleController : ControllerBase
{ 
    private readonly IRoleService _roleService;

    public RoleController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult<BaseResult<Role>>> Create([FromBody] CreateRoleDto dto)
    {
        var responce = await _roleService.CreateRoleAsync(dto);
        if(responce.IsSuccess)
            return Ok(responce); 

        return BadRequest(responce);
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPut]
    public async Task<ActionResult<BaseResult<Role>>> Update([FromBody] RoleDto dto)
    {
        var responce = await _roleService.UpdateRoleAsync(dto);
        if(responce.IsSuccess)
            return Ok(responce); 

        return BadRequest(responce);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete]
    public async Task<ActionResult<BaseResult<RoleDto>>> Delete([FromBody] long id)
    {
        var responce = await _roleService.DeleteRoleAsync(id);
        if(responce.IsSuccess)
            return Ok(responce); 

        return BadRequest(responce);
    }
    
    [HttpPost("AddRole")]
    public async Task<ActionResult<BaseResult<UserRole>>> AddRoleForUser([FromBody] UserRoleDto dto)
    {
        var responce = await _roleService.AddRoleForUserAsync(dto);
        if(responce.IsSuccess)
            return Ok(responce); 

        return BadRequest(responce);
    }
    
    [HttpPut("UpdateRole")]
    public async Task<ActionResult<BaseResult<UserRole>>> UpdateRoleForUser([FromBody] UpdateUserRoleDto dto)
    {
        var responce = await _roleService.UpdateRoleForUserAsync(dto);
        if(responce.IsSuccess)
            return Ok(responce); 

        return BadRequest(responce);
    }


    
    [HttpDelete("deleteRole")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BaseResult<UserRole>>> DeleteRole([FromBody] DeleteUserRoleDto dto)
    {
        var responce = await _roleService.DeleteRoleForUserAsync(dto);
        if(responce.IsSuccess)
            return Ok(responce);

        return BadRequest(responce);
    }
}