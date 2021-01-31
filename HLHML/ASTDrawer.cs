using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace HLHML
{
    public class ASTDrawer
    {
        private static IComparer<float> _comparer = new DescendingOrderFloatComparer();

        private Bitmap _bitmap;
        private Graphics _graphics;
        private readonly Font _font;
        private readonly Color _backgroundColor;
        private readonly Color _drawingColor;
        private readonly Brush _brush;
        private readonly Pen _pen;
        private readonly SolidBrush _solidBrush;
        private readonly int _margin;

        public ASTDrawer() : this(new Font("Arial", 12, FontStyle.Bold, GraphicsUnit.Pixel), Color.White, Color.Black, Brushes.Black)
        {
        }

        public ASTDrawer(Font font, Color backgroundColor, Color drawingColor, Brush brush, int margin = 25)
        {
            _font = font;
            _backgroundColor = backgroundColor;
            _drawingColor = drawingColor;
            _brush = brush;
            _pen = new Pen(_brush);
            _solidBrush = new SolidBrush(_drawingColor);
            _margin = margin;
            BitMaps = new SortedDictionary<float, SortedDictionary<float, BitmapEnhance>>(_comparer);
        }

        public SortedDictionary<float, SortedDictionary<float, BitmapEnhance>> BitMaps { get; private set; }

        public Bitmap GetBitmap(AST ast)
        {
            BitMaps.Clear();

            InitNodeDictionary(ast);

            _bitmap = new Bitmap(CalculateWidth(), CalulateHeight());
            _graphics = Graphics.FromImage(_bitmap);
            _graphics.Clear(_backgroundColor);

            BuildBitmap();

            return _bitmap;
        }

        public void DrawToFile(AST ast, string filename)
        {
            BitMaps.Clear();

            InitNodeDictionary(ast);

            _bitmap = new Bitmap(CalculateWidth(), CalulateHeight());
            _graphics = Graphics.FromImage(_bitmap);
            _graphics.Clear(_backgroundColor);

            BuildBitmap();

            _bitmap.Save(filename, ImageFormat.Bmp);
        }

        private int CalculateWidth()
        {
            int width = 0;

            foreach (float y in BitMaps.Keys)
            {
                int sum = BitMaps[y].Sum(b => b.Value.Width + _margin);

                if (sum > width)
                {
                    width = sum;
                }
            }

            return width;
        }

        private int CalulateHeight()
        {
            float nbNode = BitMaps.Max(b => b.Key + 1);

            return (int)Math.Floor(nbNode * (BitMaps.Values.First().Values.First().Height + _margin));
        }

        private void BuildBitmap()
        {
            foreach (KeyValuePair<float, SortedDictionary<float, BitmapEnhance>> level in BitMaps)
            {
                int nbChildProcess = 0;

                foreach (KeyValuePair<float, BitmapEnhance> row in level.Value)
                {
                    BitmapEnhance bitmap = row.Value;

                    AddNodeToBitmap(bitmap, row.Key, level.Key);

                    if (bitmap.NodeInfo.Childs.Count > 0)
                    {
                        int nbChild = bitmap.NodeInfo.Childs.Count;

                        for (int i = 0; i < nbChild; i++)
                        {
                            _graphics.DrawLine(_pen, bitmap.BottomCenter, BitMaps[level.Key + 1].ElementAt(i + nbChildProcess).Value.TopCenter);
                        }

                        nbChildProcess += nbChild;
                    }
                }
            }
        }

        private void AddNodeToBitmap(BitmapEnhance bitmap, float x, float y)
        {
            var center = _bitmap.Width / 2;

            var halfNodeWidth = bitmap.Width / 2;

            var rowLength = BitMaps[y].Values.Sum(b => b.Width + _margin) - _margin;

            var adjust = 0f;

            foreach (var node in BitMaps[y])
            {
                if (node.Key > x)
                {
                    adjust -= (node.Value.Width + _margin) / 2f;
                }
                else if (node.Key < x)
                {
                    adjust += (node.Value.Width + _margin) / 2f;
                }
            }

            float point_x = center - halfNodeWidth + adjust;

            float point_y = y * _margin + y * bitmap.Height;

            bitmap.TopCenter = new Point((int)Math.Floor(point_x + bitmap.Width / 2), (int)Math.Floor(point_y));
            bitmap.BottomCenter = new Point((int)Math.Floor(point_x + bitmap.Width / 2), (int)Math.Floor(point_y + bitmap.Height));

            _graphics.DrawImage(bitmap, point_x, point_y);
        }

        private void InitNodeDictionary(AST ast, float x = 0, float y = 0, int xJump = 0)
        {
            if (BitMaps.TryGetValue(y + 1, out SortedDictionary<float, BitmapEnhance> subDictionary))
            {
                xJump = subDictionary.Count;
            }

            for (int i = 0; i < ast.Childs.Count; i++)
            {
                InitNodeDictionary(ast.Childs[i], xJump + x + i, y + 1);
            }

            AddNodeToDictionary(ast, x, y);
        }

        private void AddNodeToDictionary(AST ast, float x, float y)
        {
            if (!BitMaps.TryGetValue(y, out SortedDictionary<float, BitmapEnhance> subDictionary))
            {
                subDictionary = new SortedDictionary<float, BitmapEnhance>();
                BitMaps.Add(y, subDictionary);
            }

            subDictionary.Add(x, GetBitmapNode(ast));
        }

        private BitmapEnhance GetBitmapNode(AST ast)
        {
            var data = $"{ast.Type}:{ast.Value}";

            var stringSize = MesureString(data, _font);

            var bitmap = new Bitmap((int)Math.Floor(stringSize.Width * 1.33), (int)Math.Floor(stringSize.Height * 2 + 4));

            using var graphics = Graphics.FromImage(bitmap);

            graphics.Clear(_backgroundColor);
            graphics.DrawEllipse(_pen, 0, 0, bitmap.Width - 1, bitmap.Height - 1);
            graphics.DrawString(data, _font, _solidBrush, bitmap.Width / 2 - stringSize.Width / 2, bitmap.Height / 2 - stringSize.Height / 2);
            graphics.Flush();

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

    public class DescendingOrderFloatComparer : IComparer<float>
    {
        public int Compare(float x, float y)
        {
            return x.CompareTo(y) * -1;
        }
    }
}
