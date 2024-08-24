using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using Win32.Shared;

namespace Win32.DwmThumbnail
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            string[] args = Environment.GetCommandLineArgs();

            ConfigScreen configScreen = new ConfigScreen();
            
            if (args.Length > 1)
            {
                //args[1] = "16:9"
                string[] aspectRatio = args[1].Split(':');
                configScreen.aspectRatio = new Tuple<int, int>(int.Parse(aspectRatio[0]), int.Parse(aspectRatio[1]));
            }
            else
                configScreen.aspectRatio = new Tuple<int, int>(16, 9);
            if (args.Length > 2)
            {
                //args[2] = "0,0,0,0"
                string[] crop = args[2].Split(',');
                configScreen.crop = new ConfigScreen.Border(int.Parse(crop[0]), int.Parse(crop[1]), int.Parse(crop[2]), int.Parse(crop[3]));
            }
            else
                configScreen.crop = new ConfigScreen.Border(0, 0, 0, 0);
            var mainWindow = new MainWindow(1, IntPtr.Zero, configScreen);
            mainWindow.Show();
            var mainWindow2 = new MainWindow(2, mainWindow._hWnd, configScreen);
            mainWindow2.Show();
        }
    }
}
