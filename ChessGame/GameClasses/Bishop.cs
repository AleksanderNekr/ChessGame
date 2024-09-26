using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace ChessGame.GameClasses;

public sealed class Bishop : Piece
{
    /// <summary>
    ///     Constructor for the Piece class.
    /// </summary>
    /// <param name="color">The color of the piece.</param>
    /// <param name="row">The row of the piece.</param>
    /// <param name="column">The column of the piece.</param>
    public Bishop(ChessBoard board, PieceColor color, int row, int column) : base(board, color, row, column)
    {
    }

    /// <summary>
    ///     White image of the piece.
    /// </summary>
    protected override ImageBrush WhiteImage { get; } = (ImageBrush)Application.Current.Resources["WhiteBishop"];

    /// <summary>
    ///     Black image of the piece.
    /// </summary>
    protected override ImageBrush BlackImage { get; } = (ImageBrush)Application.Current.Resources["BlackBishop"];

    /// <summary>
    ///     Updates the valid moves of the piece.
    /// </summary>
    /// <param name="checkCheck"></param>
    public override void UpdateValidMoves(bool checkCheck = true)
    {
        ValidMoves.Clear();
        AddRangeMoves(checkCheck, this, -1, -1);
        AddRangeMoves(checkCheck, this, -1, 1);
        AddRangeMoves(checkCheck, this, 1, -1);
        AddRangeMoves(checkCheck, this, 1, 1);
    }

    protected override IEnumerable<Coordinate> UpdateAndGetKingAttackMoves()
    {
        UpdateValidMoves(checkCheck: false);
        return ValidMoves;
    }
}