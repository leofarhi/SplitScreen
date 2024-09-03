using System;
using Newtonsoft.Json.Linq;
using Win32.DwmThumbnail.Interop;

namespace Win32.DwmThumbnail
{
    public abstract class ScreenCapture
    {
        public abstract ScreenRect GetSourceRect(MainWindow mainWindow);
    }
    
    public class ScreenCaptureSliced : ScreenCapture
    {
        private int rows;
        private int columns;
        private int screenIndex;
        
        public ScreenCaptureSliced(JToken jToken)
        {
            try
            {
                rows = int.Parse(jToken["rows"].ToString());
                columns = int.Parse(jToken["columns"].ToString());
                screenIndex = int.Parse(jToken["screen_index"].ToString());
                rows = rows < 1 ? 1 : rows;
                columns = columns < 1 ? 1 : columns;
                screenIndex = screenIndex < 0 ? 0 : screenIndex;
            }
            catch (Exception e)
            {
                rows = 1;
                columns = 1;
                screenIndex = 0;
            }
        }
        
        public override ScreenRect GetSourceRect(MainWindow mainWindow)
        {
            RECT rect = mainWindow.GetWindowRectangle();
            var sizeThumbnail = new IntVector2(rect.right - rect.left, rect.bottom - rect.top);
            ScreenRect screenRect = new ScreenRect();
            screenRect.Width = sizeThumbnail.Item1 / columns;
            screenRect.Height = sizeThumbnail.Item2 / rows;
            screenRect.X = screenRect.Width * (screenIndex % columns);
            screenRect.Y = screenRect.Height * (screenIndex / columns);
            return screenRect;
        }
    }
    
    public class ScreenCaptureCropped : ScreenCapture
    {
        private ScreenRect screenRect;
        
        public ScreenCaptureCropped(JToken jToken)
        {
            try
            {
                screenRect = new ScreenRect(
                    int.Parse(jToken["coordinates"][0].ToString()),
                    int.Parse(jToken["coordinates"][1].ToString()),
                    int.Parse(jToken["coordinates"][2].ToString()),
                    int.Parse(jToken["coordinates"][3].ToString())
                );
            }
            catch (Exception e)
            {
                screenRect = new ScreenRect();
            }
        }
        
        public override ScreenRect GetSourceRect(MainWindow mainWindow)
        {
            return screenRect;
        }
    }
    
    public class ScreenCaptureFull : ScreenCapture
    {
        public ScreenCaptureFull()
        {
            
        }

        public override ScreenRect GetSourceRect(MainWindow mainWindow)
        {
            RECT rect = mainWindow.GetWindowRectangle();
            var sizeThumbnail = new IntVector2(rect.right - rect.left, rect.bottom - rect.top);
            ScreenRect screenRect = new ScreenRect();
            screenRect.Width = sizeThumbnail.Item1;
            screenRect.Height = sizeThumbnail.Item2;
            return screenRect;
        }
    }
}