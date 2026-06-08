using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class Program
{
    static void Main()
    {
        var db = new DatabaseManager("magazines.db");
        
        string choice;
        do
        {
            Console.WriteLine("\n╔══════════════════════════════════════╗");
            Console.WriteLine("║       УПРАВЛЕНИЕ ЖУРНАЛАМИ           ║");
            Console.WriteLine("╠══════════════════════════════════════╣");
            Console.WriteLine("║ 1 — Показать все издательства        ║");
            Console.WriteLine("║ 2 — Показать все журналы             ║");
            Console.WriteLine("║ 3 — Добавить журнал                  ║");
            Console.WriteLine("║ 4 — Редактировать журнал             ║");
            Console.WriteLine("║ 5 — Удалить журнал                   ║");
            Console.WriteLine("║ 6 — Отчёты                           ║");
            Console.WriteLine("║ 7 — Фильтр по издательству           ║");
            Console.WriteLine("║ 8 — Экспорт в CSV                    ║");
            Console.WriteLine("║ 0 — Выход                            ║");
            Console.WriteLine("╚══════════════════════════════════════╝");
            Console.Write("Ваш выбор: ");
            choice = Console.ReadLine()?.Trim() ?? "";
            
            switch (choice)
            {
                case "1": ShowPublishers(db); break;
                case "2": ShowMagazines(db); break;
                case "3": AddMagazine(db); break;
                case "4": EditMagazine(db); break;
                case "5": DeleteMagazine(db); break;
                case "6": ReportsMenu(db); break;
                case "7": FilterByPublisher(db); break;
                case "8": ExportToCsv(db); break;
                case "0": Console.WriteLine("До свидания!"); break;
                default: Console.WriteLine("Неверный пункт меню."); break;
            }
        } while (choice != "0");
    }
    
    static void ShowPublishers(DatabaseManager db)
    {
        Console.WriteLine("\n--- Все издательства ---");
        foreach (var pub in db.GetAllPublishers())
            Console.WriteLine($"  {pub}");
        Console.WriteLine($"Итого: {db.GetAllPublishers().Count}");
    }
    
    static void ShowMagazines(DatabaseManager db)
    {
        Console.WriteLine("\n--- Все журналы ---");
        foreach (var mag in db.GetAllMagazines())
            Console.WriteLine($"  {mag}");
        Console.WriteLine($"Итого: {db.GetAllMagazines().Count}");
    }
    
    static void AddMagazine(DatabaseManager db)
    {
        Console.WriteLine("\n--- Добавление журнала ---");
        Console.WriteLine("Доступные издательства:");
        foreach (var pub in db.GetAllPublishers())
            Console.WriteLine($"  {pub}");
        
        Console.Write("ID издательства: ");
        if (!int.TryParse(Console.ReadLine(), out int pubId))
        {
            Console.WriteLine("Ошибка: введите целое число.");
            return;
        }
        
        Console.Write("Название журнала: ");
        string name = Console.ReadLine()?.Trim() ?? "";
        if (string.IsNullOrEmpty(name))
        {
            Console.WriteLine("Ошибка: название не может быть пустым.");
            return;
        }
        
        Console.Write("Тираж (тыс. экз.): ");
        if (!int.TryParse(Console.ReadLine(), out int circ))
        {
            Console.WriteLine("Ошибка: введите целое число.");
            return;
        }
        
        try
        {
            var mag = new Magazine(0, pubId, name, circ);
            db.AddMagazine(mag);
            Console.WriteLine("Журнал добавлен.");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }
    
    static void EditMagazine(DatabaseManager db)
    {
        Console.WriteLine("\n--- Редактирование журнала ---");
        Console.Write("Введите ID журнала: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Ошибка: введите целое число.");
            return;
        }
        
        var mag = db.GetMagazineById(id);
        if (mag == null)
        {
            Console.WriteLine($"Журнал с ID={id} не найден.");
            return;
        }
        
        Console.WriteLine($"Текущие данные: {mag}");
        Console.WriteLine("(нажмите Enter, чтобы оставить значение без изменений)");
        
        Console.Write($"Название [{mag.Name}]: ");
        string input = Console.ReadLine()?.Trim() ?? "";
        if (!string.IsNullOrEmpty(input)) mag.Name = input;
        
        Console.Write($"ID издательства [{mag.PublisherId}]: ");
        input = Console.ReadLine()?.Trim() ?? "";
        if (!string.IsNullOrEmpty(input) && int.TryParse(input, out int newPubId))
            mag.PublisherId = newPubId;
        
        Console.Write($"Тираж [{mag.CirculationK}]: ");
        input = Console.ReadLine()?.Trim() ?? "";
        if (!string.IsNullOrEmpty(input) && int.TryParse(input, out int newCirc))
        {
            try { mag.CirculationK = newCirc; }
            catch (ArgumentException ex) { Console.WriteLine($"Ошибка: {ex.Message}"); return; }
        }
        
        db.UpdateMagazine(mag);
        Console.WriteLine("Данные обновлены.");
    }
    
    static void DeleteMagazine(DatabaseManager db)
    {
        Console.WriteLine("\n--- Удаление журнала ---");
        Console.Write("Введите ID журнала: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Ошибка: введите целое число.");
            return;
        }
        
        var mag = db.GetMagazineById(id);
        if (mag == null)
        {
            Console.WriteLine($"Журнал с ID={id} не найден.");
            return;
        }
        
        Console.Write($"Удалить «{mag.Name}»? (да/нет): ");
        string confirm = Console.ReadLine()?.Trim().ToLower() ?? "";
        if (confirm == "да")
        {
            db.DeleteMagazine(id);
            Console.WriteLine("Журнал удалён.");
        }
        else Console.WriteLine("Удаление отменено.");
    }
    
    static void ReportsMenu(DatabaseManager db)
    {
        string choice;
        do
        {
            Console.WriteLine("\n--- Отчёты ---");
            Console.WriteLine(" 1 — Журналы по издательствам");
            Console.WriteLine(" 2 — Количество журналов по издательствам");
            Console.WriteLine(" 3 — Средний тираж по издательствам");
            Console.WriteLine(" 0 — Назад");
            Console.Write("Ваш выбор: ");
            choice = Console.ReadLine()?.Trim() ?? "";
            
            switch (choice)
            {
                case "1": 
                    new ReportBuilder(db)
                        .Query(@"SELECT m.magazine_name, p.publisher_name, m.circulation_k
                                 FROM magazine m
                                 JOIN publisher p ON m.publisher_id = p.publisher_id
                                 ORDER BY m.magazine_name")
                        .Title("Журналы по издательствам")
                        .Header("Название журнала", "Издательство", "Тираж (тыс.)")
                        .ColumnWidths(25, 20, 15)
                        .Numbered()
                        .Footer("Всего записей")
                        .Print();
                    break;
                case "2":
                    new ReportBuilder(db)
                        .Query(@"SELECT p.publisher_name, COUNT(*) AS cnt
                                 FROM magazine m
                                 JOIN publisher p ON m.publisher_id = p.publisher_id
                                 GROUP BY p.publisher_name
                                 ORDER BY p.publisher_name")
                        .Title("Количество журналов по издательствам")
                        .Header("Издательство", "Кол-во журналов")
                        .ColumnWidths(25, 15)
                        .Print();
                    break;
                case "3":
                    new ReportBuilder(db)
                        .Query(@"SELECT p.publisher_name, ROUND(AVG(m.circulation_k), 1) AS avg_circulation
                                 FROM magazine m
                                 JOIN publisher p ON m.publisher_id = p.publisher_id
                                 GROUP BY p.publisher_name
                                 ORDER BY avg_circulation DESC")
                        .Title("Средний тираж по издательствам")
                        .Header("Издательство", "Средний тираж (тыс.)")
                        .ColumnWidths(25, 20)
                        .Print();
                    break;
                case "0": break;
                default: Console.WriteLine("Неверный пункт."); break;
            }
        } while (choice != "0");
    }
    
    static void FilterByPublisher(DatabaseManager db)
    {
        Console.WriteLine("\n--- Фильтр по издательству ---");
        foreach (var pub in db.GetAllPublishers())
            Console.WriteLine($"  {pub}");
        
        Console.Write("Введите ID издательства: ");
        if (!int.TryParse(Console.ReadLine(), out int pubId))
        {
            Console.WriteLine("Ошибка: введите целое число.");
            return;
        }
        
        var magazines = db.GetMagazinesByPublisher(pubId);
        if (magazines.Count == 0)
        {
            Console.WriteLine("У этого издательства нет журналов.");
            return;
        }
        
        Console.WriteLine($"\nЖурналы издательства #{pubId}:");
        foreach (var mag in magazines)
            Console.WriteLine($"  {mag}");
        Console.WriteLine($"Итого: {magazines.Count}");
    }
    
    static void ExportToCsv(DatabaseManager db)
    {
        string publishersPath = "Data/publishers_export.csv";
        string magazinesPath = "Data/magazines_export.csv";
        
        var publishers = db.GetAllPublishers();
        var pubLines = new List<string> { "publisher_id;publisher_name" };
        pubLines.AddRange(publishers.Select(p => $"{p.Id};{p.Name}"));
        File.WriteAllLines(publishersPath, pubLines);
        
        var magazines = db.GetAllMagazines();
        var magLines = new List<string> { "magazine_id;publisher_id;magazine_name;circulation_k" };
        magLines.AddRange(magazines.Select(m => $"{m.Id};{m.PublisherId};{m.Name};{m.CirculationK}"));
        File.WriteAllLines(magazinesPath, magLines);
        
        Console.WriteLine($"Издательства экспортированы в: {publishersPath}");
        Console.WriteLine($"Журналы экспортированы в: {magazinesPath}");
    }
}
