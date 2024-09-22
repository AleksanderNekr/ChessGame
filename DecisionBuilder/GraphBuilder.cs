using Graphviz4Net.Graphs;

namespace DecisionBuilder;

internal abstract class GraphBuilder<TNode>
{
    private static Graph<TNode> _graph = null!;

    public static Graph<TNode> Build(TreeNode<TNode> root)
    {
        _graph = new Graph<TNode>();
        _graph.AddVertex(root.Value);

        AddChildren(_graph, root);

        return _graph;
    }

    private static void AddChildren(Graph<TNode> graph, TreeNode<TNode> root)
    {
        if (root.Children is null || !root.Children.Any())
        {
            return;
        }

        foreach (var child in root.Children)
        {
            graph.AddVertex(child.Value);
            graph.AddEdge(new Edge<TNode>(root.Value, child.Value));
            AddChildren(graph, child);
        }
    }
}

public sealed record TreeNode<T>(T Value, IEnumerable<TreeNode<T>>? Children);