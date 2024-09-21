using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ChessGame.GameClasses;

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
        => Color == PieceColor.White
            ? -1
            : 1;

    private int InitialRow
        => Color == PieceColor.White
            ? 6
            : 1;

    /// <inheritdoc />
    protected override void UpdateValidMoves()
    {
        ValidMoves.Clear();
        if (Coordinate.Row == InitialRow + Move * 6)
        {
            return;
        }

        UpdatePawnDefaultMoves();
        UpdatePawnAttackMoves();
    }

    private void UpdatePawnDefaultMoves()
    {
        var isCorrectMove = TryToAddMove(Move);
        if (isCorrectMove && Coordinate.Row == InitialRow)
        {
            TryToAddMove(Move * 2);
        }
    }

    private bool TryToAddMove(int move)
    {
        var moveRow = Coordinate.Row + move;
        var newCoordinate = new Coordinate(moveRow, Coordinate.Column);
        UserControl? placeUnderMove = ChessBoard.GetPieceOrNull(newCoordinate);

        if (placeUnderMove != null)
        {
            return false;
        }

        ValidMoves.Add(newCoordinate);
        return true;
    }

    private void UpdatePawnAttackMoves()
    {
        if (Coordinate.Column != 0)
        {
            TryToAddAttackMove(columnChange: -1);
        }

        if (Coordinate.Column != 7)
        {
            TryToAddAttackMove(columnChange: 1);
        }
    }

    private void TryToAddAttackMove(int columnChange)
    {
        var moveRow = Coordinate.Row + Move;
        var newCoordinate = new Coordinate(moveRow, Coordinate.Column + columnChange);
        if (ChessBoard.GetPieceOrNull(newCoordinate) is Piece enemy && enemy.Color != Color)
        {
            ValidMoves.Add(newCoordinate);
        }
    }
}