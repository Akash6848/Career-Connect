using System.ComponentModel.DataAnnotations;
using CareerConnect.Shared.Validation;

namespace CareerConnect.UsersService.Dto;

[AtLeastOneNotNull]
public class UpdateUserDto
{
    [StringLength(50, MinimumLength = 5)]
    public string? Username { get; set; }

    [StringLength(50, MinimumLength = 2)]
    public string? FirstName { get; set; }

    [StringLength(50, MinimumLength = 2)]
    public string? LastName { get; set; }

    [StringLength(12, MinimumLength = 8)]
    public string? Password { get; set; }
}
