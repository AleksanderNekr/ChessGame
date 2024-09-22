namespace DecisionBuilder;

public sealed partial class TreeWindow
{
    public TreeWindow()
    {
        InitializeComponent();
    }
    
    public void DrawTree<TNode>(TreeNode<TNode> root)
    {
        GraphLayout.Graph = GraphBuilder<TNode>.Build(root);
    }
}