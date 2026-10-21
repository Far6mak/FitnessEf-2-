using FitnessEf;
using Microsoft.EntityFrameworkCore;

await using var db = new FitnessContext();

// --- 8.1. NULL и nullable-свойства ---
Console.WriteLine("=== 8.1. Поиск клиентов без Email (IS NULL) ===");

// В C# проверка c.Email == null транслируется в SQL: WHERE email IS NULL
var clientsWithoutEmail = await db.Clients
    .AsNoTracking()
    .Where(c => c.Email == null)
    .OrderBy(c => c.ClientId)
    .ToListAsync();

if (clientsWithoutEmail.Count == 0)
    Console.WriteLine("Клиентов без email не найдено.");
else
{
    foreach (var c in clientsWithoutEmail)
    {
        Console.WriteLine($"{c.ClientId}: {c.FirstName} {c.LastName} (Email не указан)");
    }
}

// --- 8.4. Проверка соответствия схеме ---
Console.WriteLine("\n=== 8.4. Проверка ограничений БД (длина строки) ===");
Console.WriteLine("Пытаемся сохранить имя длиной 150 символов...");

try
{
    // В нашем FitnessContext мы задали для FirstName ограничение HasMaxLength(100)
    // Создадим строку из 150 символов 'А'
    string tooLongName = new string('А', 150);

    var invalidClient = new Client
    {
        FirstName = tooLongName, // Нарушаем ограничение в 100 символов!
        LastName = "Тестов",
        RegistrationDate = DateOnly.FromDateTime(DateTime.Today),
        Status = "active"
    };

    db.Clients.Add(invalidClient);
    await db.SaveChangesAsync();

    Console.WriteLine("Сохранено (чего не должно было случиться).");
}
catch (DbUpdateException ex)
{
    // EF Core перехватит ошибку от PostgreSQL
    Console.WriteLine("Ошибка при сохранении! База данных отклонила запрос.");
    Console.WriteLine($"Сообщение от PostgreSQL: {ex.GetBaseException().Message}");
}

Console.WriteLine("\nНажмите любую клавишу для выхода...");
Console.ReadKey();
