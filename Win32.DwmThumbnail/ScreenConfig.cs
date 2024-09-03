using System;
using System.Windows;
using Newtonsoft.Json.Linq;
using Win32.DwmThumbnail.Interop;

namespace Win32.DwmThumbnail
{
    public class ScreenConfig
    {
        private ScreenAspect screenRecord;
        private ScreenCapture screenCapture;
        private Border border;
        
        public ScreenConfig(JToken jToken)
        {
            switch (jToken["aspect_type"].ToString().ToLower())
            {
                case "ratio":
                    screenRecord = new ScreenAspectRatio(jToken);
                    break;
                case "size":
                    screenRecord = new ScreenAspectSize(jToken);
                    break;
                default:
                    screenRecord = null;
                    break;
            }
            
            switch (jToken["capture_type"].ToString().ToLower())
            {
                case "sliced":
                    screenCapture = new ScreenCaptureSliced(jToken);
                    break;
                case "cropped":
                    screenCapture = new ScreenCaptureCropped(jToken);
                    break;
                case "full":
                    screenCapture = new ScreenCaptureFull();
                    break;
                default:
                    screenCapture = null;
                    break;
            }
            try
            {
                border = new Border(
                    int.Parse(jToken["border"]["left"].ToString()),
                    int.Parse(jToken["border"]["top"].ToString()),
                    int.Parse(jToken["border"]["right"].ToString()),
                    int.Parse(jToken["border"]["bottom"].ToString())
                );
            }
            catch (Exception e)
            {
                border = new Border();
            }
        }
        
        public void Init(MainWindow mainWindow)
        {
            if (screenRecord == null)
                mainWindow.Close();
        }
        
        public RECT GetSourceRect(MainWindow mainWindow)
        {
            if (screenCapture == null)
                return new RECT();
            ScreenRect screenRect = screenCapture.GetSourceRect(mainWindow);
            screenRect.X += border.left;
            screenRect.Y += border.top;
            screenRect.Width -= border.left + border.right;
            screenRect.Height -= border.top + border.bottom;
            
            RECT rect = new RECT
            {
                left = screenRect.X,
                top = screenRect.Y,
                right = screenRect.X + screenRect.Width,
                bottom = screenRect.Y + screenRect.Height
            };
            return rect;
        }
        
        public void Update(MainWindow mainWindow)
        {
            if (screenRecord == null)
                return;
            screenRecord.Update(mainWindow);
        }
    }
}