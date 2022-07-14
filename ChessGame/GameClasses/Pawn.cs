using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ChessGame.GameClasses
{
    /// <summary>
    ///     Pawn class.
    /// </summary>
    internal sealed class Pawn : Piece
    {
        /// <inheritdoc />
        public Pawn(PieceColor color, int row, int column) : base(color, row, column)
        {
        }

        /// <inheritdoc />
        public Pawn(PieceColor color, Coordinate coordinate) : this(color, coordinate.Row, coordinate.Column)
        {
        }

        /// <inheritdoc />
        protected override ImageBrush WhiteImage { get; } = (ImageBrush)Application.Current.Resources["WhitePawn"];

        /// <inheritdoc />
        protected override ImageBrush BlackImage { get; } = (ImageBrush)Application.Current.Resources["BlackPawn"];

        internal Coordinate LastMove { get; set; }

        internal Coordinate PrevCoord { get; set; }

        internal int Move
        {
            get
            {
                return this.Color == PieceColor.White
                           ? -1
                           : 1;
            }
        }

        private int InitialRow
        {
            get
            {
                return this.Color == PieceColor.White
                           ? 6
                           : 1;
            }
        }

        /// <inheritdoc />
        protected override void UpdateValidMoves()
        {
            this.ValidMoves.Clear();
            if (this.Coordinate.Row == (this.InitialRow + (this.Move * 6)))
            {
                return;
            }

            this.UpdatePawnDefaultMoves();
            this.UpdatePawnAttackMoves();
        }

        private void UpdatePawnDefaultMoves()
        {
            bool isCorrectMove = this.TryToAddMove(this.Move);
            if (isCorrectMove && (this.Coordinate.Row == this.InitialRow))
            {
                this.TryToAddMove(this.Move * 2);
            }
        }

        private bool TryToAddMove(int move)
        {
            int          moveRow        = this.Coordinate.Row + move;
            var          newCoordinate  = new Coordinate(moveRow, this.Coordinate.Column);
            UserControl? placeUnderMove = ChessBoard.GetControlOrNull(newCoordinate);

            if (placeUnderMove != null)
            {
                return false;
            }

            this.ValidMoves.Add(newCoordinate);
            return true;
        }

        private void UpdatePawnAttackMoves()
        {
            if (this.Coordinate.Column != 0)
            {
                this.TryToAddAttackMove(columnChange: -1);
            }

            if (this.Coordinate.Column != 7)
            {
                this.TryToAddAttackMove(columnChange: 1);
            }
        }

        private void TryToAddAttackMove(int columnChange)
        {
            int moveRow       = this.Coordinate.Row + this.Move;
            var newCoordinate = new Coordinate(moveRow, this.Coordinate.Column + columnChange);
            if (ChessBoard.GetControlOrNull(newCoordinate) is Piece enemy && (enemy.Color != this.Color))
            {
                this.ValidMoves.Add(newCoordinate);
            }
        }
    }
}
