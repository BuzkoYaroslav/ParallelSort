using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using library;
using System.Drawing.Drawing2D;

namespace library
{
    class GraphicPainter
    {
        private readonly double epsilan = Math.Pow(10, -2);

        private int minX = -10;
        private int maxX = 10;
        private int minY = -10;
        private int maxY = 10;

        private float axesWidth = 2;
        private Color axesColor = Color.Black;

        private class DrawingSettings
        {
            public object drawingObject;
            public Color borderColor;
            public float borderWidth;
            public SolidBrush brush;
        }

        private List<DrawingSettings> content;

        private PictureBox elementToDraw;
        private Bitmap bmp;
        private Graphics context;

        public GraphicPainter(PictureBox element)
        {
            content = new List<DrawingSettings>();
            DrawElement = element;
            DrawAxes();
        }
        public GraphicPainter()
        {
            content = new List<DrawingSettings>();
        }

        private double XDifference
        {
            get
            {
                return Convert.ToDouble(elementToDraw.Width) / (maxX - minX);
            }
        }
        private double YDifference
        {
            get
            {
                return Convert.ToDouble(elementToDraw.Height) / (maxY - minY);
            }
        }

        private void ReloadGraphicContext()
        {
            if (elementToDraw == null)
                return;

            bmp = new Bitmap(elementToDraw.Width, elementToDraw.Height);
            context = Graphics.FromImage(bmp);
        }
        private void RedrawContent()
        {
            if (context == null)
                return;

            context.Clear(Color.White);
            DrawAxes();

            foreach(DrawingSettings dr in content)
            {
                Draw(dr);
            }

            elementToDraw.Image = bmp;
        }
        private void DrawAxes()
        {
            if (context == null)
                return;

            float centerX = (float)FunctionXToScreenX(0);
            float centerY = (float)FunctionYToScreenY(0);

            context.DrawLine(new Pen(AxesColor, AxesWidth), centerX, 0, centerX, elementToDraw.Height);
            context.DrawLine(new Pen(AxesColor, AxesWidth), 0, centerY, elementToDraw.Width, centerY);

            context.DrawString("Y", new Font("Arial", 15), new SolidBrush(axesColor), new PointF(centerX + 10, 10));
            context.DrawString("X", new Font("Arial", 15), new SolidBrush(axesColor), new PointF(elementToDraw.Width - 30, centerY + 10));

            for (int i = minX; i <= maxX; i++)
                context.DrawLine(new Pen(AxesColor, AxesWidth), (float)FunctionXToScreenX(i), centerY - AxesWidth * 2,
                     (float)FunctionXToScreenX(i), centerY + AxesWidth * 2);
            for (int j = minY; j <= maxY; j++)
                context.DrawLine(new Pen(AxesColor, AxesWidth), centerX - AxesWidth * 2, (float)FunctionYToScreenY(j),
                        centerX + AxesWidth * 2, (float)FunctionYToScreenY(j));
        }
        private void Draw(DrawingSettings drSet)
        {
            if (context == null)
                return;

            if (drSet.drawingObject is MathFunction)
            {
                MathFunction func = drSet.drawingObject as MathFunction;
                Tuple<double, double>[] continuousRegions = func.ContinuousRegions(minX, maxX, minY, maxY);

                foreach (Tuple<double, double> region in continuousRegions)
                {
                    PointF[] points = PointsForFunction(func, region.Item1, region.Item2);

                    context.DrawLines(new Pen(drSet.borderColor, drSet.borderWidth), points);
                }
            }
            else if (drSet.drawingObject is GraphicsPath)
            {
                GraphicsPath path = drSet.drawingObject as GraphicsPath;

                context.FillPath(drSet.brush, path);
                context.DrawPath(new Pen(drSet.borderColor, drSet.borderWidth), path);
            }
        }
        private PointF[] PointsForFunction(MathFunction func, double a, double b)
        {
            int count = Convert.ToInt32((b - a) / epsilan) + 1;
            PointF[] points = new PointF[Convert.ToInt32((b - a) / epsilan) + 1];
            for (int i = 0; i < count; i++)
            {
                points[i] = new PointF((float)FunctionXToScreenX(a + i * epsilan), (float)FunctionYToScreenY(func.Calculate(a + i * epsilan)));
            }

            return points;
        }

        private void UpdateBounds(Point newBounds, ref int max, ref int min)
        {
            if (newBounds.X < newBounds.Y)
            {
                min = newBounds.X;
                max = newBounds.Y;
            }
        }

        private double FunctionXToScreenX(double x)
        {
            return (x - minX) * XDifference;
        }
        private double FunctionYToScreenY(double y)
        {
            return (maxY - y) * YDifference;
        }

        #region Public Interface

        public Color AxesColor
        {
            get
            {
                return axesColor;
            }
            set
            {
                axesColor = value;
                RedrawContent();
            }
        }
        public float AxesWidth
        {
            get
            {
                return axesWidth;
            }
            set
            {
               axesWidth = value;
               RedrawContent();
            }
        }
        
        public Point XBounds
        {
            set
            {
                UpdateBounds(value, ref maxX, ref minX);
                RedrawContent();
            }
            get
            {
                return new Point(minX, maxX);
            }
        }
        public Point YBounds
        {
            set
            {
                UpdateBounds(value, ref maxY, ref minY);
                RedrawContent();
            }
            get
            {
                return new Point(minY, maxY);
            }
        }

