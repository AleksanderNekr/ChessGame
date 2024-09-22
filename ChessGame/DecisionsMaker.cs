using System;
using System.Collections.Generic;
using System.Linq;
using ChessGame.GameClasses;
using TreeDrawer;

namespace ChessGame;

public class DecisionsMaker
{
    private const int DepthLimit = 10;
    private readonly HashSet<ChessBoard> _visitedBoards = new();
    private bool _breakFlag;

    public TreeNode<ChessBoard> BuildDecisionTree(ChessBoard currentBoard, ChessBoard finalBoard, PieceColor startColor)
    {
        _visitedBoards.Clear();
        _visitedBoards.Add(currentBoard);
        var root = new TreeNode<ChessBoard>(currentBoard, new List<TreeNode<ChessBoard>>());
        int depth = 0;
        _breakFlag = false;
        BuildBranchesAndBoundsTree(root, finalBoard, startColor, ref depth);
        return root;
    }

    private void BuildBranchesAndBoundsTree(TreeNode<ChessBoard> node, ChessBoard finalBoard, PieceColor startColor, ref int depth)
    {
        if (node.Value.Equals(finalBoard) || _breakFlag)
        {
            _breakFlag = true;
            return;
        }

        if (depth >= DepthLimit)
        {
            return;
        }

        // All possible moves from current position for color
        foreach (var piece in node.Value.GetPlayerPieces(startColor))
        {
            foreach (var move in piece.GetValidMoves())
            {
                var newBoard = node.Value.Clone();
                newBoard.MovePiece(newBoard.GetPieceOrNull(piece.Coordinate)!, move.Row, move.Column);

                if (_visitedBoards.Any(x => x.Equals(newBoard)))
                {
                    continue;
                }

                depth++;
                var newNode = new TreeNode<ChessBoard>(newBoard, new List<TreeNode<ChessBoard>>());
                node.AddChild(newNode);
                if (_breakFlag)
                {
                    break;
                }
                BuildBranchesAndBoundsTree(newNode, finalBoard, 1 - startColor, ref depth);
            }
            if (_breakFlag)
            {
                break;
            }
        }
    }
}