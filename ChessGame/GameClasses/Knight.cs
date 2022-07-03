using System;
using System.Windows;
using System.Windows.Media;

namespace ChessGame.GameClasses
{
    /// <summary>
    ///     Knight class.
    /// </summary>
    internal sealed class Knight : Piece
    {
        /// <inheritdoc />
        public Knight(PieceColor color, int row, int column) : base(color, row, column)
        {
        }

        /// <inheritdoc />
        public Knight(PieceColor color, Coordinate coordinate) : base(color, coordinate)
        {
        }

        /// <inheritdoc />
        protected override ImageBrush WhiteImage { get; } = (ImageBrush)Application.Current.Resources["WhiteKnight"];

        /// <inheritdoc />
        protected override ImageBrush BlackImage { get; } = (ImageBrush)Application.Current.Resources["BlackKnight"];

        /// <inheritdoc />
        protected override void UpdateValidMoves()
        {
            this.ValidMoves.Clear();
            this.TryToAddMove(this.Coordinate.Row - 2, this.Coordinate.Column + 1);
            this.TryToAddMove(this.Coordinate.Row - 2, this.Coordinate.Column - 1);
            this.TryToAddMove(this.Coordinate.Row - 1, this.Coordinate.Column + 2);
            this.TryToAddMove(this.Coordinate.Row - 1, this.Coordinate.Column - 2);
            this.TryToAddMove(this.Coordinate.Row + 2, this.Coordinate.Column + 1);
            this.TryToAddMove(this.Coordinate.Row + 2, this.Coordinate.Column - 1);
            this.TryToAddMove(this.Coordinate.Row + 1, this.Coordinate.Column + 2);
            this.TryToAddMove(this.Coordinate.Row + 1, this.Coordinate.Column - 2);
        }

        public override Piece Clone()
        {
            return new Knight(this.Color, this.Coordinate);
        }

        private void TryToAddMove(int coordinateRow, int coordinateColumn)
        {
            Coordinate coordinate;
            try
            {
                coordinate = new Coordinate(coordinateRow, coordinateColumn);
            }
            catch (ArgumentOutOfRangeException)
            {
                return;
            }

            Piece? place = ChessBoard.GetPieceOrNull(coordinate);
            if ((place == null) || (place.Color != this.Color))
            {
                this.ValidMoves.Add(coordinate);
            }
        }
    }
}
