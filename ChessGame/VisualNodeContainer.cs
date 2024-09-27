using ChessGame.GameClasses;

namespace ChessGame;

public sealed record VisualNodeContainer(ChessBoard Board, int HNumber, int Step, int GNumber, int Fine)
{
    public int FNumber => HNumber + GNumber + Fine;
}