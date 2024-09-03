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
    public class ScreenRect
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;
        
        public ScreenRect()
        {
            X = 0;
            Y = 0;
            Width = 0;
            Height = 0;
        }
        
        public ScreenRect(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
    }
    
    public class Border
    {   
        public int left;
        public int top;
        public int right;
        public int bottom;
        
        public Border()
        {
            left = 0;
            top = 0;
            right = 0;
            bottom = 0;
        }
            
        public Border(int left, int top, int right, int bottom)
        {
            this.left = left;
            this.top = top;
            this.right = right;
            this.bottom = bottom;
        }
    }
}