namespace AdotaPet.Api.Models;

public class PetResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Species { get; set; } = string.Empty;
    public string? Breed { get; set; }
    public string? Description { get; set; }
    public int? Age { get; set; }

    public string? OwnerName { get; set; }
    public string? OwnerPhone { get; set; }

    public List<string> Photos { get; set; } = new();
}