using FonTech.Domain.Dto.Report;
using FonTech.Domain.Result;

namespace FonTech.Domain.Interfaces.Services;
/// <summary>
/// Сервис для работы отчетами
/// </summary>
public interface IReportService 
{
    /// <summary>
    /// Получение всех отчетов пользователя
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<CollectionResult<ReportDto>> GetReportsAsync(long userId);

    /// <summary>
    /// По идентификатору
    /// </summary>
    /// <param name="id"></param>
    /// <remarks>
    /// Create request report 
    /// POST
    /// {
    ///     "name": "Report 1",
    ///     "description": "test report",
    ///     "userId": 1
    /// }
    /// </remarks>
    /// <responce code="200"> Report created </responce>
    /// 
    Task<BaseResult<ReportDto>> GetReportByIdAsync(long id);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dto"></param>
    /// <remarks></remarks>
    Task<BaseResult<ReportDto>> CreateReportAsync(CreateReportDto dto);
    Task<BaseResult<ReportDto>> UpdateReportAsync(UpdateReportDto dto);
    Task<BaseResult<ReportDto>> DeleteReportAsync(long id);


}