using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

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
        Trace.WriteLine("[TRACE] Начало операции AddTask.");
        Trace.WriteLine($"[TRACE] Проверка введённого названия: \"{title}\"");

        if (string.IsNullOrWhiteSpace(title))
        {
            Trace.TraceWarning("[WARN] Пользователь ввёл пустое название. Операция add не выполнена.");
            Trace.WriteLine("[TRACE] Конец операции AddTask. Результат: неудача.");
            return;
        }

        tasks.Add(new TaskItem(title));
        Trace.TraceInformation($"[INFO] Задача \"{title}\" успешно добавлена.");
        Trace.TraceInformation($"[INFO] Количество задач: {tasks.Count}");
        Trace.WriteLine("[TRACE] Конец операции AddTask. Результат: успех.");
    }

    public void RemoveTask(string title)
    {
        Trace.WriteLine("[TRACE] Начало операции RemoveTask.");
        Trace.WriteLine($"[TRACE] Поиск задачи: \"{title}\"");

        if (string.IsNullOrWhiteSpace(title))
        {
            Trace.TraceWarning("[WARN] Пользователь ввёл пустое название. Операция remove не выполнена.");
            Trace.WriteLine("[TRACE] Конец операции RemoveTask. Результат: неудача.");
            return;
        }

        var task = tasks.FirstOrDefault(t => t.Title.Equals(title, StringComparison.OrdinalIgnoreCase));

        if (task == null)
        {
            Trace.TraceError($"[ERROR] Задача \"{title}\" не найдена.");
            Trace.WriteLine("[TRACE] Конец операции RemoveTask. Результат: неудача.");
            return;
        }

        tasks.Remove(task);
        Trace.TraceInformation($"[INFO] Задача \"{title}\" удалена.");
        Trace.TraceInformation($"[INFO] Количество задач: {tasks.Count}");
        Trace.WriteLine("[TRACE] Конец операции RemoveTask. Результат: успех.");
    }

    public void ListTasks()
    {
        Trace.WriteLine("[TRACE] Начало операции ListTasks.");

        if (tasks.Count == 0)
        {
            Console.WriteLine("Список задач пуст.");
            Trace.TraceInformation("[INFO] Список задач пуст.");
            Trace.WriteLine("[TRACE] Конец операции ListTasks. Результат: список пуст.");
            return;
        }

        Console.WriteLine("\nСписок задач:");
        for (int i = 0; i < tasks.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {tasks[i].Title}");
        }

        Trace.TraceInformation($"[INFO] Выведено задач: {tasks.Count}");
        Trace.WriteLine("[TRACE] Конец операции ListTasks. Результат: успех.");
    }
}

class Program
{
    static void Main()
    {
        try
        {
            ConfigureLogging();

            Trace.TraceInformation("[INFO] Приложение запущено.");
            Trace.WriteLine("[TRACE] Инициализация приложения.");

            TaskManager manager = new TaskManager();

            while (true)
            {
                Console.WriteLine("\nКоманды: add, remove, list, exit");
                Console.Write("Введите команду: ");
                string command = Console.ReadLine()?.Trim().ToLower() ?? "";

                Trace.WriteLine($"[TRACE] Введена команда: {command}");

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
                        Trace.TraceInformation("[INFO] Приложение завершено.");
                        Trace.WriteLine("[TRACE] Завершение Main.");
                        Trace.Flush();
                        return;

                    default:
                        Console.WriteLine("Неизвестная команда.");
                        Trace.TraceWarning($"[WARN] Неизвестная команда: {command}");
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            Trace.TraceError($"[ERROR] Необработанная ошибка: {ex.Message}");
            Trace.WriteLine("[CRITICAL] Критическая ошибка приложения. Завершение работы.");
            Trace.Flush();
        }
    }

    static void ConfigureLogging()
    {
        Trace.Listeners.Clear();

        Trace.Listeners.Add(new ConsoleTraceListener());

        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        string logPath = Path.Combine(desktopPath, "log.txt");

        Trace.Listeners.Add(new TextWriterTraceListener(logPath));

        Trace.AutoFlush = true;
    }
}