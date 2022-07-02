using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace ChessGame.GameClasses
{
    internal sealed class ValidMove : Piece
    {
        private static readonly ImageBrush BrushRectangle = GetBrushRectangle();

        public ValidMove(PieceColor color, int row, int column) : base(color, row, column)
        {
            // Images for both colors are identical, so we can use one image for both colors.
            this.BlackImage        =  this.WhiteImage;
            this.MouseEnter        += this.ValidMove_MouseEnter;
            this.MouseLeave        += this.ValidMove_MouseLeave;
            this.MouseLeftButtonUp += ValidMove_MouseLeftButtonUp;
            LastClicked            += OnLastClicked;
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
                                Viewport = new Rect(0.35, 0.35, 0.3, 0.3),
                                Opacity  = 0.4
                            };
                return image;
            }
        }

        protected override ImageBrush BlackImage { get; }

        private static Piece? LastClickedPiece { get; set; }

        private static void OnLastClicked(Piece? sender, RoutedEventArgs e)
        {
            LastClickedPiece = sender;
        }

        private static void ValidMove_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (LastClickedPiece == null)
            {
                throw new NullReferenceException("LastClickedPiece is null.");
            }

            var        validMove  = (ValidMove)sender;
            Coordinate coordinate = validMove.Coordinate;
            LastClickedPiece.MoveTo(coordinate);
        }

        private void ValidMove_MouseLeave(object sender, MouseEventArgs e)
        {
            this.Background = this.WhiteImage;
        }

        private void ValidMove_MouseEnter(object sender, MouseEventArgs e)
        {
            this.Background = BrushRectangle;
        }

        private static ImageBrush GetBrushRectangle()
        {
            var rect = new GeometryDrawing(new SolidColorBrush(),
                                           new Pen(Brushes.Chartreuse, 1),
                                           new RectangleGeometry(new Rect()));

            var brush = new ImageBrush(new DrawingImage(rect)) { Opacity = 0.2 };
            return brush;
        }

        /// <summary>
        ///     No need to implement this method.
        /// </summary>
        protected override void UpdateMoves()
        {
        }
    }
}
