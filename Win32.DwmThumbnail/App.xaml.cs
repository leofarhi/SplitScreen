using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using Win32.Shared;

using Newtonsoft.Json.Linq;

namespace Win32.DwmThumbnail
{
    public partial class App : Application
    {
        public App()
        {
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: SplitScreen.exe <json file>");
                MessageBox.Show("Usage: SplitScreen.exe <json file>");
                Shutdown();
                return;
            }
            Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            IntPtr _hWnd = new WindowPicker().PickCaptureTarget(new WindowInteropHelper(Application.Current.MainWindow).Handle);
            Application.Current.ShutdownMode = ShutdownMode.OnLastWindowClose;
    
            if (_hWnd == IntPtr.Zero)
            {
                Console.WriteLine("No window selected or operation canceled.");
                MessageBox.Show("No window selected or operation canceled.");
                Shutdown();
                return;
            }
            try
            {
                string path = args[1];
                //check if file exists
                if (!System.IO.File.Exists(path))
                {
                    MessageBox.Show("File not found: " + path);
                    Shutdown();
                }
                string json = System.IO.File.ReadAllText(path);
                JObject jObject = JObject.Parse(json);
                //screenConfigs is list of ScreenConfig
                int screenCount = jObject["screenConfigs"].Count();
                ScreenConfig[] screenConfigs = new ScreenConfig[screenCount];
                for (int i = 0; i < screenCount; i++)
                    screenConfigs[i] = new ScreenConfig(jObject["screenConfigs"][i]);
                for (int i = 0; i < screenCount; i++)
                {
                    var mainWindow = new MainWindow(_hWnd, screenConfigs[i]);
                    mainWindow.Show();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                MessageBox.Show("Error: " + e.Message);
                throw;
            }
        }
    }
}
