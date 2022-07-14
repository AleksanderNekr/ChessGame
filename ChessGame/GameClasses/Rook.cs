using System.Windows;
using System.Windows.Media;

namespace ChessGame.GameClasses
{
    internal sealed class Rook : Piece
    {
        /// <summary>
        ///     Constructor for the Piece class.
        /// </summary>
        /// <param name="color">The color of the piece.</param>
        /// <param name="row">The row of the piece.</param>
        /// <param name="column">The column of the piece.</param>
        public Rook(PieceColor color, int row, int column) : base(color, row, column)
        {
        }

        /// <summary>
        ///     Constructor for the Piece class.
        /// </summary>
        /// <param name="color">The color of the piece.</param>
        /// <param name="coordinate">The coordinate of the piece.</param>
        public Rook(PieceColor color, Coordinate coordinate) : base(color, coordinate)
        {
        }

        /// <summary>
        ///     White image of the piece.
        /// </summary>
        protected override ImageBrush WhiteImage { get; } = (ImageBrush)Application.Current.Resources["WhiteRook"];

        /// <summary>
        ///     Black image of the piece.
        /// </summary>
        protected override ImageBrush BlackImage { get; } = (ImageBrush)Application.Current.Resources["BlackRook"];

        /// <summary>
        ///     Updates the valid moves of the piece.
        /// </summary>
        protected override void UpdateValidMoves()
        {
            this.ValidMoves.Clear();
            AddRangeMoves(this, -1, 0);
            AddRangeMoves(this, 1,  0);
            AddRangeMoves(this, 0,  -1);
            AddRangeMoves(this, 0,  1);
        }

        public Piece Clone()
        {
            return new Rook(this.Color, this.Coordinate);
        }
    }
}
