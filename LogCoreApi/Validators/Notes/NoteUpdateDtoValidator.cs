using FluentValidation;
using LogCoreApi.DTOs.Notes;

namespace LogCoreApi.Validators.Notes;

public class NoteUpdateDtoValidator : AbstractValidator<NoteUpdateDto>
{
    public NoteUpdateDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(100).WithMessage("Title must be at most 100 characters.");
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Content is required.")
            .MaximumLength(2000).WithMessage("Content must be at most 2000 characters.");
    }
}