using System;
using System.Collections.Generic;
using ChessGame.GameClasses;
using TreeDrawer;

namespace ChessGame;

public class DecisionsMaker
{
    public TreeNode<ChessBoard> BuildDecisionTree(ChessBoard currentBoard, ChessBoard finalBoard)
    {
        var root = new TreeNode<ChessBoard>(currentBoard, null);
        BuildTree(root, finalBoard);
        return root;
    }

    private void BuildTree(TreeNode<ChessBoard> node, ChessBoard finalBoard)
    {
        if (node.Value.Equals(finalBoard))
        {
            return;
        }

        
    }
}