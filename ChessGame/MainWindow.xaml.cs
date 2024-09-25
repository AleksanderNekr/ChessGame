using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ChessGame.GameClasses;
using TreeDrawer;

namespace ChessGame;

/// <inheritdoc cref="System.Windows.Window" />
internal sealed partial class MainWindow
{
    private ChessBoard _board;
    private ChessBoard? _solutionBoard;
    private PieceColor? _startColor;

    public MainWindow()
    {
        InitializeComponent();
        _board = ChessBoard.Init(SetDefaultPreset);

        _board.AfterBoardChanged += AfterBoardChangedHandle;
        ValidMove.ShowValidMove += ShowValidMoveShowValidMove;

        _solutionBoard = ChessBoard.Init(board =>
        {
            _ = new King(board, PieceColor.Black, 0, 4);
            _ = new King(board, PieceColor.White, 2, 4);
            _ = new Rook(board, PieceColor.White, 0, 7);
        });
        _startColor = PieceColor.White;
        AfterBoardChangedHandle();
    }

    private void AfterBoardChangedHandle()
    {
        BoardPresenter.ApplyBoardLayout(_board);
    }

    private void ShowValidMoveShowValidMove(ValidMove sender, EventArgs e)
    {
        BoardPresenter.SetPieceToBoard(sender, sender.Coordinate.Row, sender.Coordinate.Column);
    }

    private void ButtonBase_Click(object sender, RoutedEventArgs e)
    {
        _board = ChessBoard.Init(b =>
        {
            SetDefaultPreset(b);
            b.AfterBoardChanged += AfterBoardChangedHandle;
        });
        AfterBoardChangedHandle();
    }

    private static void SetDefaultPreset(ChessBoard board)
    {
        SetPawns(board);
        SetKnights(board);
        SetBishops(board);
        SetRooks(board);
        SetQueens(board);
        SetKings(board);
    }

    private static void SetKings(ChessBoard board)
    {
        _ = new King(board, PieceColor.Black, 0, 4);
        _ = new King(board, PieceColor.White, 7, 4);
    }

    private static void SetQueens(ChessBoard board)
    {
        _ = new Queen(board, PieceColor.Black, 0, 3);
        _ = new Queen(board, PieceColor.White, 7, 3);
    }

    private static void SetRooks(ChessBoard board)
    {
        _ = new Rook(board, PieceColor.Black, 0, 0);
        _ = new Rook(board, PieceColor.Black, 0, 7);
        _ = new Rook(board, PieceColor.White, 7, 0);
        _ = new Rook(board, PieceColor.White, 7, 7);
    }

    private static void SetBishops(ChessBoard board)
    {
        _ = new Bishop(board, PieceColor.White, 7, 2);
        _ = new Bishop(board, PieceColor.White, 7, 5);
        _ = new Bishop(board, PieceColor.Black, 0, 2);
        _ = new Bishop(board, PieceColor.Black, 0, 5);
    }

    private static void SetKnights(ChessBoard board)
    {
        _ = new Knight(board, PieceColor.White, 7, 1);
        _ = new Knight(board, PieceColor.White, 7, 6);
        _ = new Knight(board, PieceColor.Black, 0, 1);
        _ = new Knight(board, PieceColor.Black, 0, 6);
    }

    private static void SetPawns(ChessBoard board)
    {
        for (var i = 0; i < 8; i++)
        {
            _ = new Pawn(board, PieceColor.White, 6, i);
        }

        for (var i = 0; i < 8; i++)
        {
            _ = new Pawn(board, PieceColor.Black, 1, i);
        }
    }

