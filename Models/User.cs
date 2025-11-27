using System.ComponentModel.DataAnnotations;

namespace AdotaPet.Api.Models;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public UserType UserType { get; set; }

    [Required] 
    public String Phone { get; set; }= string.Empty;

    [Required]
    public bool HasWhatsapp { get; set; }

}