using System;
using System.Threading.Tasks;
using Armoire.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Armoire.ViewModels;

public partial class SqlDialogViewModel : ViewModelBase
{
    public SqlDialogViewModel() { }

    public SqlDialogViewModel(ISqlDialog sqlDialogModel)
    {
        _model = sqlDialogModel;
        _heading0 = sqlDialogModel.Heading0;
        _heading1 = sqlDialogModel.Heading1;
        _heading2 = sqlDialogModel.Heading2;
        _body1 = sqlDialogModel.Body1;
        _body2 = sqlDialogModel.Body2;
    }

    private readonly ISqlDialog _model;

    [ObservableProperty]
    private string _heading0;

    [ObservableProperty]
    private string _heading1;

    [ObservableProperty]
    private string _heading2;

    [ObservableProperty]
    private string _body1;

    [ObservableProperty]
    private string _body2;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(HandleDbAddClickCommand))]
    private string? _valueToAdd1;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(HandleDbAddClickCommand))]
    private string? _valueToAdd2;

    private bool CanAddToDb() =>
        !string.IsNullOrEmpty(ValueToAdd1) && !string.IsNullOrEmpty(ValueToAdd2);

    [RelayCommand(CanExecute = nameof(CanAddToDb))]
    private async Task HandleDbAddClick()
    {
        await Task.Delay(TimeSpan.FromSeconds(1));
        _model.ValueToAdd1 = ValueToAdd1;
        _model.ValueToAdd2 = ValueToAdd2;
        _model.AddToDb();
        Body2 = _model.Body2;
    }

    [RelayCommand]
    private void HandleDbReadClick()
    {
        _model.ReadFromDb();
        Body1 = _model.Body1;
        Body2 = _model.Body2;
    }
}
