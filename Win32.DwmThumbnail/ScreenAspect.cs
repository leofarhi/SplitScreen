using System;
using System.Windows;
using Newtonsoft.Json.Linq;

namespace Win32.DwmThumbnail
{
    public abstract class ScreenAspect
    {
        public abstract void Update(MainWindow mainWindow);
    }
    
    public class ScreenAspectRatio : ScreenAspect
    {
        public Tuple<int,int> aspectRatio;
        
        public ScreenAspectRatio(JToken jToken)
        {
            try
            {
                string aspectRatioString = jToken["ratio"].ToString();
                string[] aspectRatioArray = aspectRatioString.Split(':');
                aspectRatio = new Tuple<int, int>(int.Parse(aspectRatioArray[0]), int.Parse(aspectRatioArray[1]));
            }
            catch (Exception e)
            {
                aspectRatio = new Tuple<int, int>(16, 9);
            }
        }
        
        public override void Update(MainWindow mainWindow)
        {
            mainWindow.Grid.Width = aspectRatio.Item1 * 50;
            mainWindow.Grid.Height = aspectRatio.Item2 * 50;
            mainWindow.UpdateLayout();
        }
    }

    public class ScreenAspectSize : ScreenAspect
    {
        Tuple<int, int> size;
        
        public ScreenAspectSize(JToken jToken)
        {
            try
            {
                string sizeString = jToken["size"].ToString();
                string[] sizeArray = sizeString.Split('x');
                size = new Tuple<int, int>(int.Parse(sizeArray[0]), int.Parse(sizeArray[1]));
            }
            catch (Exception e)
            {
                size = new Tuple<int, int>(1920, 1080);
            }
        }
        
        public override void Update(MainWindow mainWindow)
        {
            mainWindow.Grid.Width = size.Item1;
            mainWindow.Grid.Height = size.Item2;
            mainWindow.UpdateLayout();
        }
    }
}