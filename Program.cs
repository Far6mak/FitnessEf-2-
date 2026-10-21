using FitnessEf;
using Microsoft.EntityFrameworkCore;

await using var db = new FitnessContext();

// --- 9.4. Добавление посещения ---
Console.WriteLine("=== 9.4. Добавление посещения ===");
int clientId = 1; // Берем ИД первого клиента (Иван Иванов)

// Проверяем, существует ли клиент
var clientExists = await db.Clients.AnyAsync(c => c.ClientId == clientId);
if (!clientExists)
{
    Console.WriteLine($"Клиента с ID {clientId} нет.");
}
else
{
    var newVisit = new Visit
    {
        ClientId = clientId,
        VisitDate = DateOnly.FromDateTime(DateTime.Now.AddDays(7)), // Дата через неделю
        Note = "Записался на первое занятие"
    };

    db.Visits.Add(newVisit);
    await db.SaveChangesAsync();
    Console.WriteLine($"Посещение для клиента {clientId} успешно добавлено.");
}

// --- 9.5. Чтение клиента вместе с посещениями (Include) ---
Console.WriteLine("\n=== 9.5. Клиенты и их посещения (Include) ===");

var clientsWithVisits = await db.Clients
    .AsNoTracking()
    .Include(c => c.Visits) // Загружаем связанные данные (JOIN в SQL)
    .OrderBy(c => c.ClientId)
    .ToListAsync();

foreach (var client in clientsWithVisits)
{
    Console.WriteLine($"\nКлиент: {client.FirstName} {client.LastName}");

    if (client.Visits.Count == 0)
    {
        Console.WriteLine("  Нет посещений.");
    }
    else
    {
        foreach (var visit in client.Visits)
        {
            Console.WriteLine($"  - {visit.VisitDate}: {visit.Note}");
        }
    }
}

Console.WriteLine("\nНажмите любую клавишу для выхода...");
Console.ReadKey();