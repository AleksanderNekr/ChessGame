using ChessGame.GameClasses;

namespace ChessGame;

public sealed record VisualNodeContainer(ChessBoard Board, int HNumber, int Step, int GNumber)
{
    public int FNumber => HNumber + GNumber;
}