using System.Windows;
using System.Windows.Media;

namespace ChessGame.GameClasses
{
    internal sealed class King : Piece
    {
        /// <summary>
        ///     Constructor for the Piece class.
        /// </summary>
        /// <param name="color">The color of the piece.</param>
        /// <param name="row">The row of the piece.</param>
        /// <param name="column">The column of the piece.</param>
        public King(PieceColor color, int row, int column) : base(color, row, column)
        {
        }

        /// <summary>
        ///     Constructor for the Piece class.
        /// </summary>
        /// <param name="color">The color of the piece.</param>
        /// <param name="coordinate">The coordinate of the piece.</param>
        public King(PieceColor color, Coordinate coordinate) : base(color, coordinate)
        {
        }

        /// <summary>
        ///     White image of the piece.
        /// </summary>
        protected override ImageBrush WhiteImage { get; } = (ImageBrush)Application.Current.Resources["WhiteKing"];

        /// <summary>
        ///     Black image of the piece.
        /// </summary>
        protected override ImageBrush BlackImage { get; } = (ImageBrush)Application.Current.Resources["BlackKing"];

        /// <summary>
        ///     Updates the valid moves of the piece.
        /// </summary>
        protected override void UpdateValidMoves()
        {
            this.ValidMoves.Clear();
            this.TryToAdd(-1, 0);
            this.TryToAdd(1,  0);
            this.TryToAdd(0,  -1);
            this.TryToAdd(0,  1);
            this.TryToAdd(-1, -1);
            this.TryToAdd(-1, 1);
            this.TryToAdd(1,  -1);
            this.TryToAdd(1,  1);
        }

        public override Piece Clone()
        {
            return new King(this.Color, this.Coordinate);
        }

        private void TryToAdd(int rowDif, int colDif)
        {
            if (!Coordinate.IsCorrectCoordinate(this.Coordinate.Row + rowDif, this.Coordinate.Column + colDif))
            {
                return;
            }

            var    newCoordinate = new Coordinate(this.Coordinate.Row + rowDif, this.Coordinate.Column + colDif);
            Piece? place         = ChessBoard.GetPieceOrNull(newCoordinate);
            if ((place != null) && (place.Color == this.Color))
            {
                return;
            }

            this.ValidMoves.Add(newCoordinate);
        }
    }
}
