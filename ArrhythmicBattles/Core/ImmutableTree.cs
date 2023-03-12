using System.Collections;

namespace ArrhythmicBattles.Core;

public class TreeBuilder<T>
{
    private T value;
    private List<TreeBuilder<T>> children = new List<TreeBuilder<T>>();

    public TreeBuilder(T value)
    {
        this.value = value;
    }

    public TreeBuilder<T> PushChild(TreeBuilder<T> childBuilder)
    {
        children.Add(childBuilder);
        return this;
    }

    public TreeBuilder<T> PushChild(T value, Action<TreeBuilder<T>>? childBuilderConsumer = null)
    {
        TreeBuilder<T> childBuilder = new TreeBuilder<T>(value);
        childBuilderConsumer?.Invoke(childBuilder);
        return PushChild(childBuilder);
    }

    public ImmutableTree<T> Build()
    {
        ImmutableNode<T> rootNode = new ImmutableNode<T>(value, GetChildren(this));
        return new ImmutableTree<T>(rootNode);
    }

    private static List<ImmutableNode<T>> GetChildren(TreeBuilder<T> parent)
    {
        List<ImmutableNode<T>> children = new List<ImmutableNode<T>>();

        foreach (TreeBuilder<T> child in parent.children)
        {
            children.Add(new ImmutableNode<T>(child.value, GetChildren(child)));
        }

        return children;
    }
}

public class ImmutableNode<T> : IEnumerable<ImmutableNode<T>>
{
    public T Value { get; }
    public IReadOnlyList<ImmutableNode<T>> Children => children;

    private readonly List<ImmutableNode<T>> children;

    public ImmutableNode(T value, IEnumerable<ImmutableNode<T>>? children)
    {
        Value = value;
        this.children = children == null ? new List<ImmutableNode<T>>() : children.ToList();
    }

    public IEnumerator<ImmutableNode<T>> GetEnumerator()
    {
        Stack<ImmutableNode<T>> stack = new Stack<ImmutableNode<T>>();
        stack.Push(this);

        while (stack.Count > 0)
        {
            ImmutableNode<T> node = stack.Pop();
            yield return node;

            for (int i = node.Children.Count - 1; i >= 0; i--)
            {
                stack.Push(node.Children[i]);
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public class ImmutableTree<T> : IEnumerable<T>
{
    public ImmutableNode<T> RootNode { get; }

    public ImmutableTree(ImmutableNode<T> rootNode)
    {
        RootNode = rootNode;
    }

    public IEnumerator<T> GetEnumerator()
    {
        foreach (ImmutableNode<T> node in RootNode)
        {
            yield return node.Value;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}