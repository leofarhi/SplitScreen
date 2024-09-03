using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

using Win32.DwmThumbnail.Interop;
using Win32.Shared;

namespace Win32.DwmThumbnail
{
    public class PreviousWindowState
    {
        public WindowStyle WindowStyle;
        public ResizeMode ResizeMode;
        public double Width;
        public double Height;
        public WindowState WindowState;
        
        public PreviousWindowState(MainWindow mainWindow)
        {
            WindowStyle = mainWindow.WindowStyle;
            ResizeMode = mainWindow.ResizeMode;
            Width = mainWindow.Width;
            Height = mainWindow.Height;
            WindowState = mainWindow.WindowState;
        }
        
        public void Restore(MainWindow mainWindow)
        {
            mainWindow.WindowStyle = WindowStyle;
            mainWindow.ResizeMode = ResizeMode;
            mainWindow.Width = Width;
            mainWindow.Height = Height;
            mainWindow.WindowState = WindowState;
        }
    }
    public partial class MainWindow : Window
    {
        private IntPtr _hThumbnail = IntPtr.Zero;
        private IntPtr _hWnd = IntPtr.Zero;
        private ScreenConfig _screenConfig;
        
        private bool _isFullScreen = false;
        private PreviousWindowState _previousWindowState;
        
        public MainWindow(IntPtr hWnd, ScreenConfig screenConfig)
        {
            InitializeComponent();
            _hWnd = hWnd;
            _screenConfig = screenConfig;
            Loaded += OnLoaded;
            SizeChanged += OnSizeChanged;
            _previousWindowState = new PreviousWindowState(this);
            this.KeyDown += MainWindow_KeyDown;
        }
        
        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F11)
            {
                ToggleFullScreen();
                e.Handled = true; // Marquer l'événement comme traité
            }
        }

        private void ToggleFullScreen()
        {
            if (_isFullScreen)
                _previousWindowState.Restore(this);
            else
            {
                _previousWindowState = new PreviousWindowState(this);
                WindowStyle = WindowStyle.None;
                ResizeMode = ResizeMode.NoResize;
                WindowState = WindowState.Maximized;
            }
            _isFullScreen = !_isFullScreen;
        }
        
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var hr = NativeMethods.DwmRegisterThumbnail(new WindowInteropHelper(this).Handle, _hWnd, out _hThumbnail);
            if (hr != 0)
                return;
            _screenConfig.Init(this);
            UpdateProperties();
        }
        
        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_hThumbnail == IntPtr.Zero)
                return;
            UpdateProperties();
        }

        private void UpdateProperties()
        {
            _screenConfig.Update(this);
            var dpi = GetDpiScaleFactor();
            double gridWidth = Viewbox.ActualWidth * dpi.X;
            double gridHeight = Viewbox.ActualHeight * dpi.Y;
            int left = (int)(Grid.TranslatePoint(new Point(0, 0), this).X * dpi.X);
            int top = (int)(Grid.TranslatePoint(new Point(0, 0), this).Y * dpi.Y);
            int right = left + (int)gridWidth;
            int bottom = top + (int)gridHeight;
            RECT screenRect = _screenConfig.GetSourceRect(this);
            var props = new DWM_THUMBNAIL_PROPERTIES
            {
                fVisible = true,
                dwFlags = (int)(DWM_TNP.DWM_TNP_VISIBLE | DWM_TNP.DWM_TNP_OPACITY | DWM_TNP.DWM_TNP_RECTDESTINATION | DWM_TNP.DWM_TNP_SOURCECLIENTAREAONLY | DWM_TNP.DWM_TNP_RECTSOURCE),
                opacity = 255,
                rcDestination = new RECT
                {
                    left = left,
                    top = top,
                    right = right,
                    bottom = bottom
                },
                rcSource = new RECT
                {
                    left = screenRect.left,
                    top = screenRect.top,
                    right = screenRect.right,
                    bottom = screenRect.bottom
                },
                fSourceClientAreaOnly = true
            };

            NativeMethods.DwmUpdateThumbnailProperties(_hThumbnail, ref props);
        }
        
        public Point GetDpiScaleFactor()
        {
            var source = PresentationSource.FromVisual(this);
            return source?.CompositionTarget != null ? new Point(source.CompositionTarget.TransformToDevice.M11, source.CompositionTarget.TransformToDevice.M22) : new Point(1.0d, 1.0d);
        }
        
        public RECT GetWindowRectangle()
        {
            RECT rect;

            int size = Marshal.SizeOf(typeof(RECT));
            NativeMethods.DwmGetWindowAttribute(this._hWnd, DWMWINDOWATTRIBUTE.ExtendedFrameBounds, out rect, size);

            return rect;
        }
    }
}