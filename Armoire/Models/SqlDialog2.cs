using Armoire.Utils;
using Microsoft.Data.Sqlite;

namespace Armoire.Models;

public class SqlDialog2 : ISqlDialog
{
    public string Heading0 { get; set; }
    public string Heading1 { get; set; }
    public string Heading2 { get; set; }
    public string Body1 { get; set; }
    public string Body2 { get; set; }
    public long LastRowId { get; set; }
    public string? ValueToAdd1 { get; set; }
    public string? ValueToAdd2 { get; set; }

    public SqlDialog2()
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

    public void AppendToBody1(string str)
    {
        Body1 += $"\n{str}";
    }

    public void AppendToBody2(string str)
    {
        Body2 += $"\n{str}";
    }

    public void AddToDb()
    {
        throw new System.NotImplementedException();
    }

    public void ReadFromDb()
    {
        throw new System.NotImplementedException();
    }
}
