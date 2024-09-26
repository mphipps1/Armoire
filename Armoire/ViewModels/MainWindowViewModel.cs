using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Armoire.Utils;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Data.Sqlite;

namespace Armoire.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        private int _drawerCount = 0;

        private const string DbTableName = "user";

        private const string DbPath = "hello.db";

        public ObservableCollection<DrawerContentsViewModel> DrawerContents { get; } = [];

        private long _dbLastRowId;

        [ObservableProperty]
        private string _headingDialogCol0 = "Add To Database";

        [ObservableProperty]
        private string _headingDialogCol1 = "Read From Database";

        [ObservableProperty]
        private string _headingDialogCol2 = "Log";

        [ObservableProperty]
        private string _textDialogCol1 = "Database Contents:";

        [ObservableProperty]
        private string _textDialogCol2 = "Log:";

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(HandleAddClickCommand))]
        private string? _username;

        [ObservableProperty]
        private string _headingMain = "Welcome to Armoire!";

        [ObservableProperty]
        private string _headingDock = "Dock";

        [ObservableProperty]
        private string _headingDockAlt = "Dock Alternate";

        private bool CanAddDrawer() => true;

        [RelayCommand(CanExecute = nameof(CanAddDrawer))]
        private async Task HandleAddClick()
        {
            await Task.Delay(TimeSpan.FromSeconds(3));
            DrawerContents.Add(new DrawerContentsViewModel() { Content = $"hi{_drawerCount++}" });
        }

        public MainWindowViewModel()
        {
            DrawerContents.Add(new DrawerContentsViewModel() { Content = "hi" });

            SQLitePCL.Batteries.Init();
            using var connection = new SqliteConnection("Data Source=" + DbPath);
            connection.Open();
            if (SqlUtils.TableAlreadyExists(connection, DbTableName))
                return;

            var command = connection.CreateCommand();
            command.CommandText =
                @"
            CREATE TABLE user (
                id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                name TEXT NOT NULL
            );

            INSERT INTO user
            VALUES (1, 'Brice'),
                   (2, 'Alexander'),
                   (3, 'Nate');
            ";
            command.ExecuteNonQuery();
            connection.Close();
        }

        private bool CanAddToDb() => !string.IsNullOrEmpty(Username);

        [RelayCommand]
        private async Task HandleDbAddClick()
        {
            await Task.Delay(TimeSpan.FromSeconds(3));
            AddToDb();
        }

        [RelayCommand]
        private void HandleDbReadClick()
        {
            ReadFromDb();
        }

        private void AppendToTextColumn2(string text)
        {
            TextDialogCol2 += $"\n{text}";
        }

        private void AppendToTextColumn1(string text)
        {
            TextDialogCol1 += $"\n{text}";
        }

        private void AddToDb()
        {
            using var connection = new SqliteConnection($"Data Source={DbPath}");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText =
                @"
            INSERT INTO user (name)
            VALUES ($name)
            ";
            command.Parameters.AddWithValue("$name", Username);
            command.ExecuteNonQuery();
            AppendToTextColumn2($"user with name {Username} inserted into {DbTableName}.");

            command.CommandText =
                @"
            SELECT last_insert_rowid()
            ";
            _dbLastRowId = (long)(command.ExecuteScalar() ?? 0);
            AppendToTextColumn2($"`_dbLastRowId` set to {_dbLastRowId}.");
            connection.Close();
        }

        private void ReadFromDb()
        {
            using var connection = new SqliteConnection($"Data Source={DbPath}");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText =
                @"
            SELECT name
            FROM user
            WHERE id = $id
            ";
            command.Parameters.AddWithValue("$id", _dbLastRowId);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var name = reader.GetString(0);

                AppendToTextColumn1($"Hi, {name}!");
                AppendToTextColumn2($"Read {name} from db.");
            }
            connection.Close();
        }
    }
}
