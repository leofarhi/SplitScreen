using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

using Win32.DwmThumbnail.Interop;
using Win32.Shared;

namespace Win32.DwmThumbnail
{
    
    public class IntVector2
    {
        public int Item1;
        public int Item2;
        
        public IntVector2(int item1, int item2)
        {
            Item1 = item1;
            Item2 = item2;
        }
    }
    public class ConfigScreen
    {
        public class Border
        {   
            public int left;
            public int top;
            public int right;
            public int bottom;
            
            public Border(int left, int top, int right, int bottom)
            {
                this.left = left;
                this.top = top;
                this.right = right;
                this.bottom = bottom;
            }
        }
        public Tuple<int,int> aspectRatio;
        public Border crop = new Border(0, 0, 0, 0);
    }
    
    /// <summary>
    ///     MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private IntPtr _hThumbnail = IntPtr.Zero;
        public IntPtr _hWnd = IntPtr.Zero;
        private int _screenNumber;
        
        private bool _isFullScreen = false;
        private WindowStyle _previousWindowStyle;
        private ResizeMode _previousResizeMode;
        private double _previousWidth;
        private double _previousHeight;
        private WindowState _previousWindowState;
        private ConfigScreen configScreen;

        public MainWindow(int screenNumber, IntPtr hWnd, ConfigScreen configScreen)
        {
            this.configScreen = configScreen;
            _hWnd = hWnd;
            _screenNumber = screenNumber;
            InitializeComponent();
            Loaded += OnLoaded;
            SizeChanged += OnSizeChanged;
            SetAspectRatio();
            
            // Enregistrer les paramètres de la fenêtre initiale
            _previousWindowStyle = WindowStyle;
            _previousResizeMode = ResizeMode;
            _previousWidth = Width;
            _previousHeight = Height;
            _previousWindowState = WindowState;

            // Écouter les événements de touches
            this.KeyDown += MainWindow_KeyDown;
        }
        
        private void SetAspectRatio()
        {
            var aspectRatio = configScreen.aspectRatio;
            Grid.Width = aspectRatio.Item1 * 50;
            Grid.Height = aspectRatio.Item2 * 50;
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
            {
                // Rétablir le mode fenêtre
                WindowStyle = _previousWindowStyle;
                ResizeMode = _previousResizeMode;
                Width = _previousWidth;
                Height = _previousHeight;
                WindowState = _previousWindowState;
            }
            else
            {
                // Passer en plein écran
                _previousWindowStyle = WindowStyle;
                _previousResizeMode = ResizeMode;
                _previousWidth = Width;
                _previousHeight = Height;
                _previousWindowState = WindowState;

                WindowStyle = WindowStyle.None;
                ResizeMode = ResizeMode.NoResize;
                WindowState = WindowState.Maximized;
            }
            _isFullScreen = !_isFullScreen;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (_hWnd == IntPtr.Zero)
            {
                do
                {
                    _hWnd = new WindowPicker().PickCaptureTarget(new WindowInteropHelper(this).Handle);
                } while (_hWnd == IntPtr.Zero);
            }

            var hr = NativeMethods.DwmRegisterThumbnail(new WindowInteropHelper(this).Handle, _hWnd, out _hThumbnail);
            if (hr != 0)
                return;

            UpdateThumbnailProperties();
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_hThumbnail == IntPtr.Zero)
                return;
            UpdateThumbnailProperties();
        }
        
        private void UpdateThumbnailProperties()
        {
            var dpi = GetDpiScaleFactor();
    
            // Dimensions de la grille en pixels, en tenant compte des DPI
            double gridWidth = Viewbox.ActualWidth * dpi.X;
            double gridHeight = Viewbox.ActualHeight * dpi.Y;

            // Les coordonnées de la grille pour rcDestination
            /*int left = (int)(Grid.TranslatePoint(new Point(0, 0), this).X * dpi.X);
            int top = (int)(Grid.TranslatePoint(new Point(0, 0), this).Y * dpi.Y);
            int right = left + (int)gridWidth;
            int bottom = top + (int)gridHeight;*/
            
            int left = (int)(Grid.TranslatePoint(new Point(0, 0), this).X * dpi.X);
            int top = (int)(Grid.TranslatePoint(new Point(0, 0), this).Y * dpi.Y);
            int right = left + (int)gridWidth;
            int bottom = top + (int)gridHeight;
            
            RECT rect = GetWindowRectangle(_hWnd);
            var sizeThumbnail = new IntVector2(rect.right - rect.left, rect.bottom - rect.top);
            
            int newH = sizeThumbnail.Item2 - (configScreen.crop.top + configScreen.crop.bottom);
            
            int topsrc, bottomsrc;
            if (_screenNumber == 1)
            {
                topsrc = 0 + configScreen.crop.top;
                bottomsrc = newH/2 + configScreen.crop.top;
            }
            else
            {
                topsrc = newH/2 + configScreen.crop.top;
                bottomsrc = sizeThumbnail.Item2 - configScreen.crop.bottom;
            }

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
                    left = 0 + configScreen.crop.left,
                    top = topsrc,
                    right = sizeThumbnail.Item1 - configScreen.crop.right,
                    bottom = bottomsrc
                },
                fSourceClientAreaOnly = true
            };

            NativeMethods.DwmUpdateThumbnailProperties(_hThumbnail, ref props);
        }

        private Point GetDpiScaleFactor()
        {
            var source = PresentationSource.FromVisual(this);
            return source?.CompositionTarget != null ? new Point(source.CompositionTarget.TransformToDevice.M11, source.CompositionTarget.TransformToDevice.M22) : new Point(1.0d, 1.0d);
        }
        

        public static RECT GetWindowRectangle(IntPtr hWnd)
        {
            RECT rect;

            int size = Marshal.SizeOf(typeof(RECT));
            NativeMethods.DwmGetWindowAttribute(hWnd, DWMWINDOWATTRIBUTE.ExtendedFrameBounds, out rect, size);

            return rect;
        }
    }
}