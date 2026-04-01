using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics; 
using Serilog;
using Serilog.Formatting.Json;

class TaskItem
{
    public string Title { get; set; }

    public TaskItem(string title)
    {
        Title = title;
    }
}

class TaskManager
{
    private readonly List<TaskItem> tasks = new List<TaskItem>();

    public void AddTask(string title)
    {
        var stopwatch = Stopwatch.StartNew(); 
        Log.Debug("Начало операции AddTask");

        if (string.IsNullOrWhiteSpace(title))
        {
            stopwatch.Stop();
            Log.Warning("Пользователь ввёл пустое название.");
            Log.Debug("Конец AddTask. Результат: неудача. Время: {Time} ms", stopwatch.ElapsedMilliseconds);
            return;
        }

        tasks.Add(new TaskItem(title));

        stopwatch.Stop();
        Log.Information("Задача \"{Title}\" успешно добавлена.", title);
        Log.Information("Количество задач: {Count}", tasks.Count);
        Log.Debug("Конец AddTask. Результат: успех. Время: {Time} ms", stopwatch.ElapsedMilliseconds);
    }

    public void RemoveTask(string title)
    {
        var stopwatch = Stopwatch.StartNew();
        Log.Debug("Начало операции RemoveTask");

        if (string.IsNullOrWhiteSpace(title))
        {
            stopwatch.Stop();
            Log.Warning("Пользователь ввёл пустое название.");
            Log.Debug("Конец RemoveTask. Результат: неудача. Время: {Time} ms", stopwatch.ElapsedMilliseconds);
            return;
        }

        var task = tasks.FirstOrDefault(t => t.Title.Equals(title, StringComparison.OrdinalIgnoreCase));

        if (task == null)
        {
            stopwatch.Stop();
            Log.Error("Задача \"{Title}\" не найдена.", title);
            Log.Debug("Конец RemoveTask. Результат: ошибка. Время: {Time} ms", stopwatch.ElapsedMilliseconds);
            return;
        }

        tasks.Remove(task);

        stopwatch.Stop();
        Log.Information("Задача \"{Title}\" удалена.", title);
        Log.Information("Количество задач: {Count}", tasks.Count);
        Log.Debug("Конец RemoveTask. Результат: успех. Время: {Time} ms", stopwatch.ElapsedMilliseconds);
    }

    public void ListTasks()
    {
        var stopwatch = Stopwatch.StartNew();
        Log.Debug("Начало операции ListTasks");

        if (tasks.Count == 0)
        {
            stopwatch.Stop();
            Console.WriteLine("Список задач пуст.");
            Log.Information("Список задач пуст.");
            Log.Debug("Конец ListTasks. Результат: пусто. Время: {Time} ms", stopwatch.ElapsedMilliseconds);
            return;
        }

        Console.WriteLine("\nСписок задач:");
        for (int i = 0; i < tasks.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {tasks[i].Title}");
        }

        stopwatch.Stop();
        Log.Information("Выведено задач: {Count}", tasks.Count);
        Log.Debug("Конец ListTasks. Результат: успех. Время: {Time} ms", stopwatch.ElapsedMilliseconds);
    }
}

class Program
{
    static void Main()
    {
        ConfigureLogging();

        try
        {
            Log.Information("Приложение запущено.");
            Log.Debug("Инициализация приложения.");

            TaskManager manager = new TaskManager();

            while (true)
            {
                Console.WriteLine("\nКоманды: add, remove, list, exit");
                Console.Write("Введите команду: ");
                string command = Console.ReadLine()?.Trim().ToLower() ?? "";

                Log.Debug("Введена команда: {Command}", command);

                switch (command)
                {
                    case "add":
                        Console.Write("Название задачи: ");
                        string addTitle = Console.ReadLine() ?? "";
                        manager.AddTask(addTitle);
                        break;

                    case "remove":
                        Console.Write("Удалить задачу: ");
                        string removeTitle = Console.ReadLine() ?? "";
                        manager.RemoveTask(removeTitle);
                        break;

                    case "list":
                        manager.ListTasks();
                        break;

                    case "exit":
                        Log.Information("Приложение завершено.");
                        Log.Debug("Завершение Main.");
                        return;

                    default:
                        Console.WriteLine("Неизвестная команда.");
                        Log.Warning("Неизвестная команда: {Command}", command);
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Критическая ошибка приложения.");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    static void ConfigureLogging()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()

            .WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")


            .WriteTo.File(
                path: "logs\\taskmanager-.log",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7,
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")

            .WriteTo.File(
                new JsonFormatter(),
                path: "logs\\taskmanager-.json",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7)

            .CreateLogger();
    }
}