using System.Collections.Generic;
using System.Linq;
using ChessGame.GameClasses;
using TreeDrawer;

namespace ChessGame;

internal sealed class DecisionsMaker
{
    private const int DepthLimit = 10;
    private readonly HashSet<(ChessBoard VisitedBoard, int G)> _visitedBoards = new();
    private bool _breakFlag;
    private TreeNode<VisualNodeContainer> _root = null!;

    public TreeNode<VisualNodeContainer> BuildDecisionTree(ChessBoard currentBoard, ChessBoard finalBoard, PieceColor startColor)
    {
        _visitedBoards.Clear();
        _visitedBoards.Add((currentBoard, int.MaxValue));
        _root = new TreeNode<VisualNodeContainer>(new VisualNodeContainer(currentBoard, 0, 0, int.MaxValue), new List<TreeNode<VisualNodeContainer>>());
        int depth = 0;
        int step = 0;
        _breakFlag = false;
        BuildBranchesAndBoundsTree(_root, finalBoard, startColor, ref depth, ref step);
        return _root;
    }

    private void BuildBranchesAndBoundsTree(TreeNode<VisualNodeContainer> node, ChessBoard finalBoard, PieceColor startColor, ref int depth, ref int step)
    {
        while (true)
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

                    if (_visitedBoards.Any(x => x.VisitedBoard.Equals(newBoard)))
                    {
                        continue;
                    }

                    step++;
                    var newG = newBoard.CalculateDifferentCells(finalBoard);
                    var newNode = new TreeNode<VisualNodeContainer>(new VisualNodeContainer(newBoard, node.Value.HNumber + 1, step, newG), new List<TreeNode<VisualNodeContainer>>());
                    node.AddChild(newNode);
                    _visitedBoards.Add((newBoard, newG));
                    
                    if (newG == 0 || _breakFlag)
                    {
                        _breakFlag = true;
                        return;
                    }
                }
            }

            depth++;

            node = FindLeafWithMinF();
            startColor = 1 - startColor;
        }
    }

    private TreeNode<VisualNodeContainer> FindLeafWithMinF()
    {
        var queue = new Queue<TreeNode<VisualNodeContainer>>();
        queue.Enqueue(_root);
        var minF = int.MaxValue;
        TreeNode<VisualNodeContainer> minNode = null!;
        while (queue.Count > 0)
        {
            var node = queue.Dequeue();
            if (node.Value.FNumber < minF && node.Children.Count == 0)
            {
                minF = node.Value.GNumber;
                minNode = node;
            }

            foreach (var child in node.Children)
            {
                queue.Enqueue(child);
            }
        }

        return minNode;
    }
}