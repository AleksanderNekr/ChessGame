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

        private void TryToAdd(int rowDif, int colDif)
        {
            int newRow = this.Coordinate.Row    + rowDif;
            int newCol = this.Coordinate.Column + colDif;
            if (!Coordinate.IsCorrectCoordinate(newRow, newCol))
            {
                return;
            }

            var newCoordinate = new Coordinate(newRow, newCol);
            if (ChessBoard.GetPieceOrNull(newCoordinate) is Piece piece && (piece.Color == this.Color))
            {
                return;
            }

            // If going on this place is leading to a check, then it is not a valid move.
            // if (this.IsUnderAttack(newCoordinate))
            // {
            //     return;
            // }

            this.ValidMoves.Add(newCoordinate);
        }

        private bool IsUnderAttack(Coordinate newCoordinate)
        {
            Coordinate oldKingCoordinate = this.Coordinate;

            PieceColor enemyColor = this.Color == PieceColor.White
                                        ? PieceColor.Black
                                        : PieceColor.White;

            // If there is an enemy piece on the new place, then remember it.
            var enemy = ChessBoard.GetPieceOrNull(newCoordinate);

            // Move the king to the new place.
            ChessBoard.Board[oldKingCoordinate.Row, oldKingCoordinate.Column] = null;
            ChessBoard.Board[newCoordinate.Row, newCoordinate.Column]         = this;

            // Check if the king is in check.
            if (GetAllAttackCoordinates(enemyColor).Contains(newCoordinate))
            {
                // If the king is in check, then move the king back to the old place.
                this.RestorePosition(newCoordinate, oldKingCoordinate, enemy);

                return true;
            }

            // If the king is not in check, then restore the old position of the king and return false.
            this.RestorePosition(newCoordinate, oldKingCoordinate, enemy);

            return false;
        }

        private void RestorePosition(Coordinate newCoordinate, Coordinate oldKingCoordinate, Piece? enemy)
        {
            ChessBoard.Board[oldKingCoordinate.Row, oldKingCoordinate.Column] = this;
            ChessBoard.Board[newCoordinate.Row, newCoordinate.Column]         = enemy;
        }
    }
}
