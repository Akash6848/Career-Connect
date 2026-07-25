using System.ComponentModel.DataAnnotations;

namespace CareerConnect.Shared.Validation;

/// <summary>
/// Class-level validator for partial-update DTOs (e.g. UpdateUserDto, UpdateExperienceDto):
/// rejects requests where every property is null, since such an update would be a no-op.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class AtLeastOneNotNullAttribute : ValidationAttribute
{
    public AtLeastOneNotNullAttribute()
    {
        ErrorMessage = "At least one field must be provided";
    }

    public override bool IsValid(object? value)
    {
        if (value is null) return false;

        return value.GetType()
            .GetProperties()
            .Any(property => property.GetValue(value) is not null);
    }
}
