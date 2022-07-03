using System.Windows;
using System.Windows.Media;

namespace ChessGame.GameClasses
{
    internal class Rook : Piece
    {
        /// <summary>
        /// Constructor for the Piece class.
        /// </summary>
        /// <param name="color">The color of the piece.</param>
        /// <param name="row">The row of the piece.</param>
        /// <param name="column">The column of the piece.</param>
        public Rook(PieceColor color, int row, int column) : base(color, row, column)
        {
        }

        /// <summary>
        /// Constructor for the Piece class.
        /// </summary>
        /// <param name="color">The color of the piece.</param>
        /// <param name="coordinate">The coordinate of the piece.</param>
        public Rook(PieceColor color, Coordinate coordinate) : base(color, coordinate)
        {
        }

        /// <summary>
        /// White image of the piece.
        /// </summary>
        protected override ImageBrush WhiteImage { get; } = (ImageBrush)Application.Current.Resources["WhiteRook"];

        /// <summary>
        /// Black image of the piece.
        /// </summary>
        protected override ImageBrush BlackImage { get; } = (ImageBrush)Application.Current.Resources["BlackRook"];

        /// <summary>
        /// Updates the valid moves of the piece.
        /// </summary>
        protected override void UpdateValidMoves()
        {
            this.ValidMoves.Clear();
            this.AddMoves(-1, 0);
            this.AddMoves(1, 0);
            this.AddMoves(0, -1);
            this.AddMoves(0, 1);
        }

        private void AddMoves(int rowDif, int colDif)
        {
            int row    = this.Coordinate.Row;
            int column = this.Coordinate.Column;
            while (Coordinate.IsCorrectCoordinate(row += rowDif, column += colDif))
            {
                Piece? place = ChessBoard.GetPieceOrNull(row, column);
                if (place == null)
                {
                    this.ValidMoves.Add(new Coordinate(row, column));
                    continue;
                }

                // If ally piece is found, then stop.
                if (place.Color == this.Color)
                {
                    break;
                }

                // If enemy piece is found, then add it to the valid moves and stop.
                this.ValidMoves.Add(new Coordinate(row, column));
                break;
            }
        }
    }
}
