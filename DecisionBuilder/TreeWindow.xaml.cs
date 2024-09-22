using System.Windows;

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

    private void SizeSlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        GraphLayout?.Graph?.Vertices.ToList().ForEach(v =>
        {
            if (v is not FrameworkElement control)
            {
                return;
            }

            control.Width = (int)e.NewValue;
            control.Height = (int)e.NewValue;
        });
    }
}