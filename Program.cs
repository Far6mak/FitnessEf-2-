using FitnessEf;

await using var db = new FitnessContext();
var canConnect = await db.Database.CanConnectAsync();
Console.WriteLine(canConnect ? "Соединение с PostgreSQL установлено." : "PostgreSQL недоступен.");