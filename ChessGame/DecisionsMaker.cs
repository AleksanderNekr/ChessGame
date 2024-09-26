using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using ChessGame.GameClasses;
using TreeDrawer;

namespace ChessGame;

public sealed class DecisionsMaker
{
    private int _depthLimit;
    private readonly HashSet<(ChessBoard VisitedBoard, int G)> _visitedBoards = new();
    private bool _breakFlag;
    private TreeNode<VisualNodeContainer> _root = null!;
    private int _step;

    public async Task<TreeNode<VisualNodeContainer>> BuildDecisionTreeAsync(ChessBoard currentBoard, ChessBoard? finalBoard, int depthLimit, CancellationToken cancellationToken)
    {
        _visitedBoards.Clear();
        _visitedBoards.Add((currentBoard, int.MaxValue));
        _root = new TreeNode<VisualNodeContainer>(new VisualNodeContainer(currentBoard, 0, 0, int.MaxValue), new List<TreeNode<VisualNodeContainer>>());
        _step = 0;
        _breakFlag = false;
        _depthLimit = depthLimit;
        try
        {
            await BuildBranchesAndBoundsTreeAsync(_root, finalBoard, cancellationToken);
        }
        catch (OperationCanceledException e)
        {
            MessageBox.Show("Операция прервана пользователем");
            Console.WriteLine("Operation was canceled.");
            throw;
        }

        return _root;
    }

    private async Task BuildBranchesAndBoundsTreeAsync(TreeNode<VisualNodeContainer> node, ChessBoard? finalBoard, CancellationToken cancellationToken)
    {
        while (true)
        {
            if (FinishCondition(node.Value.Board, finalBoard) || _breakFlag)
            {
                _breakFlag = true;
                return;
            }

            if (node.Value.HNumber >= _depthLimit)
            {
                return;
            }

            // All possible moves from current position for color
            foreach (var piece in node.Value.Board.GetPlayerPieces(node.Value.Board.GetCurrentPlayer()))
            {
                foreach (var move in piece.GetValidMoves())
                {
                    var newBoard = node.Value.Board.Clone();
                    newBoard.MovePiece(newBoard.GetPieceOrNull(piece.Coordinate)!, move.Row, move.Column);

                    if (_visitedBoards.Any(x => x.VisitedBoard.Equals(newBoard)))
                    {
                        continue;
                    }

                    _step++;

                    try
                    {
                        var newG = finalBoard is not null
                            ? await newBoard.CalculateDifferentCellsAsync(finalBoard, cancellationToken)
                            : FinishCondition(newBoard, null)
                                ? 0
                                : 1;

                        var newNode = new TreeNode<VisualNodeContainer>(new VisualNodeContainer(newBoard, node.Value.HNumber + 1, _step, newG), new List<TreeNode<VisualNodeContainer>>());
                        node.AddChild(newNode);
                        _visitedBoards.Add((newBoard, newG));

                        if (newG == 0 || _breakFlag)
                        {
                            _breakFlag = true;
                            return;
                        }
                    }
                    catch (OperationCanceledException e)
                    {
                        _breakFlag = true;
                        return;
                    }
                }
            }

            node = FindLeafWithMinF();
        }
    }

    private static bool FinishCondition(ChessBoard board, ChessBoard? finalBoard)
        => (finalBoard is null && board.GetWinner() is not null) || (finalBoard is not null && board.Equals(finalBoard));

    private TreeNode<VisualNodeContainer> FindLeafWithMinF()
    {
        var queue = new Queue<TreeNode<VisualNodeContainer>>();
        queue.Enqueue(_root);
        var minF = int.MaxValue;
        TreeNode<VisualNodeContainer> minNode = null!;
        while (queue.Count > 0)
        {
            var node = queue.Dequeue();
            if ((node.Value.FNumber < minF || minNode is null) && node.Children.Count == 0)
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