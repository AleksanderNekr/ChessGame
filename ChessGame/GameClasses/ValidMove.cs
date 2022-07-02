using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ChessGame.GameClasses
{
    internal sealed class ValidMove : Piece
    {
        public ValidMove(PieceColor color, int row, int column) : base(color, row, column)
        {
            this.BlackImage        =  this.WhiteImage;
            this.MouseEnter        += this.ValidMove_MouseEnter;
            this.MouseLeave        += this.ValidMove_MouseLeave;
            this.MouseLeftButtonUp += this.ValidMove_MouseLeftButtonUp;
        }

        private void ValidMove_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
        }

        private void ValidMove_MouseLeave(object sender, MouseEventArgs e)
        {
            this.Background = this.WhiteImage;
        }

        private void ValidMove_MouseEnter(object sender, MouseEventArgs e)
        {
            this.Background = BrushRectangle;
        }

        private static readonly ImageBrush BrushRectangle = GetBrushRectangle();

        private static ImageBrush GetBrushRectangle()
        {
            var rect = new GeometryDrawing(new SolidColorBrush(),
                                           new Pen(Brushes.Chartreuse, 1),
                                           new RectangleGeometry(new Rect()));

            var brush = new ImageBrush(new DrawingImage(rect))
                        {
                            Opacity = 0.4
                        };
            return brush;
        }

        public ValidMove(PieceColor color, Coordinate coordinate) : this(color, coordinate.Row, coordinate.Column)
        {
        }

        protected override ImageBrush WhiteImage
        {
            get
            {
                var circle = new GeometryDrawing(new SolidColorBrush(),
                                                 new Pen(Brushes.Chartreuse, 1),
                                                 new EllipseGeometry());
                var image = new ImageBrush(new DrawingImage(circle))
                            {
                                Viewport = new Rect(0.25, 0.25, 0.5, 0.5),
                                Opacity  = 0.4
                            };
                return image;
            }
        }

        protected override ImageBrush BlackImage { get; }

        protected override void UpdateMoves()
        {
        }
    }
}
