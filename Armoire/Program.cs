using System;
using Avalonia;
using Avalonia.Controls;

namespace Armoire
{
    internal sealed class Program
    {
        private static Window? _programMainWindow;
        public static Window ProgramMainWindow
        {
            get => _programMainWindow ?? throw new InvalidOperationException();
            set => _programMainWindow = value;
        }

        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        public static void Main(string[] args) =>
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp() =>
            AppBuilder.Configure<App>().UsePlatformDetect().WithInterFont().LogToTrace();
    }
}
