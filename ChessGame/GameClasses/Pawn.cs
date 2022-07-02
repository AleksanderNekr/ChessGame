using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace ChessGame.GameClasses
{
    internal sealed class Pawn : Piece
    {
        public Pawn(PieceColor color, int row, int column) : base(color, row, column)
        {
        }

        public Pawn(PieceColor color, Coordinate coordinate) : this(color, coordinate.Row, coordinate.Column)
        {
        }

        protected override ImageBrush WhiteImage { get; } = (ImageBrush)Application.Current.Resources["WhitePawn"];
        protected override ImageBrush BlackImage { get; } = (ImageBrush)Application.Current.Resources["BlackPawn"];

        public override List<Coordinate> ValidMoves { get; } = new();

        private int Move
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

        /// <summary>
        ///     Updates the valid moves of the pawn.
        /// </summary>
        protected override void UpdateMoves()
        {
            this.ValidMoves.Clear();
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
            int    moveRow        = this.Coordinate.Row + move;
            var    newCoordinate  = new Coordinate(moveRow, this.Coordinate.Column);
            Piece? placeUnderMove = ChessBoard.GetPieceOrNull(newCoordinate);

            if (placeUnderMove != null)
            {
                return false;
            }

            this.ValidMoves.Add(newCoordinate);
            return true;
        }

        private void UpdatePawnAttackMoves()
        {
            this.TryToAddAttackMove(columnChange: -1);
            this.TryToAddAttackMove(columnChange: 1);
        }

        private void TryToAddAttackMove(int columnChange)
        {
            int    moveRow          = this.Coordinate.Row + this.Move;
            var    newCoordinate    = new Coordinate(moveRow, this.Coordinate.Column + columnChange);
            Piece? placeUnderAttack = ChessBoard.GetPieceOrNull(newCoordinate);
            if ((placeUnderAttack != null) && (placeUnderAttack.Color != this.Color))
            {
                this.ValidMoves.Add(newCoordinate);
            }
        }
    }
}
