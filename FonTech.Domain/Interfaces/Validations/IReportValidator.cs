using FonTech.Domain.Entity;
using FonTech.Domain.Result;

namespace FonTech.Domain.Interfaces.Validations;

public interface IReportValidator : IBaseValidator<Report>
{
    BaseResult CreateValidator(Report report, User user);
}