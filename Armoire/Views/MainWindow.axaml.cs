using System;
using System.Diagnostics;
using System.Linq;
using Armoire.Models;
using Armoire.ViewModels;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Armoire.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public void FlyoutGate(object? sender, RoutedEventArgs args)
        {
            Debug.WriteLine("Reporting from MainWindow CodeBehind FlyoutGate. `sender`: " + sender);
            if (sender is not Button senderAsButton)
                return;

            if (senderAsButton.CommandParameter is not ContentsUnitViewModel cuVm)
            {
                Debug.WriteLine(
                    "Reporting from MainWindow CodeBehind FlyoutGate. `CommandParameter.Type`: "
                        + senderAsButton.CommandParameter?.GetType()
                );
                return;
            }

            // TODO: This is anti-MVVM because the view should be unaware of the model.
            Debug.WriteLine(
                "Reporting from MainWindow CodeBehind FlyoutGate. `CommandParameter.Model`: "
                    + cuVm.Model
            );

            if (cuVm.Model is DrawerAsContents)
                return;

            senderAsButton.Flyout?.Hide();
        }
    }
}
