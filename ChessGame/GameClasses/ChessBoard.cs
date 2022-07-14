using System;
using System.Collections.Generic;
using System.Linq;

namespace ChessGame.GameClasses
{
    internal static class ChessBoard
    {
        public delegate void BoardChangeHandler(Piece sender, BoardChangedEventArgs e);

        public static List<Piece> Pieces = new();

        public static Piece?[,] Board { get; } = new Piece?[8, 8];

        public static event BoardChangeHandler? BoardChanged;

        public static Piece? GetPieceOrNull(int row, int column)
        {
            var coord = new Coordinate(row, column);
            return Board[coord.Row, coord.Column];
        }

        public static Piece? GetPieceOrNull(Coordinate coordinate)
        {
            return GetPieceOrNull(coordinate.Row, coordinate.Column);
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

        public static void RemovePiece(int row, int column)
        {
            var    coord = new Coordinate(row, column);
            Piece? piece = GetPieceOrNull(coord);
            if (piece == null)
            {
                return;
            }

            Board[coord.Row, coord.Column] = null;
            OnBoardChanged(piece, new BoardChangedEventArgs(coord, null));
            Pieces.Remove(piece);
        }

        public static void RemovePiece(Coordinate coordinate)
        {
            RemovePiece(coordinate.Row, coordinate.Column);
        }

        internal static void OnBoardChanged(Piece sender, BoardChangedEventArgs e)
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
