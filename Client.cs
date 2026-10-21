namespace FitnessEf;

public sealed class Client
{
    public int ClientId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public DateOnly? BirthDate { get; set; }

    public string? Phone { get; set; }
    public string? Email { get; set; }
    public DateOnly RegistrationDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    public string Status { get; set; } = "active";

    // Связь один-ко-многим: у клиента может быть много посещений
    public List<Visit> Visits { get; set; } = new();

    public string? Note { get; set; }
}