using FitnessEf;
using Microsoft.EntityFrameworkCore;

while (true)
{
    Console.WriteLine("\n=== УПРАВЛЕНИЕ КЛИЕНТАМИ ФИТНЕС-КЛУБА ===");
    Console.WriteLine("1. Список клиентов (сортировка по Фамилии и Имени)");
    Console.WriteLine("2. Найти клиента по ID");
    Console.WriteLine("3. Поиск по фрагменту фамилии");
    Console.WriteLine("4. Добавить клиента");
    Console.WriteLine("5. Изменить телефон клиента");
    Console.WriteLine("6. Удалить клиента");
    Console.WriteLine("7. Статистика");
    Console.WriteLine("0. Выход");
    Console.Write("\nВыбор: ");

    string choice = Console.ReadLine() ?? string.Empty;

    switch (choice)
    {
        case "1": await ListClientsAsync(); break;
        case "2": await FindClientAsync(); break;
        case "3": await SearchByLastNameAsync(); break;
        case "4": await AddClientAsync(); break;
        case "5": await UpdatePhoneAsync(); break;
        case "6": await DeleteClientAsync(); break;
        case "7": await ShowStatisticsAsync(); break;
        case "0": return;
        default: Console.WriteLine("Неизвестный пункт меню. Попробуйте снова."); break;
    }
}

// --- МЕТОДЫ ---

async Task ListClientsAsync()
{
    // Короткоживущий контекст (создается и уничтожается в рамках метода)
    await using var db = new FitnessContext();
    var clients = await db.Clients
        .AsNoTracking()
        .OrderBy(c => c.LastName)
        .ThenBy(c => c.FirstName)
        .ToListAsync();

    Console.WriteLine("\n--- Список клиентов ---");
    foreach (var c in clients)
    {
        Console.WriteLine($"{c.ClientId}: {c.LastName} {c.FirstName} | Тел: {c.Phone ?? "не указан"} | Статус: {c.Status}");
    }
}

async Task FindClientAsync()
{
    Console.Write("Введите ID клиента: ");
    if (!int.TryParse(Console.ReadLine(), out int id))
    {
        Console.WriteLine("Ошибка: ID должен быть числом.");
        return;
    }

    await using var db = new FitnessContext();
    var client = await db.Clients.AsNoTracking().FirstOrDefaultAsync(c => c.ClientId == id);

    if (client is null)
        Console.WriteLine($"Клиент с ID {id} не найден.");
    else
        Console.WriteLine($"Найден: {client.FirstName} {client.LastName} (Статус: {client.Status}, Тел: {client.Phone ?? "нет"})");
}

async Task SearchByLastNameAsync()
{
    Console.Write("Введите фрагмент фамилии: ");
    string term = Console.ReadLine()?.Trim() ?? string.Empty;

    if (string.IsNullOrWhiteSpace(term))
    {
        Console.WriteLine("Строка поиска пуста.");
        return;
    }

    await using var db = new FitnessContext();
    var found = await db.Clients
        .AsNoTracking()
        .Where(c => c.LastName.Contains(term))
        .OrderBy(c => c.LastName)
        .ToListAsync();

    Console.WriteLine($"\nНайдено совпадений: {found.Count}");
    foreach (var c in found)
    {
        Console.WriteLine($"{c.ClientId}: {c.FirstName} {c.LastName}");
    }
}

async Task AddClientAsync()
{
    Console.Write("Имя: ");
    string firstName = Console.ReadLine()?.Trim() ?? string.Empty;
    Console.Write("Фамилия: ");
    string lastName = Console.ReadLine()?.Trim() ?? string.Empty;
    Console.Write("Телефон (необязательно): ");
    string phone = Console.ReadLine()?.Trim() ?? string.Empty;

    if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
    {
        Console.WriteLine("Ошибка: Имя и фамилия обязательны.");
        return;
    }

    var newClient = new Client
    {
        FirstName = firstName,
        LastName = lastName,
        Phone = string.IsNullOrWhiteSpace(phone) ? null : phone,
        RegistrationDate = DateOnly.FromDateTime(DateTime.Today),
        Status = "active"
    };

    try
    {
        await using var db = new FitnessContext();
        db.Clients.Add(newClient);
        await db.SaveChangesAsync();
        Console.WriteLine($"Успешно! Клиент добавлен с ID: {newClient.ClientId}");
    }
    catch (DbUpdateException ex)
    {
        Console.WriteLine($"Ошибка БД: {ex.GetBaseException().Message}");
    }
}

async Task UpdatePhoneAsync()
{
    Console.Write("Введите ID клиента: ");
    if (!int.TryParse(Console.ReadLine(), out int id)) return;

    await using var db = new FitnessContext();
    var client = await db.Clients.FindAsync(id);

    if (client is null)
    {
        Console.WriteLine("Клиент не найден.");
        return;
    }

    Console.Write($"Текущий телефон ({client.Phone ?? "нет"}). Введите новый: ");
    client.Phone = Console.ReadLine()?.Trim();

    await db.SaveChangesAsync();
    Console.WriteLine("Телефон успешно обновлен.");
}

async Task DeleteClientAsync()
{
    Console.Write("Введите ID клиента для удаления: ");
    if (!int.TryParse(Console.ReadLine(), out int id)) return;

    try
    {
        await using var db = new FitnessContext();
        var client = await db.Clients.FindAsync(id);

        if (client is null)
        {
            Console.WriteLine("Клиент не найден.");
            return;
        }

        db.Clients.Remove(client);
        await db.SaveChangesAsync();
        Console.WriteLine("Клиент успешно удален.");
    }
    catch (DbUpdateException ex)
    {
        // Здесь мы ловим ошибку внешнего ключа (Restrict), если у клиента есть посещения
        Console.WriteLine("\nНе удалось удалить клиента!");
        Console.WriteLine("Вероятно, у него есть связанные посещения (visits). Сначала удалите их.");
        // Console.WriteLine($"Техническая ошибка: {ex.GetBaseException().Message}");
    }
}

async Task ShowStatisticsAsync()
{
    await using var db = new FitnessContext();
    int total = await db.Clients.CountAsync();
    int active = await db.Clients.CountAsync(c => c.Status == "active");

    Console.WriteLine("\n--- Статистика ---");
    Console.WriteLine($"Всего клиентов: {total}");
    Console.WriteLine($"Активных клиентов: {active}");
}