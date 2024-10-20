using Armoire.Interfaces;
using Armoire.Utils;
using Microsoft.Data.Sqlite;

namespace Armoire.Models;

public class SqlDialog : ISqlDialog
{
    public SqlDialog()
    {
        SQLitePCL.Batteries.Init();
        using var connection = new SqliteConnection("Data Source=" + SqlUtils.DbPath);
        connection.Open();
        if (SqlUtils.TableAlreadyExists(connection, "user"))
            return;

        var command = connection.CreateCommand();
        command.CommandText = """
                        CREATE TABLE user (
                            id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                            name TEXT NOT NULL,
                            age INTEGER NOT NULL
                        );

                        INSERT INTO user
                        VALUES (1, 'Brice', 33),
                               (2, 'Alexander', 44),
                               (3, 'Nate', 22);
            """;
        command.ExecuteNonQuery();
        connection.Close();
    }

    public void AddToDb()
    {
        using var connection = new SqliteConnection($"Data Source={SqlUtils.DbPath}");
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = """
                        INSERT INTO user (name, age)
                        VALUES ($name, $age)
            """;
        command.Parameters.AddWithValue("$name", ValueToAdd1);
        command.Parameters.AddWithValue("$age", ValueToAdd2);
        command.ExecuteNonQuery();
        AppendToBody2($"user with name {ValueToAdd1} and age {ValueToAdd2} inserted into user.");

        command.CommandText = "SELECT last_insert_rowid()";
        LastRowId = (long)(command.ExecuteScalar() ?? 0);
        AppendToBody2($"`LastRowId` set to {LastRowId}.");
        connection.Close();
    }

    public void ReadFromDb()
    {
        using var connection = new SqliteConnection($"Data Source={SqlUtils.DbPath}");
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = """
                        SELECT name
                        FROM user
                        WHERE id = $id
            """;
        command.Parameters.AddWithValue("$id", LastRowId);

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            var name = reader.GetString(0);

            AppendToBody1($"Hi, {name}!");
            AppendToBody2($"Read {name} from db.");
        }
        connection.Close();
    }

    public string Heading0 { get; set; } = "Add To Database";
    public string Heading1 { get; set; } = "Read From Database";
    public string Heading2 { get; set; } = "Log";
    public string Body1 { get; set; } = "Database Contents:";
    public string Body2 { get; set; } = "Log:";
    public long LastRowId { get; set; }
    public string? ValueToAdd1 { get; set; }
    public string? ValueToAdd2 { get; set; }

    public void AppendToBody1(string str)
    {
        Body1 += $"\n{str}";
    }

    public void AppendToBody2(string str)
    {
        Body2 += $"\n{str}";
    }
}
