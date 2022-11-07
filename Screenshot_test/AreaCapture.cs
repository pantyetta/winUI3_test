using Microsoft.UI.Windowing;
using System.Diagnostics;
using System.Drawing;

namespace Screenshot_test
{
    internal class AreaCapture
    {
        MouseHook mouseHook = new MouseHook();
        Position area = null;

        public AreaCapture()
        {
            mouseHook.LButtonDownEvent += MouseHook_LButtonDownEvent;
            mouseHook.LButtonUpEvent += MouseHook_LButtonUpEvent;
        }
        private void MouseHook_LButtonDownEvent(object sender, MouseEventArg e)
        {
            area = new Position();
            area.setStartXY(e.Point.X, e.Point.Y);
        }

        private void MouseHook_LButtonUpEvent(object sender, MouseEventArg e)
        {
            mouseHook.HookEnd();
            area.setEndXY(e.Point.X, e.Point.Y);
            area.status();
            Crop();
        }

        public Bitmap ScreenCapture()
        {
            var primary = DisplayArea.Primary.OuterBounds;
            Bitmap bitmap = new Bitmap(primary.Width, primary.Height);
            Graphics graphics = Graphics.FromImage(bitmap);

            graphics.CopyFromScreen(0, 0, 0, 0, new Size(primary.Width, primary.Height));

            return bitmap;            
        }

        public void Crop()
        {
            string savePath = @"C:\Users\pancho\Desktop\";

            Bitmap bitmap = new Bitmap(area.width, area.height);
            Graphics graphics = Graphics.FromImage(bitmap);

            graphics.CopyFromScreen(area.getStartX(), area.getStartY(), 0, 0, new Size(area.width, area.height));
            bitmap.Save(savePath + "area.png", System.Drawing.Imaging.ImageFormat.Png);
        }

        public void Hook()
        {
            mouseHook.Hook();
        }
    }
    
    class Position
    {
        private int startX;
        private int startY;
        private int endX;
        private int endY;
        public int width;
        public int height;

        public void status()
        {
            Debug.WriteLine($"{startX}, {startY}, {endX}, {endY}, {width}, {height}");
        }

        public int getStartX()
        {
            return startX;
        }

        public int getStartY()
        {
            return startY;
        }

        public int getEndX()
        {
            return endX;
        }

        public int getEndY()
        {
            return endY;
        }

        public void setStartXY(int X, int Y)
        {
            startX = X;
            startY = Y;
        }

        public void setEndXY(int X, int Y)
        {
            endX = X;
            endY = Y;

            if(endX < startX)
                (endX, startX) = (startX, endX);
            if(endY < startY)
                (endY, startY) = (startY, endY);

            width = endX - startX;
            height = endY - startY;
        }

        public Position()
        {
            startX = 0;
            startY = 0;
            endX = 0;
            endY = 0;
            width = 0;
            height = 0;
        }
    }
}
