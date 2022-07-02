using System;

namespace ChessGame.GameClasses
{
    internal static class ChessBoard
    {
        public delegate void ContentChangeHandler(Piece sender, ContentChangedEventArgs e);

        public static event ContentChangeHandler? ContentChanged;

        public static Piece?[,] Board { get; } = new Piece?[8, 8];

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
            OnContentChanged(piece, new ContentChangedEventArgs(null, coord));
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

            OnContentChanged(piece, new ContentChangedEventArgs(coord, null));
            Board[coord.Row, coord.Column] = null;
        }

        public static void RemovePiece(Coordinate coordinate)
        {
            RemovePiece(coordinate.Row, coordinate.Column);
        }

        private static void OnContentChanged(Piece sender, ContentChangedEventArgs e)
        {
            ContentChanged?.Invoke(sender, e);
        }
    }

    internal sealed class ContentChangedEventArgs : EventArgs
    {
        public ContentChangedEventArgs(Coordinate? oldCoordinate, Coordinate? newCoordinate)
        {
            this.OldCoordinate = oldCoordinate;
            this.NewCoordinate = newCoordinate;
        }

        public Coordinate? OldCoordinate { get; }

        public Coordinate? NewCoordinate { get; }
    }
}
