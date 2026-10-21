using FitnessEf;
using Microsoft.EntityFrameworkCore;

await using var db = new FitnessContext();

Console.WriteLine("=== Задание 6. Добавление нового клиента ===");

// 6.2. Проверка пользовательского ввода (Пункт 6.2)
Console.Write("Введите имя: ");
string firstName = Console.ReadLine()?.Trim() ?? string.Empty;

Console.Write("Введите фамилию: ");
string lastName = Console.ReadLine()?.Trim() ?? string.Empty;

// Проверка на пустоту
if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
{
    Console.WriteLine("Ошибка: Имя и фамилия обязательны для заполнения.");
    return; // Завершаем программу, если данные неверные
}

// Создаем объект клиента (Пункт 6.1)
var client = new Client
{
    FirstName = firstName,
    LastName = lastName,
    BirthDate = new DateOnly(2000, 5, 15), // Пример даты
    Phone = "79995554433",
    Email = "sokolov@example.com",
    RegistrationDate = DateOnly.FromDateTime(DateTime.Today),
    Status = "active"
};

// 6.3. Обработка DbUpdateException (Пункт 6.3)
try
{
    // Добавляем в контекст
    db.Clients.Add(client);

    // Сохраняем изменения в БД
    int saved = await db.SaveChangesAsync();

    Console.WriteLine($"\nУспешно сохранено изменений: {saved}");

    // ID генерируется базой данных (ValueGeneratedOnAdd), 
    // поэтому оно появится только ПОСЛЕ сохранения.
    Console.WriteLine($"ID нового клиента в базе: {client.ClientId}");
}
catch (DbUpdateException ex)
{
    Console.WriteLine("\nНе удалось сохранить клиента. Проверьте данные и ограничения БД.");
    // GetBaseException() достает суть ошибки из недр EF Core (например, нарушение NOT NULL)
    Console.WriteLine($"Причина: {ex.GetBaseException().Message}");
}

Console.WriteLine("\nНажмите любую клавишу для выхода...");
Console.ReadKey();