namespace FitnessEf;

public sealed class Visit
{
    public int VisitId { get; set; }
    public int ClientId { get; set; }

    // Используем DateOnly для типа date в PostgreSQL
    public DateOnly VisitDate { get; set; }

    public string? Note { get; set; }

    // Навигационное свойство
    public Client Client { get; set; } = null!;
}