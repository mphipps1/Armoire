using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using Armoire.Models;
using Armoire.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using System.Threading;
using Avalonia.Markup.Xaml;

namespace Armoire.Views
{
    public partial class MainWindow : Window
    {

        public bool CtrlHeld;
        public MainWindow()
        {
            InitializeComponent();
        }

        //https://github.com/AvaloniaUI/Avalonia/discussions/8441
        //the following checks if the pointer is both pressed and moving, then updates the position of the window accordingly
        private bool _mouseDownForWindowMoving = false;
        private PointerPoint _originalPoint;

        private void InputElement_OnPointerMoved(object? sender, PointerEventArgs e)
        {
            var point = e.GetCurrentPoint(sender as Control);
            //Debug.WriteLine(e.Source.GetType());
            if (point.Properties.IsRightButtonPressed)
                return;
            if (!_mouseDownForWindowMoving)
                return;

            PointerPoint currentPoint = e.GetCurrentPoint(this);
            Position = new PixelPoint(Position.X + (int)(currentPoint.Position.X - _originalPoint.Position.X),
                Position.Y + (int)(currentPoint.Position.Y - _originalPoint.Position.Y));
        }

        private void InputElement_OnPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            var point = e.GetCurrentPoint(sender as Control);
            Debug.WriteLine("Mouse click on: " + e.Source.ToString());
            //Debug.WriteLine(sender.ToString());
            if (StartMenuItemView.popup != null && StartMenuItemView.popup.IsOpen)
                      StartMenuItemView.popup.IsOpen = false;
            if(NewItemView.popup != null && NewItemView.popup.IsOpen)
                NewItemView.popup.IsOpen = false;
            //add controls that we want to be draggable here using the source of what was pressed
            if (point.Properties.IsRightButtonPressed || 
                ((e.Source.ToString() != "Avalonia.Controls.Panel") &&
                (e.Source.ToString() != "Avalonia.Controls.StackPanel")&& 
                (e.Source.ToString() != "Avalonia.Controls.Border")))
                return;

            ContextMenu cm = this.Find<ContextMenu>("MainWindowContextMenu");
            if (cm.IsOpen)
                cm.Close();

            if (WindowState == WindowState.Maximized || WindowState == WindowState.FullScreen)
                return;

            _mouseDownForWindowMoving = true;
            _originalPoint = e.GetCurrentPoint(this);
        }

        private void InputElement_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            _mouseDownForWindowMoving = false;
        }


        private async void OpenFileDialogClick(object sender, RoutedEventArgs e)
        {
            var window = TopLevel.GetTopLevel(this) as Window;

            var FileSystemdialog = new OpenFileDialog
            {
                AllowMultiple = true
            };

            var result = await FileSystemdialog.ShowAsync(window);

        }

        private void MainWindow_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
                CtrlHeld = true;
            
        }

        private void MainWindow_KeyUp(object? sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Z:
                    if (CtrlHeld)
                    {
                        ContentsUnitViewModel.Undo();
                    }
                    break;
                case Key.LeftCtrl:
                    CtrlHeld = false;
                    break;
                case Key.RightCtrl:
                    CtrlHeld = false;
                    break;

                default:
                    break;
            }
        }

        public const Int32 MONITOR_DEFAULTTOPRIMERTY = 0x00000001;
        public const Int32 MONITOR_DEFAULTTONEAREST = 0x00000002;


        [DllImport("user32.dll")]
        public static extern IntPtr MonitorFromWindow(IntPtr handle, Int32 flags);


        [DllImport("user32.dll")]
        public static extern Boolean GetMonitorInfo(IntPtr hMonitor, NativeMonitorInfo lpmi);


        [Serializable, StructLayout(LayoutKind.Sequential)]
        public struct NativeRectangle
        {
            public Int32 Left;
            public Int32 Top;
            public Int32 Right;
            public Int32 Bottom;


            public NativeRectangle(Int32 left, Int32 top, Int32 right, Int32 bottom)
            {
                this.Left = left;
                this.Top = top;
                this.Right = right;
                this.Bottom = bottom;
            }
        }


        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public sealed class NativeMonitorInfo
        {
            public Int32 Size = Marshal.SizeOf(typeof(NativeMonitorInfo));
            public NativeRectangle Monitor;
            public NativeRectangle Work;
            public Int32 Flags;
        }

        public void CheckWindowPosition(object? sender, RoutedEventArgs args)
        {
            Window? ArmoireWindow = this.FindControl<Window>("window");
            if (ArmoireWindow is null)
                return;

            var primaryWindowWidth = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Width;
            
            if(primaryWindowWidth - 400 < ArmoireWindow.Position.X)
            {
                for (int i = 0; i < 400; i += 5)
                {
                    Position = new PixelPoint(ArmoireWindow.Position.X - 5, ArmoireWindow.Position.Y);
                    //Thread.Sleep(1);
                }
            }
        }
    }
}
