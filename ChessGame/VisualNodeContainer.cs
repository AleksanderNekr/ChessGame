using ChessGame.GameClasses;

namespace ChessGame;

internal sealed record VisualNodeContainer(ChessBoard Board, int HNumber, int Step)
{
    public int GNumber { get; set; }

    public int FNumber => HNumber + GNumber;
}