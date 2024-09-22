using System.Windows;
using Graphviz4Net.Graphs;

namespace DecisionBuilder;

public sealed partial class TreeWindow
{
    public TreeWindow()
    {
        InitializeComponent();
    }

    public static Graph<TNode> NewGraph<TNode>()
        => new();

    public static void AddTreeRoot<TNode>(Graph<TNode> graph, TNode root)
    {
        graph.AddVertex(root);
    }

    public static void AddChild<TNode>(Graph<TNode> graph, TNode parent, TNode child)
    {
        graph.AddVertex(child);
        graph.AddEdge(new Edge<TNode>(parent, child));
    }

    public void BuildGraph<TNode>(Graph<TNode> graph)
    {
        GraphLayout.Graph = graph;
    }

    private void AddTree_OnClick(object sender, RoutedEventArgs e)
    {
        var graph = NewGraph<string>();
        AddTreeRoot(graph, "Root");

        for (var i = 0; i < 7; i++)
        {
            AddChild(graph, "Root", "Child " + i);

            for (var j = 0; j < 3; j++)
            {
                AddChild(graph, "Child " + i, "GrandChild " + i + "." + j);
            }
        }

        BuildGraph(graph);
    }
}