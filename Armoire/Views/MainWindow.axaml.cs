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
using Avalonia.Markup.Xaml;

namespace Armoire.Views
{
    public partial class MainWindow : Window
    {
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
            if (point.Properties.IsRightButtonPressed || 
                ((e.Source.ToString() != "Avalonia.Controls.Panel") &&
                (e.Source.ToString() != "Avalonia.Controls.StackPanel")))
                return;
            
            if (WindowState == WindowState.Maximized || WindowState == WindowState.FullScreen)
                return;

            _mouseDownForWindowMoving = true;
            _originalPoint = e.GetCurrentPoint(this);
        }

        private void InputElement_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            _mouseDownForWindowMoving = false;
        }
    }
}
