using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;

class DatabaseManager
{
    private string _connectionString;
    
    public DatabaseManager(string dbPath)
    {
        _connectionString = $"Data Source={dbPath}";
        CreateTables();
        LoadData();
    }
    
    private void CreateTables()
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS publisher (
                publisher_id INTEGER PRIMARY KEY AUTOINCREMENT,
                publisher_name TEXT NOT NULL
            );
            CREATE TABLE IF NOT EXISTS magazine (
                magazine_id INTEGER PRIMARY KEY AUTOINCREMENT,
                publisher_id INTEGER NOT NULL,
                magazine_name TEXT NOT NULL,
                circulation_k INTEGER NOT NULL
            );";
        cmd.ExecuteNonQuery();
    }
    
    private void LoadData()
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        
        var checkCmd = conn.CreateCommand();
        checkCmd.CommandText = "SELECT COUNT(*) FROM publisher";
        long count = Convert.ToInt64(checkCmd.ExecuteScalar());
        
        if (count == 0)
        {
            string publishersFile = "Data/publishers.csv";
            if (File.Exists(publishersFile))
            {
                var lines = File.ReadAllLines(publishersFile);
                for (int i = 1; i < lines.Length; i++)
                {
                    var parts = lines[i].Split(';');
                    if (parts.Length >= 2)
                    {
                        var cmd = conn.CreateCommand();
                        cmd.CommandText = "INSERT INTO publisher (publisher_id, publisher_name) VALUES (@id, @name)";
                        cmd.Parameters.AddWithValue("@id", int.Parse(parts[0]));
                        cmd.Parameters.AddWithValue("@name", parts[1]);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            
            string magazinesFile = "Data/magazines.csv";
            if (File.Exists(magazinesFile))
            {
                var lines = File.ReadAllLines(magazinesFile);
                for (int i = 1; i < lines.Length; i++)
                {
                    var parts = lines[i].Split(';');
                    if (parts.Length >= 4)
                    {
                        var cmd = conn.CreateCommand();
                        cmd.CommandText = "INSERT INTO magazine (magazine_id, publisher_id, magazine_name, circulation_k) VALUES (@id, @pubId, @name, @circ)";
                        cmd.Parameters.AddWithValue("@id", int.Parse(parts[0]));
                        cmd.Parameters.AddWithValue("@pubId", int.Parse(parts[1]));
                        cmd.Parameters.AddWithValue("@name", parts[2]);
                        cmd.Parameters.AddWithValue("@circ", int.Parse(parts[3]));
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
    }
    
    public List<Publisher> GetAllPublishers()
    {
        var result = new List<Publisher>();
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT publisher_id, publisher_name FROM publisher";
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            int id = reader.GetInt32(0);
            string name = reader.GetString(1);
            result.Add(new Publisher(id, name));
        }
        return result;
    }
    
    public List<Magazine> GetAllMagazines()
    {
        var result = new List<Magazine>();
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT magazine_id, publisher_id, magazine_name, circulation_k FROM magazine";
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            int id = reader.GetInt32(0);
            int pubId = reader.GetInt32(1);
            string name = reader.GetString(2);
            int circ = reader.GetInt32(3);
            result.Add(new Magazine(id, pubId, name, circ));
        }
        return result;
    }
    
    public Magazine? GetMagazineById(int id)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT magazine_id, publisher_id, magazine_name, circulation_k FROM magazine WHERE magazine_id = @id";
        cmd.Parameters.AddWithValue("@id", id);
        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            int mid = reader.GetInt32(0);
            int pubId = reader.GetInt32(1);
            string name = reader.GetString(2);
            int circ = reader.GetInt32(3);
            return new Magazine(mid, pubId, name, circ);
        }
        return null;
    }
    
    public void AddMagazine(Magazine mag)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "INSERT INTO magazine (publisher_id, magazine_name, circulation_k) VALUES (@pubId, @name, @circ)";
        cmd.Parameters.AddWithValue("@pubId", mag.PublisherId);
        cmd.Parameters.AddWithValue("@name", mag.Name);
        cmd.Parameters.AddWithValue("@circ", mag.CirculationK);
        cmd.ExecuteNonQuery();
    }
    
    public void UpdateMagazine(Magazine mag)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "UPDATE magazine SET publisher_id = @pubId, magazine_name = @name, circulation_k = @circ WHERE magazine_id = @id";
        cmd.Parameters.AddWithValue("@id", mag.Id);
        cmd.Parameters.AddWithValue("@pubId", mag.PublisherId);
        cmd.Parameters.AddWithValue("@name", mag.Name);
        cmd.Parameters.AddWithValue("@circ", mag.CirculationK);
        cmd.ExecuteNonQuery();
    }
    
    public void DeleteMagazine(int id)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "DELETE FROM magazine WHERE magazine_id = @id";
        cmd.Parameters.AddWithValue("@id", id);
        cmd.ExecuteNonQuery();
    }
    
    public List<Magazine> GetMagazinesByPublisher(int publisherId)
    {
        var result = new List<Magazine>();
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT magazine_id, publisher_id, magazine_name, circulation_k FROM magazine WHERE publisher_id = @pubId";
        cmd.Parameters.AddWithValue("@pubId", publisherId);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            int id = reader.GetInt32(0);
            int pubId = reader.GetInt32(1);
            string name = reader.GetString(2);
            int circ = reader.GetInt32(3);
            result.Add(new Magazine(id, pubId, name, circ));
        }
        return result;
    }
    
    public (string[] columns, List<string[]> rows) ExecuteQuery(string sql)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = sql;
        using var reader = cmd.ExecuteReader();
        
        string[] columns = new string[reader.FieldCount];
        for (int i = 0; i < reader.FieldCount; i++)
        {
            columns[i] = reader.GetName(i);
        }
        
        var rows = new List<string[]>();
        while (reader.Read())
        {
            string[] row = new string[reader.FieldCount];
            for (int i = 0; i < reader.FieldCount; i++)
            {
                object? value = reader.GetValue(i);
                row[i] = value?.ToString() ?? "";
            }
            rows.Add(row);
        }
        return (columns, rows);
    }
}
