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
            this.Coordinate           =  new Coordinate(row, column);
            this.Color                =  color;
            this.Cursor               =  Cursors.Hand;
            this.BorderThickness      =  new Thickness(1);
            this.Focusable            =  true;
            this.FocusVisualStyle     =  null;
            ChessBoard.ContentChanged += ChessBoard_ContentChanged;
            this.MouseEnter           += Piece_MouseEnter;
            this.MouseLeave           += Piece_MouseLeave;
            this.GotFocus             += Piece_GotFocus;
            this.LostFocus            += Piece_LostFocus;
            this.MouseLeftButtonUp    += Piece_MouseLeftButtonUp;
            this.SetBackgroundImage();
            ChessBoard.SetPiece(this, this.Coordinate);
        }

        protected Piece(PieceColor color, Coordinate coordinate) : this(color, coordinate.Row, coordinate.Column)
        {
        }

        public PieceColor Color { get; }

        public abstract ICollection<Coordinate> ValidMoves { get; }

        protected abstract ImageBrush WhiteImage { get; }

        protected abstract ImageBrush BlackImage { get; }

        protected Coordinate Coordinate { get; }

        protected abstract void UpdateMoves();

        private static void Piece_GotFocus(object sender, RoutedEventArgs e)
        {
            var piece = (Piece)sender;
            piece.BorderBrush =  Brushes.Chartreuse;
            piece.MouseEnter  -= Piece_MouseEnter;
            piece.MouseLeave  -= Piece_MouseLeave;
        }

        private static void Piece_LostFocus(object sender, RoutedEventArgs e)
        {
            var piece = (Piece)sender;
            piece.BorderBrush =  Brushes.Transparent;
            piece.MouseEnter  += Piece_MouseEnter;
            piece.MouseLeave  += Piece_MouseLeave;
        }

        private static void Piece_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var piece = (Piece)sender;
            if (piece.IsFocused)
            {
                // Remove focus from the piece.
                piece.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                return;
            }

            piece.Focus();
        }

        private static void ChessBoard_ContentChanged(Piece sender, ContentChangedEventArgs e)
        {
            sender.UpdateMoves();
        }

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
    }

    internal enum PieceColor
    {
        White,
        Black
    }
}
