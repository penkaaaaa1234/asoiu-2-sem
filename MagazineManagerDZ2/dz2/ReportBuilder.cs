using System;
using System.Text;
using System.Collections.Generic;
using System.IO;

class ReportBuilder
{
    private DatabaseManager _db;
    private string _sql = "";
    private string _title = "";
    private string[] _headers = Array.Empty<string>();
    private int[] _widths = Array.Empty<int>();
    private bool _numbered = false;
    private string _footer = "";
    
    public ReportBuilder(DatabaseManager db)
    {
        _db = db;
    }
    
    public ReportBuilder Query(string sql)
    {
        _sql = sql;
        return this;
    }
    
    public ReportBuilder Title(string title)
    {
        _title = title;
        return this;
    }
    
    public ReportBuilder Header(params string[] columns)
    {
        _headers = columns;
        return this;
    }
    
    public ReportBuilder ColumnWidths(params int[] widths)
    {
        _widths = widths;
        return this;
    }
    
    public ReportBuilder Numbered()
    {
        _numbered = true;
        return this;
    }
    
    public ReportBuilder Footer(string label)
    {
        _footer = label;
        return this;
    }
    
    public string Build()
    {
        var (columns, rows) = _db.ExecuteQuery(_sql);
        var sb = new StringBuilder();
        
        if (_title.Length > 0)
        {
            sb.AppendLine();
            sb.AppendLine($"=== {_title} ===");
        }
        
        string[] displayHeaders = _headers.Length > 0 ? _headers : columns;
        int colCount = displayHeaders.Length;
        
        int[] widths;
        if (_widths.Length >= colCount)
            widths = _widths;
        else
        {
            widths = new int[colCount];
            for (int i = 0; i < colCount; i++) widths[i] = 20;
        }
        
        int numWidth = _numbered ? 5 : 0;
        
        if (_numbered) sb.Append("№".PadRight(numWidth));
        for (int i = 0; i < colCount; i++) sb.Append(displayHeaders[i].PadRight(widths[i]));
        sb.AppendLine();
        
        int totalWidth = numWidth;
        for (int i = 0; i < colCount; i++) totalWidth += widths[i];
        sb.AppendLine(new string('─', totalWidth));
        
        for (int r = 0; r < rows.Count; r++)
        {
            if (_numbered) sb.Append((r + 1).ToString().PadRight(numWidth));
            for (int c = 0; c < rows[r].Length && c < colCount; c++)
                sb.Append(rows[r][c].PadRight(widths[c]));
            sb.AppendLine();
        }
        
        if (_footer.Length > 0)
        {
            sb.AppendLine(new string('─', totalWidth));

            // determine if last displayed column contains numeric values and compute their sum
            double sum = 0;
            bool anyNumeric = false;
            int lastCol = Math.Max(0, colCount - 1);
            for (int r = 0; r < rows.Count; r++)
            {
                if (rows[r].Length <= lastCol) continue;
                var s = rows[r][lastCol]?.Trim();
                if (string.IsNullOrEmpty(s)) continue;
                s = s.Replace(',', '.');
                if (double.TryParse(s, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double v))
                {
                    sum += v;
                    anyNumeric = true;
                }
            }

            // build count text. If footer contains a format placeholder, use it;
            // if it already mentions "запис" (e.g. "Всего записей"), append the number after a colon;
            // otherwise append a default phrase with the word "записей".
            string countText;
            if (_footer.Contains("{0}"))
                countText = string.Format(_footer, rows.Count);
            else if (_footer.IndexOf("запис", StringComparison.OrdinalIgnoreCase) >= 0)
                countText = $"{_footer}: {rows.Count}";
            else
                countText = $"{_footer} {rows.Count} записей";

            if (anyNumeric)
            {
                string sumText = Math.Abs(sum - Math.Round(sum)) < 1e-9
                    ? ((long)Math.Round(sum)).ToString()
                    : sum.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture);

                sb.AppendLine($"{countText}: {sumText}");
            }
            else
            {
                sb.AppendLine(countText);
            }
        }
        
        return sb.ToString();
    }
    
    public void Print()
    {
        Console.Write(Build());
    }
    
    public void SaveToFile(string path)
    {
        File.WriteAllText(path, Build());
        Console.WriteLine($"Отчёт сохранён в файл: {path}");
    }
}
