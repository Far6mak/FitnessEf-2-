using FitnessEf;
using Microsoft.EntityFrameworkCore;

await using var db = new FitnessContext();

Console.WriteLine("=== 10.1. Проверка нового свойства Note ===");

// Найдем клиента с ID 1 и добавим ему заметку
var client = await db.Clients.FindAsync(1);

if (client != null)
{
    client.Note = "VIP-клиент, постоянные персональные тренировки";
    await db.SaveChangesAsync();
    Console.WriteLine($"Клиенту {client.FirstName} добавлена заметка: {client.Note}");
}
else
{
    Console.WriteLine("Клиент с ID 1 не найден.");
}

Console.WriteLine("\nВсе текущие заметки:");
var clientsWithNotes = await db.Clients
    .AsNoTracking()
    .Where(c => c.Note != null)
    .ToListAsync();

foreach (var c in clientsWithNotes)
{
    Console.WriteLine($"- {c.FirstName} {c.LastName}: {c.Note}");
}

Console.WriteLine("\nНажмите любую клавишу для выхода...");
Console.ReadKey();