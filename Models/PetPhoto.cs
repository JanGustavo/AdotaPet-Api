namespace AdotaPet.Api.Models;

public class PetPhoto

{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Url { get; set; } = string.Empty;

    //FK
    public Guid PetId { get; set; }

    //navegação
    public Pet? Pet { get; set; }
}
