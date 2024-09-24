using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace ChessGame.GameClasses;

public sealed class King : Piece
{
    /// <summary>
    ///     Constructor for the Piece class.
    /// </summary>
    /// <param name="board">The chess board</param>
    /// <param name="color">The color of the piece.</param>
    /// <param name="row">The row of the piece.</param>
    /// <param name="column">The column of the piece.</param>
    public King(ChessBoard board, PieceColor color, int row, int column) : base(board, color, row, column)
    {
    }

    /// <summary>
    ///     Constructor for the Piece class.
    /// </summary>
    /// <param name="board">The chess board</param>
    /// <param name="color">The color of the piece.</param>
    /// <param name="coordinate">The coordinate of the piece.</param>
    public King(ChessBoard board, PieceColor color, Coordinate coordinate) : base(board, color, coordinate)
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
    public override void UpdateValidMoves()
    {
        ValidMoves.Clear();
        TryToAdd(-1, 0);
        TryToAdd(1, 0);
        TryToAdd(0, -1);
        TryToAdd(0, 1);
        TryToAdd(-1, -1);
        TryToAdd(-1, 1);
        TryToAdd(1, -1);
        TryToAdd(1, 1);
    }

    private void TryToAdd(int rowDif, int colDif)
    {
        var newRow = Coordinate.Row + rowDif;
        var newCol = Coordinate.Column + colDif;
        if (!Coordinate.IsCorrectCoordinate(newRow, newCol))
        {
            return;
        }

        var newCoordinate = new Coordinate(newRow, newCol);
        var piece = Board.GetPieceOrNull(newCoordinate);
        if (piece != null && piece.Color == Color)
        {
            return;
        }

        if (IsUnderAttack(newCoordinate))
        {
            return;
        }

        ValidMoves.Add(newCoordinate);
    }


    private bool IsUnderAttack(Coordinate newCoordinate)
    {
        var enemyPieces = Board.GetPlayerPieces(1 - Color);

        var enemyPawns = enemyPieces.Where(piece => piece is Pawn).Cast<Pawn>();
        if (EnemyPawnAttacks(newCoordinate))
        {
            return true;
        }

        if (EnemyKingIsNearTo(newCoordinate))
        {
            return true;
        }

        var otherEnemyPieces = enemyPieces.Where(piece => piece is not Pawn);
        return otherEnemyPieces.Any(enemyPiece => enemyPiece
            .GetValidMoves()
            .Any(validMove => validMove.Row == newCoordinate.Row && validMove.Column == newCoordinate.Column));

        bool EnemyPawnAttacks(Coordinate coordinate)
            => enemyPawns.Any(enemyPawn => enemyPawn
                .GetValidMoves()
                .Where(validEnemyPawnMove => validEnemyPawnMove.Column != enemyPawn.Coordinate.Column)
                .Any(validEnemyPawnMove => validEnemyPawnMove.Row == coordinate.Row && validEnemyPawnMove.Column == coordinate.Column));
    }

    private bool EnemyKingIsNearTo(Coordinate newCoordinate)
    {
        var enemyKing = Board.GetPlayerPieces(1 - Color).FirstOrDefault(piece => piece is King);
        if (enemyKing == null)
        {
            return false;
        }

        var enemyKingCoordinate = enemyKing.Coordinate;
        if (enemyKingCoordinate.Row == newCoordinate.Row && Math.Abs(enemyKingCoordinate.Column - newCoordinate.Column) == 1)
        {
            return true;
        }

        return enemyKingCoordinate.Column == newCoordinate.Column && Math.Abs(enemyKingCoordinate.Row - newCoordinate.Row) == 1;
    }
}