        public PictureBox DrawElement
        {
            set
            {
                elementToDraw = value;
                ReloadGraphicContext();

                RedrawContent();
            }
        }

        public void Draw(MathFunction func, Color funcColor, float width)
        {
            DrawingSettings drSet = new DrawingSettings();
            drSet.drawingObject = func;
            drSet.borderColor = funcColor;
            drSet.borderWidth = width;

            content.Add(drSet);

            Draw(drSet);

            if (elementToDraw != null)
                elementToDraw.Image = bmp;
        }
        public void DrawPath(GraphicsPath path, Color borderColor, float borderWidth, Color brushColor, double brushAlpha)
        {
            DrawingSettings drSet = new DrawingSettings();
            drSet.drawingObject = path;
            drSet.borderColor = borderColor;
            drSet.borderWidth = borderWidth;
            drSet.brush = new SolidBrush(Color.FromArgb(Convert.ToInt32(brushAlpha * 100), brushColor));

            content.Add(drSet);

            Draw(drSet);

            if (elementToDraw != null)
                elementToDraw.Image = bmp;
        }

        public void UpdateUI()
        {
            if (elementToDraw == null)
                return;

            elementToDraw.Image = bmp;
        }

        #endregion

        #region Paths for integral approximation methods graphical representation

        public GraphicsPath PathForFunction(MathFunction func, double a, double b)
        {
            GraphicsPath path = new GraphicsPath();

            path.StartFigure();
            path.AddLines(PointsForFunction(func, a, b));

            path.AddLine(new PointF((float)FunctionXToScreenX(b), (float)FunctionYToScreenY(func.Calculate(b))), 
                         new PointF((float)FunctionXToScreenX(b), (float)FunctionYToScreenY(0)));
            path.AddLine(new PointF((float)FunctionXToScreenX(b), (float)FunctionYToScreenY(0)),
                         new PointF((float)FunctionXToScreenX(a), (float)FunctionYToScreenY(0)));
            path.CloseFigure();

            return path;
        }
        public GraphicsPath PathForSimpsonMethod(MathFunction func, double a, double b, int n)
        {
            GraphicsPath path = new GraphicsPath();

            double difference = (b - a) / n;

            for (int i = 0; i < n; i++)
            {
                double x = a + difference * i;
                MathFunction simpsonFunc = FunctionForSimpsonInterval(new PointF((float)x, (float)func.Calculate(x)),
                                                                      new PointF((float)(x + difference / 2), (float)func.Calculate(x + difference / 2)),
                                                                      new PointF((float)(x + difference), (float)func.Calculate(x + difference)));
                path.AddPath(PathForFunction(simpsonFunc, x, x + difference), false);
            }

            return path;
        }

        private MathFunction FunctionForSimpsonInterval(PointF pnt1, PointF pnt2, PointF pnt3)
        {
            double a = (pnt3.Y - (pnt3.X * (pnt2.Y - pnt1.Y) + pnt2.X * pnt1.Y - pnt1.X * pnt2.Y) / (pnt2.X - pnt1.X)) /
                (pnt3.X * (pnt3.X - pnt2.X - pnt1.X) + pnt1.X * pnt2.X);
            double b = (pnt2.Y - pnt1.Y) / (pnt2.X - pnt1.X) - a * (pnt1.X + pnt2.X);
            double c = (pnt2.X * pnt1.Y - pnt1.X * pnt2.Y) / (pnt2.X - pnt1.X) + a * pnt1.X * pnt2.X;

            return a * new PowerFunction(1.0d, new XFunction(1.0d), 2) + b * new XFunction(1.0d) + c;
        }

        private const int dotHalfWidth = 5;
        public GraphicsPath PathForDots(KeyValuePair<double, double>[] dots)
        {
            GraphicsPath path = new GraphicsPath();

            foreach(KeyValuePair<double, double> pair in dots)
            {
                path.AddEllipse((float)FunctionXToScreenX(pair.Key) - dotHalfWidth,
                    (float)FunctionYToScreenY(pair.Value) - dotHalfWidth,
                    dotHalfWidth * 2, dotHalfWidth * 2);
            }

            return path;
        }

        public GraphicsPath DotsPathFromDots(PointF[] dots)
        {
            GraphicsPath path = new GraphicsPath();

            foreach (PointF point in dots)
            {
                path.AddEllipse((float)FunctionXToScreenX(point.X) - dotHalfWidth,
                    (float)FunctionYToScreenY(point.Y) - dotHalfWidth,
                    dotHalfWidth * 2, dotHalfWidth * 2);
            }

            return path;
        }
        public GraphicsPath LinePathFromDots(PointF[] dots)
        {
            GraphicsPath path = new GraphicsPath();
            PointF[] newDots = new PointF[dots.Length];

            for (int i = 0; i < dots.Length; i++)
                newDots[i] = new PointF((float)FunctionXToScreenX(dots[i].X), 
                    (float)FunctionYToScreenY(dots[i].Y));

            path.AddLines(newDots);

            return path;
        }

        #endregion
    }
}
