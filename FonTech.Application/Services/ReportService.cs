using AutoMapper;
using FonTech.Application.Resources;
using FonTech.Domain.Dto.Report;
using FonTech.Domain.Entity;
using FonTech.Domain.Enum;
using FonTech.Domain.Interfaces.Repositories;
using FonTech.Domain.Interfaces.Services;
using FonTech.Domain.Interfaces.Validations;
using FonTech.Domain.Result;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace FonTech.Application.Services;

/// <inheritdoc />
public class ReportService : IReportService
{
    private readonly IBaseRepository<Report> _reportRepository;
    private readonly IBaseRepository<User> _userRepository;
    private readonly IReportValidator _reportValidator;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;

    public ReportService(IBaseRepository<Report> reportRepository,
        IBaseRepository<User> userRepository, 
        IReportValidator reportValidator,
        IMapper mapper,
        ILogger logger)
    {
        _reportRepository = reportRepository;
        _userRepository = userRepository;
        _reportValidator = reportValidator;
        _mapper = mapper;
        _logger = logger;
    }
    public async Task<CollectionResult<ReportDto>> GetReportsAsync(long userId)
    {
        ReportDto[] reports;
 
        try
        {
            reports = await _reportRepository.GetAll()
                .Where(x => x.UserId == userId)
                .Select(x => new ReportDto(x.Id, x.Name, x.Description, x.CreatedAt.ToLongDateString()))
                .ToArrayAsync();
        }
        catch (Exception e)
        {
            _logger.Error(e, e.Message);
            return new CollectionResult<ReportDto>()
            {
                ErrorMessage = ErrorMessage.InternalServerError,
                ErrorCode = (int)ErrorCodes.InternalServerError
            };
        }

        if (!reports.Any())
        {
            _logger.Warning(ErrorMessage.ReportsNotFound, reports.Length);
            return new CollectionResult<ReportDto>()
            {
                ErrorMessage = ErrorMessage.ReportNotFound,
                ErrorCode = (int)ErrorCodes.ReportsNotFound
            };
        }
        return new CollectionResult<ReportDto>()
        {
            Data = reports,
            Count = reports.Length
        };
    }

    public async Task<BaseResult<ReportDto>> GetReportByIdAsync(long id)
    {
        ReportDto? report;
        try
        {
            report = await _reportRepository.GetAll()
                .Select(x => new ReportDto(x.Id, x.Name, x.Description, x.CreatedAt.ToLongDateString()))
                .FirstOrDefaultAsync(x => x.Id == id);
            //return report;
        }
        catch (Exception e)
        {
            _logger.Warning(ErrorMessage.ReportsNotFound);
            return new BaseResult<ReportDto>()
            {
                ErrorMessage = ErrorMessage.InternalServerError,
                ErrorCode = (int)ErrorCodes.InternalServerError
            };
        }

        if (report is null)
        {
            _logger.Warning($"Отчет с ид {id} не найден");
            return new BaseResult<ReportDto>()
            {
                ErrorMessage = ErrorMessage.ReportNotFound,
                ErrorCode = (int)ErrorCodes.ReportNotFound
            };
        }
        else
        {
            return new BaseResult<ReportDto>()
            {
                Data = _mapper.Map<ReportDto>(report)
            };
        }
    }

    public async Task<BaseResult<ReportDto>> CreateReportAsync(CreateReportDto dto)
    {
        try
        {
            var user = await _userRepository.GetAll().FirstOrDefaultAsync(x => x.Id == dto.userId);
            var report = await _reportRepository.GetAll()
                .FirstOrDefaultAsync(x => x.Name == dto.Name);
            var result = _reportValidator.CreateValidator(report, user);

            if (!result.IsSuccess)
            {
                return new BaseResult<ReportDto>()
                {
                    ErrorMessage = result.ErrorMessage,
                    ErrorCode = result.ErrorCode
                };
            }

            report = new Report()
            {
                Name = dto.Name,
                Description = dto.Description,
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedBy = user.Id,
                UpdatedAt = DateTime.UtcNow
            };

            await _reportRepository.CreateAsync(report);
            
            return new BaseResult<ReportDto>()
            {
                Data = _mapper.Map<ReportDto>(report)
            };
        }
        catch (Exception e)
        {
            _logger.Error(e, e.Message);
            return new BaseResult<ReportDto>()
            {
                ErrorMessage = ErrorMessage.InternalServerError,
                ErrorCode = (int)ErrorCodes.InternalServerError
            };
        }
    }

    public async Task<BaseResult<ReportDto>> UpdateReportAsync(UpdateReportDto dto)
    {
        try
        {
            var report = await _reportRepository.GetAll().FirstOrDefaultAsync(x => x.Id == dto.Id);
            var result = _reportValidator.ValidateOnNull(report);

            if (!result.IsSuccess)
            {
                return new BaseResult<ReportDto>()
                {
                    ErrorMessage = result.ErrorMessage,
                    ErrorCode = result.ErrorCode
                };
            }

            report.Name = dto.Name;
            report.Description = dto.Description;

            await _reportRepository.UpdateAsync(report);
            
            return new BaseResult<ReportDto>()
            {
                Data = _mapper.Map<ReportDto>(report)
            };
            
        }
    catch (Exception e)
    {
        _logger.Error(e, e.Message);
        return new BaseResult<ReportDto>()
        {
            ErrorMessage = ErrorMessage.InternalServerError,
            ErrorCode = (int)ErrorCodes.InternalServerError
        };
    }
    }

    public async Task<BaseResult<ReportDto>> DeleteReportAsync(long id)
    {
        try
        {
            var report = await _reportRepository.GetAll().FirstOrDefaultAsync(x => x.Id == id);
            var result = _reportValidator.ValidateOnNull(report);

            if (!result.IsSuccess)
            {
                return new BaseResult<ReportDto>()
                {
                    ErrorMessage = result.ErrorMessage,
                    ErrorCode = result.ErrorCode
                };
            }
            
            await _reportRepository.DeleteAsync(report);
            
            return new BaseResult<ReportDto>()
            {
                Data = _mapper.Map<ReportDto>(report)
            };
        }
        catch (Exception e)
        {
            _logger.Warning(ErrorMessage.ReportsNotFound);
            return new BaseResult<ReportDto>()
            {
                ErrorMessage = ErrorMessage.InternalServerError,
                ErrorCode = (int)ErrorCodes.InternalServerError
            };
        }
    }
}