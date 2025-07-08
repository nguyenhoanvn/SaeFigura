using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FigureManagementSystem.Helpers
{
    public class FullScreenWindow : Window
    {
        public FullScreenWindow()
        {
            WindowStyle = WindowStyle.None;
            WindowState = WindowState.Maximized;
            ResizeMode = ResizeMode.NoResize;
            ShowInTaskbar = true;
        }
    }
}
