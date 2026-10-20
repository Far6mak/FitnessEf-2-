using FitnessEf;
using Microsoft.EntityFrameworkCore;

await using var db = new FitnessContext();

Console.WriteLine("=== 1. Получение всех клиентов (сортировка по ID) ===");

// Пункт 4.1: AsNoTracking (только чтение) + OrderBy + ToListAsync
var clients = await db.Clients
    .AsNoTracking()
    .OrderBy(c => c.ClientId)
    .ToListAsync();

foreach (var client in clients)
{
    Console.WriteLine($"{client.ClientId}: {client.FirstName} {client.LastName} (Статус: {client.Status})");
}

Console.WriteLine("\n=== 2. Проекция в анонимный тип (сортировка по Фамилии, затем Имени) ===");

// Пункт 4.2: Projection через Select
var projectedClients = await db.Clients
    .AsNoTracking()
    .OrderBy(c => c.LastName)
    .ThenBy(c => c.FirstName)
    .Select(c => new
    {
        c.ClientId,
        FullName = c.FirstName + " " + c.LastName,
        c.Phone
    })
    .ToListAsync();

foreach (var c in projectedClients)
{
    Console.WriteLine($"{c.ClientId}: {c.FullName}; тел.: {c.Phone ?? "не указан"}");
}

// Пауза, чтобы консоль не закрывалась сразу
Console.WriteLine("\nНажмите любую клавишу для выхода...");
Console.ReadKey();