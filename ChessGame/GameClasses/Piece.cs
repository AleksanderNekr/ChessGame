using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ChessGame.GameClasses
{
    internal abstract class Piece : UserControl
    {
        protected Piece(PieceColor color, int row, int column)
        {
            this.Color           = color;
            this.Coordinate      = new Coordinate(row, column);
            this.Cursor          = Cursors.Hand;
            this.BorderThickness = new Thickness(1);
            this.SetBackgroundImage();

            this.MouseEnter      += Piece_MouseEnter;
            this.MouseLeave      += Piece_MouseLeave;
            this.GotFocus       += Piece_GotFocus;
            ChessBoard.SetPiece(this, this.Coordinate);
        }

        private static void Piece_GotFocus(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Got focus");
        }

        protected abstract ImageBrush WhiteImage { get; }

        protected abstract ImageBrush BlackImage { get; }

        private void SetBackgroundImage()
        {
            if (this.Color == PieceColor.White)
            {
                this.Background = this.WhiteImage;
                return;
            }

            this.Background = this.BlackImage;
        }

        private static void Piece_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Piece)sender).BorderBrush = Brushes.Transparent;
        }

        private static void Piece_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Piece)sender).BorderBrush = Brushes.Chartreuse;
        }

        protected Piece(PieceColor color, Coordinate coordinate) : this(color, coordinate.Row, coordinate.Column)
        {
        }

        public PieceColor Color { get; }

        protected Coordinate Coordinate { get; }

        public abstract ICollection<Coordinate> ValidMoves { get; }
    }

    public enum PieceColor
    {
        White,
        Black
    }
}
