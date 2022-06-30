using System.Collections.Generic;

namespace ChessGame.GameClasses
{
    public abstract partial class Piece
    {
        protected Piece(PieceColor color, int row, int column)
        {
            this.Color      = color;
            this.Coordinate = new Coordinate(row, column);
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
