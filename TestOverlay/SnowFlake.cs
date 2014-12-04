using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TestOverlay
{
    public class SnowFlake
    {
        public Ellipse flake;
        public int x;
        public int y;
        public double fallspeed;

        public SnowFlake(float size, int setX, double fallSpeed)
        {
            SolidColorBrush brush = new SolidColorBrush(Colors.White);
            Pen pen = new Pen(brush, size);
            y = -100;
            x = setX;
            flake = new Ellipse();
            flake.Fill = brush;
            flake.StrokeThickness = 0.6;
            flake.Stroke = new SolidColorBrush(Colors.Black);
            flake.Width = size;
            flake.Height = size;
            fallspeed = fallSpeed;
            
        }
    }
}
