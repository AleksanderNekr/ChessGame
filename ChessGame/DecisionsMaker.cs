using System.Collections.Generic;
using System.Linq;
using ChessGame.GameClasses;
using TreeDrawer;

namespace ChessGame;

internal sealed class DecisionsMaker
{
    private const int DepthLimit = 10;
    private readonly HashSet<ChessBoard> _visitedBoards = new();
    private bool _breakFlag;

    public TreeNode<VisualNodeContainer> BuildDecisionTree(ChessBoard currentBoard, ChessBoard finalBoard, PieceColor startColor)
    {
        _visitedBoards.Clear();
        _visitedBoards.Add(currentBoard);
        var root = new TreeNode<VisualNodeContainer>(new VisualNodeContainer(currentBoard, 0, 0), new List<TreeNode<VisualNodeContainer>>());
        int depth = 0;
        int step = 0;
        _breakFlag = false;
        BuildBranchesAndBoundsTree(root, finalBoard, startColor, ref depth, ref step);
        return root;
    }

    private void BuildBranchesAndBoundsTree(TreeNode<VisualNodeContainer> node, ChessBoard finalBoard, PieceColor startColor, ref int depth, ref int step)
    {
        if (node.Value.Board.Equals(finalBoard) || _breakFlag)
        {
            _breakFlag = true;
            return;
        }

        if (depth >= DepthLimit)
        {
            return;
        }

        // All possible moves from current position for color
        foreach (var piece in node.Value.Board.GetPlayerPieces(startColor))
        {
            foreach (var move in piece.GetValidMoves())
            {
                var newBoard = node.Value.Board.Clone();
                newBoard.MovePiece(newBoard.GetPieceOrNull(piece.Coordinate)!, move.Row, move.Column);

                if (_visitedBoards.Any(x => x.Equals(newBoard)))
                {
                    continue;
                }

                step++;
                var newNode = new TreeNode<VisualNodeContainer>(new VisualNodeContainer(newBoard, node.Value.HNumber + 1, step), new List<TreeNode<VisualNodeContainer>>());
                node.AddChild(newNode);
                _visitedBoards.Add(newBoard);
                if (_breakFlag)
                {
                    break;
                }

                depth++;
                BuildBranchesAndBoundsTree(newNode, finalBoard, 1 - startColor, ref depth, ref step);
                if (_breakFlag)
                {
                    break;
                }
            }
            if (_breakFlag)
            {
                break;
            }
        }
    }
}