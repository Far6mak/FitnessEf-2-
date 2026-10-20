using FitnessEf;
using Microsoft.EntityFrameworkCore;

await using var db = new FitnessContext();

// 5.1. Фильтрация через Where
Console.WriteLine("=== 5.1. Только активные клиенты (Where) ===");
var activeClients = await db.Clients
    .AsNoTracking()
    .Where(c => c.Status == "active")
    .OrderBy(c => c.LastName)
    .ToListAsync();

foreach (var c in activeClients)
{
    Console.WriteLine($"{c.LastName} {c.FirstName} - {c.Status}");
}

// 5.2. Поиск по фрагменту фамилии
Console.WriteLine("\n=== 5.2. Поиск по фамилии (Contains) ===");
Console.Write("Введите фрагмент фамилии для поиска (например, 'Смир' или 'Иван'): ");
string term = Console.ReadLine()?.Trim() ?? string.Empty;

if (string.IsNullOrWhiteSpace(term))
{
    Console.WriteLine("Строка поиска не задана.");
}
else
{
    var found = await db.Clients
        .AsNoTracking()
        .Where(c => c.LastName.Contains(term)) // Переводится в SQL LIKE '%term%'
        .OrderBy(c => c.LastName)
        .ToListAsync();

    Console.WriteLine($"Найдено записей: {found.Count}");
    foreach (var c in found)
    {
        Console.WriteLine($"{c.ClientId}: {c.FirstName} {c.LastName}");
    }
}

// 5.3. Поиск по идентификатору
Console.WriteLine("\n=== 5.3. Поиск по ID (FirstOrDefault) ===");
int searchId = 2;
var clientById = await db.Clients.FirstOrDefaultAsync(c => c.ClientId == searchId);
if (clientById is null)
    Console.WriteLine($"Клиент с ID {searchId} не найден.");
else
    Console.WriteLine($"Найден: {clientById.FirstName} {clientById.LastName}");

// 5.4. Количество и наличие
Console.WriteLine("\n=== 5.4. Агрегаты (Count, Any) ===");
int total = await db.Clients.CountAsync();
bool hasInactive = await db.Clients.AnyAsync(c => c.Status == "inactive");
int activeCount = await db.Clients.CountAsync(c => c.Status == "active");

Console.WriteLine($"Всего клиентов в базе: {total}");
Console.WriteLine($"Активных клиентов: {activeCount}");
Console.WriteLine(hasInactive ? "В базе есть неактивные клиенты." : "Неактивных клиентов нет.");

// 5.5. Постраничная загрузка (Пагинация)
Console.WriteLine("\n=== 5.5. Постраничная загрузка (Skip, Take) ===");
int page = 1;
int pageSize = 2; // Выведем по 2 записи на страницу (у нас в базе 3 записи)
var pageOfClients = await db.Clients
    .AsNoTracking()
    .OrderBy(c => c.ClientId)
    .Skip((page - 1) * pageSize) // Пропускаем записи предыдущих страниц
    .Take(pageSize)              // Берем только pageSize записей
    .ToListAsync();

Console.WriteLine($"--- Страница {page} ---");
foreach (var c in pageOfClients)
{
    Console.WriteLine($"{c.ClientId}: {c.FirstName} {c.LastName}");
}

Console.WriteLine("\nНажмите любую клавишу для выхода...");
Console.ReadKey();