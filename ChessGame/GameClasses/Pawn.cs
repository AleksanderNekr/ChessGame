using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ChessGame.GameClasses;

/// <summary>
///     Pawn class.
/// </summary>
public sealed class Pawn : Piece
{
    /// <inheritdoc />
    public Pawn(ChessBoard board, PieceColor color, int row, int column) : base(board, color, row, column)
    {
    }

    /// <inheritdoc />
    protected override ImageBrush WhiteImage { get; } = (ImageBrush)Application.Current.Resources["WhitePawn"];

    /// <inheritdoc />
    protected override ImageBrush BlackImage { get; } = (ImageBrush)Application.Current.Resources["BlackPawn"];

    private int Move
        => Color == PieceColor.White
            ? -1
            : 1;

    private int InitialRow
        => Color == PieceColor.White
            ? 6
            : 1;

    public override void UpdateValidMoves(bool checkCheck = true)
    {
        ValidMoves.Clear();
        if (Coordinate.Row == InitialRow + Move * 6)
        {
            return;
        }

        UpdatePawnDefaultMoves(checkCheck);
        UpdatePawnAttackMoves(checkCheck);
    }

    protected override IEnumerable<Coordinate> UpdateAndGetKingAttackMoves()
    {
        if (Coordinate.Column != 0)
        {
            yield return TryToAddAttackMove(checkCheck: false, columnChange: -1);
        }

        if (Coordinate.Column != 7)
        {
            yield return TryToAddAttackMove(checkCheck: false, columnChange: 1);
        }
    }

    private void UpdatePawnDefaultMoves(bool checkCheck)
    {
        var isCorrectMove = TryToAddMove(Move, checkCheck);
        if (isCorrectMove && Coordinate.Row == InitialRow)
        {
            TryToAddMove(Move * 2, checkCheck);
        }
    }

    private bool TryToAddMove(int move, bool checkCheck)
    {
        var moveRow = Coordinate.Row + move;
        var newCoordinate = new Coordinate(moveRow, Coordinate.Column);
        UserControl? placeUnderMove = Board.GetPieceOrNull(newCoordinate);

        if (placeUnderMove != null)
        {
            return false;
        }

        if (checkCheck && MoveThereWillCauseCheck(newCoordinate))
        {
            return false;
        }

        ValidMoves.Add(newCoordinate);
        return true;
    }

    private void UpdatePawnAttackMoves(bool checkCheck)
    {
        if (Coordinate.Column != 0)
        {
            TryToAddAttackMove(checkCheck, columnChange: -1);
        }

        if (Coordinate.Column != 7)
        {
            TryToAddAttackMove(checkCheck, columnChange: 1);
        }
    }

    private Coordinate TryToAddAttackMove(bool checkCheck, int columnChange)
    {
        var moveRow = Coordinate.Row + Move;
        var newCoordinate = new Coordinate(moveRow, Coordinate.Column + columnChange);
        if (Board.GetPieceOrNull(newCoordinate) is { } enemy
            && enemy.Color != Color
            && !(checkCheck && MoveThereWillCauseCheck(newCoordinate)))
        {
            ValidMoves.Add(newCoordinate);
        }

        return newCoordinate;
    }
}