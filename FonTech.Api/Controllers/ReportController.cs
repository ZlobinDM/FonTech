using Asp.Versioning;
using FonTech.Domain.Dto.Report;
using FonTech.Domain.Dto.Role;
using FonTech.Domain.Dto.UserRole;
using FonTech.Domain.Interfaces.Services;
using FonTech.Domain.Result;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FonTech.Api.Controllers;

[ApiController]
[ApiVersion((1.0))]
[Route("api/{version:apiVersion}/[controller]")]
public class ReportController : ControllerBase
{
    private readonly IReportService _reportService;
    private readonly IRoleService _roleService;
    
    public ReportController(IReportService reportService, IRoleService roleService)
    {
        _reportService = reportService;
        _roleService = roleService;
    }
    
    // [Authorize]
    [HttpGet("reports")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BaseResult<ReportDto>>> GetReports(long userId)
    {
        var responce = await _reportService.GetReportsAsync(userId);
        if(responce.IsSuccess)
            return Ok(responce); 

        return BadRequest(responce);
    }
    
    [HttpGet("reports/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BaseResult<ReportDto>>> GetUserReports(long userId)
    {
        var responce = await _reportService.GetReportsAsync(userId);
        if(responce.IsSuccess)
            return Ok(responce);

        return BadRequest(responce);
    }
    
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BaseResult<ReportDto>>> GetReport(long id)
    {
        var responce = await _reportService.GetReportByIdAsync(id);
        if(responce.IsSuccess)
            return Ok(responce);

        return BadRequest(responce);
    }
    
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BaseResult<ReportDto>>> DeleteReport(long id)
    {
        var responce = await _reportService.DeleteReportAsync(id);
        if(responce.IsSuccess)
            return Ok(responce);

        return BadRequest(responce);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="dto"></param>
    /// <remarks>
    /// Creates Report bla bla bla
    /// returns object
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BaseResult<ReportDto>>> CreateReport(CreateReportDto dto)
    {
        var responce = await _reportService.CreateReportAsync(dto);
        if(responce.IsSuccess)
            return Ok(responce);

        return BadRequest(responce);
    }
    
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BaseResult<ReportDto>>> UpdateReport(UpdateReportDto dto)
    {
        var responce = await _reportService.UpdateReportAsync(dto);
        if(responce.IsSuccess)
            return Ok(responce);

        return BadRequest(responce);
    }
    
}
