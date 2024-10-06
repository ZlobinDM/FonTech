using FluentValidation;
using FonTech.Domain.Dto.Report;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace FonTech.Application.Validations.FluentValidations.Report;

public class CreateValidator : AbstractValidator<CreateReportDto>
{
    public CreateValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(1000);
    }
}