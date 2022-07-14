using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace ChessGame.GameClasses
{
    internal static class ChessBoard
    {
        public delegate void BoardChangeHandler(UserControl sender, BoardChangedEventArgs e);

        public static readonly List<Piece> Pieces = new();

        public static UserControl?[,] Board { get; } = new UserControl?[8, 8];

        public static event BoardChangeHandler? BoardChanged;

        public static UserControl? GetControlOrNull(int row, int column)
        {
            var coord = new Coordinate(row, column);
            return Board[coord.Row, coord.Column];
        }

        public static UserControl? GetControlOrNull(Coordinate coordinate)
        {
            return GetControlOrNull(coordinate.Row, coordinate.Column);
        }

        public static void SetPiece(Piece piece, int row, int column)
        {
            var coord = new Coordinate(row, column);
            Board[coord.Row, coord.Column] = piece;
            piece.Coordinate               = coord;
            OnBoardChanged(piece, new BoardChangedEventArgs(null, coord));
            Pieces.Add(piece);
        }

        public static void SetPiece(Piece piece, Coordinate coordinate)
        {
            SetPiece(piece, coordinate.Row, coordinate.Column);
        }

        public static void RemoveControl(int row, int column)
        {
            var          coord   = new Coordinate(row, column);
            UserControl? control = GetControlOrNull(coord);
            if (control == null)
            {
                return;
            }

            Board[coord.Row, coord.Column] = null;
            OnBoardChanged(control, new BoardChangedEventArgs(coord, null));
            if (control is Piece piece)
            {
                Pieces.Remove(piece);
            }
        }

        public static void RemoveControl(Coordinate coordinate)
        {
            RemoveControl(coordinate.Row, coordinate.Column);
        }

        internal static void OnBoardChanged(UserControl sender, BoardChangedEventArgs e)
        {
            BoardChanged?.Invoke(sender, e);
        }

        /// <summary>
        ///     Removes all pieces from the board.
        /// </summary>
        public static void Clear()
        {
            for (var row = 0; row < 8; row++)
            {
                for (var column = 0; column < 8; column++)
                {
                    Board[row, column] = null;
                }
            }

            Pieces.Clear();
        }
    }

    internal sealed class BoardChangedEventArgs : EventArgs
    {
        public BoardChangedEventArgs(Coordinate? oldCoordinate, Coordinate? newCoordinate)
        {
            this.OldCoordinate = oldCoordinate;
            this.NewCoordinate = newCoordinate;
        }

        public Coordinate? OldCoordinate { get; }

        public Coordinate? NewCoordinate { get; }
    }
}
