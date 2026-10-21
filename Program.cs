using FitnessEf;
using Microsoft.EntityFrameworkCore;

await using var db = new FitnessContext();

// --- 7.1. Изменение отслеживаемого объекта ---
Console.WriteLine("=== 7.1. Изменение телефона клиента ===");
Console.Write("Введите ID клиента для изменения телефона (например, 1, 2 или 3): ");
if (int.TryParse(Console.ReadLine(), out int updateId))
{
    // FindAsync сначала ищет объект в памяти (Change Tracker), затем в БД
    var clientToUpdate = await db.Clients.FindAsync(updateId);

    if (clientToUpdate is null)
    {
        Console.WriteLine("Клиент с таким ID не найден.");
    }
    else
    {
        Console.WriteLine($"Текущий телефон клиента {clientToUpdate.FirstName}: {clientToUpdate.Phone ?? "не указан"}");

        // Меняем свойство. EF Core сразу же помечает объект как Modified.
        clientToUpdate.Phone = "79990001122";

        // Без этой строки изменения НЕ сохранятся в базу! (Пункт 7.3)
        await db.SaveChangesAsync();
        Console.WriteLine("Телефон успешно изменен.");
    }
}

// --- 7.2. Удаление ---
Console.WriteLine("\n=== 7.2. Удаление клиента ===");
Console.Write("Введите ID клиента для удаления (например, тот, что добавляли в Задании 6): ");
if (int.TryParse(Console.ReadLine(), out int deleteId))
{
    var clientToDelete = await db.Clients.FindAsync(deleteId);

    if (clientToDelete is null)
    {
        Console.WriteLine("Клиент с таким ID не найден.");
    }
    else
    {
        db.Clients.Remove(clientToDelete); // Помечаем на удаление
        await db.SaveChangesAsync();       // Выполняем DELETE
        Console.WriteLine($"Клиент {clientToDelete.FirstName} {clientToDelete.LastName} удален.");
    }
}

// --- 7.4. Несколько действий перед сохранением ---
Console.WriteLine("\n=== 7.4. Несколько действий за один SaveChanges ===");
// 1. Создаем нового клиента
db.Clients.Add(new Client
{
    FirstName = "Анна",
    LastName = "Смирнова",
    RegistrationDate = DateOnly.FromDateTime(DateTime.Today),
    Status = "active"
});

// 2. Находим существующего клиента с ID 2 и меняем его статус
var existing = await db.Clients.FindAsync(2);
if (existing is not null)
{
    existing.Status = "inactive";
}

// 3. Сохраняем ОБА изменения (и INSERT, и UPDATE) одной транзакцией
await db.SaveChangesAsync();
Console.WriteLine("Новый клиент добавлен, а клиенту с ID 2 изменен статус. Все сохранено одной командой.");

Console.WriteLine("\nНажмите любую клавишу для выхода...");
Console.ReadKey();