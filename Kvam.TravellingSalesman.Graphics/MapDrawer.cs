using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;

namespace Kvam.TravellingSalesman.Graphics
{
    public class MapDrawer
    {
        public static int Counter = 0;
        private readonly float _canvasSize;
        private readonly float _frameWidthX;
        private readonly float _frameWidthY;

        public MapDrawer(string uri, IEnumerable<Point> points, int canvasSize)
        {
            var pointList = points.ToList();

            _canvasSize = canvasSize;
            _frameWidthX = _frameWidthY = 20;

            using (var bitmap = new Bitmap((int)(canvasSize + 2 * _frameWidthX), (int)(canvasSize + 2 * _frameWidthY)))
            {
                using (var canvas = System.Drawing.Graphics.FromImage(bitmap))
                {
                    canvas.Clear(Color.GhostWhite);
                    Draw(canvas, pointList);
                }
                bitmap.Save(string.Format(uri, Counter.ToString("D2")), ImageFormat.Png);
            }
            ++Counter;
        }

        private void Draw(System.Drawing.Graphics canvas, List<Point> pointList)
        {
            float minX = pointList.Min(x => x.X),
                  maxX = pointList.Max(x => x.X),
                  minY = pointList.Min(x => x.Y),
                  maxY = pointList.Max(x => x.Y);
            const int circleRadius = 4;
            var circleBrush = Brushes.BlueViolet;
            var linePen = new Pen(Color.Green, width: 2);


            var canvasPoints = new List<Point>();
            foreach (var point in pointList)
            {
                var x = (int)(_frameWidthX + _canvasSize * ((point.X - minX) / (maxX - minX)));
                var y = (int)(_frameWidthY + _canvasSize * ((point.Y - minY) / (maxY - minY)));

                canvasPoints.Add(new Point(x, y));
            }

            for (int i = 0; i < canvasPoints.Count; ++i)
            {
                canvas.FillEllipse(circleBrush,
                                   canvasPoints[i].X,
                                   canvasPoints[i].Y,
                                   circleRadius * 2,
                                   circleRadius * 2);

                canvas.DrawLine(linePen,
                                new Point
                                {
                                    X = canvasPoints[i].X + circleRadius,
                                    Y = canvasPoints[i].Y + circleRadius
                                },
                                new Point
                                {
                                    X = canvasPoints[(i + 1) % canvasPoints.Count].X + circleRadius,
                                    Y = canvasPoints[(i + 1) % canvasPoints.Count].Y + circleRadius
                                });
            }
        }
    }
}
