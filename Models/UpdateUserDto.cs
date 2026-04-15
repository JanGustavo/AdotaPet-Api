namespace AdotaPet.Api.Models;

public class UpdateUserDto
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public bool HasWhatsapp { get; set; }
    public int PetsRegistered { get; set; }
}
