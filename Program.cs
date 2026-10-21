using FitnessEf;
using Microsoft.EntityFrameworkCore;

// 11.1. Базовая обработка ошибок (try-catch)
try
{
    await using var db = new FitnessContext();

    // 11.4. Диагностика SQL (ToQueryString)
    Console.WriteLine("=== 11.4. Диагностика SQL ===");

    // Создаем запрос, но пока НЕ выполняем его (IQueryable)
    var query = db.Clients
        .Where(c => c.Status == "active")
        .OrderBy(c => c.LastName);

    Console.WriteLine("EF Core сгенерировал следующий SQL для базы данных:");
    Console.WriteLine(query.ToQueryString()); // Выводим SQL-код в консоль

    // 11.2. Асинхронное выполнение и 11.3. Сравнение tracking / no-tracking
    Console.WriteLine("\n=== 11.2 и 11.3. Выполнение запроса (Async) ===");

    // Только чтение: AsNoTracking() экономит память
    var activeClients = await query.AsNoTracking().ToListAsync();
    Console.WriteLine($"Найдено активных клиентов: {activeClients.Count}");

    // Изменение: объект отслеживается контекстом (без AsNoTracking)
    var clientToUpdate = await db.Clients.FirstOrDefaultAsync(c => c.ClientId == 1);
    if (clientToUpdate is not null)
    {
        clientToUpdate.Status = "inactive"; // EF Core "видит" это изменение
        await db.SaveChangesAsync();        // Асинхронно сохраняем
        Console.WriteLine("Клиенту с ID 1 изменен статус на 'inactive'.");
    }
}
catch (InvalidOperationException ex)
{
    // Ошибка конфигурации (например, не задана строка подключения)
    Console.WriteLine($"Ошибка конфигурации: {ex.Message}");
}
catch (DbUpdateException ex)
{
    // Ошибка при сохранении (например, нарушены ограничения БД)
    Console.WriteLine("Ошибка сохранения данных в БД.");
    Console.WriteLine($"Причина: {ex.GetBaseException().Message}");
}
catch (Exception ex)
{
    // Общий перехват (например, если упал PostgreSQL)
    Console.WriteLine("Произошла непредвиденная ошибка.");
    Console.WriteLine(ex.Message);
}

Console.WriteLine("\nНажмите любую клавишу для выхода...");
Console.ReadKey();