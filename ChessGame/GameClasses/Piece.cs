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
            this.Color      =  color;
            this.Coordinate =  new Coordinate(row, column);
            this.Cursor     =  Cursors.Hand;
            this.BorderThickness = new Thickness(1);
            this.MouseEnter += Piece_MouseEnter;
            this.MouseLeave += Piece_MouseLeave;
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

        public PieceColor Color { get; set; }

        protected Coordinate Coordinate { get; set; }

        public abstract List<Coordinate> ValidMoves { get; }
    }

    public enum PieceColor
    {
        White,
        Black
    }
}
