namespace TreeDrawer;

public sealed partial class TreeWindow
{
    public TreeWindow()
    {
        InitializeComponent();
    }

    public void DrawTree<TNode>(TreeNode<TNode> root, double maxKpd)
    {
        KpdField.Text += maxKpd;
        GraphLayout.Graph = GraphBuilder<TNode>.Build(root);
    }
}