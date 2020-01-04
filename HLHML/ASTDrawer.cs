using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HLHML
{
    public class ASTDrawer
    {
        private readonly AST _ast;
        private readonly Bitmap _bitmap;
        private readonly Graphics _graphics;
        private readonly SortedDictionary<float, SortedDictionary<float, BitmapEnhance>> _bitMaps;

        public ASTDrawer(AST ast)
        {
            _ast = ast;
            _bitMaps = new SortedDictionary<float, SortedDictionary<float, BitmapEnhance>>();
            InitNodeDictionary(_ast);

            _bitmap = new Bitmap(CalculateWidth(), CalulateHeight());
            _graphics = Graphics.FromImage(_bitmap);
            _graphics.Clear(Color.White);
        }

        public void DrawToFile(string filename)
        {
            BuildBitmap();

            _bitmap.Save(filename, ImageFormat.Bmp);
        }

        private int CalculateWidth()
        {
            var width = 0;

            foreach (var y in _bitMaps.Keys)
            {
                var sum = _bitMaps[y].Sum(b => b.Value.Width + 25);

                if (sum > width)
                {
                    width = sum;
                }
            }

            return width;
        }

        private int CalulateHeight()
        {
            var nbNode = _bitMaps.Max(b => b.Key + 1);

            return (int)Math.Floor(nbNode * (_bitMaps.Values.First().Values.First().Height + 25));
        }

        private void BuildBitmap()
        {
            foreach (var level in _bitMaps.Reverse())
            {
                var nbChildProcess = 0;

                foreach (var row in _bitMaps[level.Key])
                {
                    var bitmap = _bitMaps[level.Key][row.Key];

                    AddNodeToBitmap(bitmap, row.Key, level.Key);

                    if (bitmap.NodeInfo.Childs.Count > 0)
                    {
                        var nbChild = bitmap.NodeInfo.Childs.Count;

                        for  (int i = 0; i < nbChild; i++)
                        {
                            _graphics.DrawLine(new Pen(Brushes.Black), bitmap.BottomCenter, _bitMaps[level.Key + 1].ElementAt(i + nbChildProcess).Value.TopCenter);
                        }

                        nbChildProcess += nbChild;
                    }
                }
            }
        }

        private void InitNodeDictionary(AST ast, float x = 0, float y = 0)
        {
            if (ast.Childs.Count != 0)
            {
                var xJump = _bitMaps.ContainsKey(y + 1) ? _bitMaps[y + 1].Count : 0;

                for (int i = 0; i < ast.Childs.Count; i++)
                {
                    InitNodeDictionary(ast.Childs[i], xJump + x + i, y + 1);
                }
            }

            AddNodeToDictionary(ast, x, y);
        }

        private void AddNodeToDictionary(AST ast, float x, float y)
        {
            if (!_bitMaps.ContainsKey(y))
            {
                _bitMaps.Add(y, new SortedDictionary<float, BitmapEnhance>());
            }

            _bitMaps[y].Add(x, GetBitmapNode(ast));
        }

        private void AddNodeToBitmap(BitmapEnhance bitmap, float x, float y)
        {
            var center = _bitmap.Width / 2;

            var halfNodeWidth = bitmap.Width / 2;

            var rowLength = _bitMaps[y].Values.Sum(b => b.Width + 25) - 25;

            var adjust = 0f;

            foreach (var node in _bitMaps[y])
            {
                if (node.Key > x)
                {
                    adjust -= (node.Value.Width + 25) / 2f;
                }
                else if (node.Key < x)
                {
                    adjust += (node.Value.Width + 25) / 2f;
                }
            }

            var point_x = center - halfNodeWidth + adjust;

            var point_y = y * 25 + y * bitmap.Height;

            bitmap.TopCenter = new Point((int)Math.Floor(point_x + bitmap.Width / 2), (int)Math.Floor(point_y));
            bitmap.BottomCenter = new Point((int)Math.Floor(point_x + bitmap.Width / 2), (int)Math.Floor(point_y + bitmap.Height));

            _graphics.DrawImage(bitmap, point_x, point_y);
        }

        private BitmapEnhance GetBitmapNode(AST ast)
        {
            var data = $"{ast.Type}:{ast.Value}";

            Font font = new Font("Arial", 12, FontStyle.Bold, GraphicsUnit.Pixel);
            var stringSize = MesureString(data, font);

            var bitmap = new Bitmap((int)Math.Floor(stringSize.Width * 1.33), (int)Math.Floor(stringSize.Height * 2 + 4));

            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.Clear(Color.White);
                graphics.DrawEllipse(new Pen(Brushes.Black), 0, 0, bitmap.Width - 1, bitmap.Height - 1);
                graphics.DrawString(data, font, new SolidBrush(Color.Black), bitmap.Width / 2 - stringSize.Width / 2, bitmap.Height / 2 - stringSize.Height / 2);
                graphics.Flush();
            }

            return new BitmapEnhance(bitmap, ast);
        }

        private SizeF MesureString(string data, Font font)
        {
            using var image = new Bitmap(1, 1);
            using var graphics = Graphics.FromImage(image);
            return graphics.MeasureString(data, font);
        }
    }

    public class BitmapEnhance
    {
        private readonly Bitmap _bitmap;

        public BitmapEnhance(Bitmap bitmap, AST ast)
        {
            _bitmap = bitmap;
            NodeInfo = ast;
        }

        public int Width => _bitmap.Width;

        public int Height => _bitmap.Height;

        public Point TopCenter { get; set; }

        public Point BottomCenter { get; set; }

        public static implicit operator Bitmap(BitmapEnhance bitmapWithPosition) => bitmapWithPosition._bitmap;

        public AST NodeInfo { get; }
    }
}
