using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace ChessGame.GameClasses
{
    internal sealed class ValidMove : Piece
    {
        public ValidMove(PieceColor color, int row, int column) : base(color, row, column)
        {
            this.MouseEnter        += this.ValidMove_MouseEnter;
            this.MouseLeave        += this.ValidMove_MouseLeave;
            this.MouseLeftButtonUp += ValidMove_MouseLeftButtonUp;
            LastClicked            += OnLastClicked;
        }

        public ValidMove(PieceColor color, Coordinate coordinate) : this(color, coordinate.Row, coordinate.Column)
        {
        }

        protected override ImageBrush WhiteImage { get; } = CircleBrush;

        protected override ImageBrush BlackImage { get; } = CircleBrush;

        internal static ImageBrush CircleBrush
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

        private static Piece? LastClickedPiece { get; set; }

        private static ImageBrush RectangleBrush
        {
            get
            {
                var rect = new GeometryDrawing(new SolidColorBrush(),
                                               new Pen(Brushes.Chartreuse, 1),
                                               new RectangleGeometry(new Rect()));

                var brush = new ImageBrush(new DrawingImage(rect)) { Opacity = 0.2 };
                return brush;
            }
        }

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

        private void ValidMove_MouseEnter(object sender, MouseEventArgs e)
        {
            this.Background = RectangleBrush;
        }

        private void ValidMove_MouseLeave(object sender, MouseEventArgs e)
        {
            this.Background = CircleBrush;
        }

        /// <summary>
        ///     No need to implement this method for valid move.
        /// </summary>
        protected override void UpdateValidMoves()
        {
        }
    }
}