    private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        var newSize = Math.Min(e.NewSize.Width, e.NewSize.Height) - 140;
        BoardPresenter.Width = newSize;
        BoardPresenter.Height = newSize;
    }

    private void ShowTree_Click(object sender, RoutedEventArgs e)
    {
        if (_solutionBoard is null || _startColor is null)
        {
            MessageBox.Show("Сначала выберите задачу");
            return;
        }

        var newBoard = _board.Clone();
        DecisionsMaker decisionsMaker = new();
        var boardsTree = decisionsMaker.BuildDecisionTree(newBoard, _solutionBoard, _startColor.Value);

        var grid = BoardPresenter.Clone();
        var hField = TextBlock(0, "H = {0}");
        var gField = TextBlock(0, "G = {0}");
        var num = TextBlock(0, "№ {0}");
        var fField = TextBlock(0, "F = {0}");
        var gridsTree = new TreeNode<StackPanel>(NodePanel(grid, num, hField, gField, fField), new List<TreeNode<StackPanel>>());
        gridsTree = BoardsToGrids(boardsTree, gridsTree);

        DrawGraph(gridsTree);
    }

    private TreeNode<StackPanel> BoardsToGrids(TreeNode<VisualNodeContainer> root, TreeNode<StackPanel> gridsTree)
    {
        var boardRoot = gridsTree.Value;
        foreach (var child in root.Children)
        {
            var boardLayout = boardRoot.Children.OfType<Grid>().Single().Clone().ApplyBoardLayout(child.Value.Board);
            var hField = TextBlock(child.Value.HNumber, "H = {0}");
            var gField = TextBlock(child.Value.GNumber, "G = {0}");
            var num = TextBlock(child.Value.Step, "№ {0}");
            var fField = TextBlock(child.Value.FNumber, "F = {0}");
            var gridNode = new TreeNode<StackPanel>(NodePanel(boardLayout, num, hField, gField, fField), new List<TreeNode<StackPanel>>());
            gridsTree.AddChild(gridNode);

            BoardsToGrids(child, gridNode);
        }

        return gridsTree;
    }

    private void DrawGraph<T>(TreeNode<T> root) where T : FrameworkElement
    {
        var treeWindow = new TreeWindow();

        treeWindow.DrawTree(root);

        ValidMove.ShowValidMove -= ShowValidMoveShowValidMove;

        treeWindow.Closing += (_, _) =>
        {
            ValidMove.ShowValidMove += ShowValidMoveShowValidMove;
        };

        treeWindow.ShowDialog();
    }

    private void Mate1MoveEasy_Click(object sender, RoutedEventArgs e)
    {
        _startColor = PieceColor.White;
        _board = ChessBoard.Init(b =>
        {
            _ = new King(b, PieceColor.Black, 0, 4);
            _ = new King(b, PieceColor.White, 2, 4);
            _ = new Rook(b, PieceColor.White, 7, 7);
            b.AfterBoardChanged += AfterBoardChangedHandle;
        });
        AfterBoardChangedHandle();

        _solutionBoard = ChessBoard.Init(board =>
        {
            _ = new King(board, PieceColor.Black, 0, 4);
            _ = new King(board, PieceColor.White, 2, 4);
            _ = new Rook(board, PieceColor.White, 0, 7);
        });
    }

    private static StackPanel NodePanel(Grid board, params FrameworkElement[] elements)
    {
        var panel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Margin = new Thickness(0),
            Background = new SolidColorBrush(Colors.Bisque),
        };
        var leftPanel = new StackPanel
        {
            Orientation = Orientation.Vertical,
            VerticalAlignment = VerticalAlignment.Center,
        };
        panel.Children.Add(leftPanel);
        panel.Children.Add(board);
        foreach (var element in elements)
        {
            element.Margin = new Thickness(20, 5, 20, 5);
            if (element is TextBlock textBlock)
            {
                textBlock.FontSize = 20;
                textBlock.FontWeight = FontWeights.SemiBold;
            }
            leftPanel.Children.Add(element);
        }

        return panel;
    }

    private static TextBlock TextBlock<T>(T element, string? template = null) where T : notnull
        => new() { Text = template is null ? element.ToString() : string.Format(template, element) };
}