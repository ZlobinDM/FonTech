using FluentValidation;
using FonTech.Domain.Dto.Report;

namespace FonTech.Application.Validations.FluentValidations.Report;

public class UpdateValidator : AbstractValidator<UpdateReportDto>
{
    public UpdateValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(1000);

    }
    
}