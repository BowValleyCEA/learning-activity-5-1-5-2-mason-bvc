namespace HighScore;

public class DescendingOrderTree<T> where T : IComparable, IComparable<T>
{
    private class Node(T data)
    {
        public Node? Left, Right;
        public T Data = data;
    }

    private Node? _root;

    private void TraverseInorderDescending(Action<T> traversalDelegate, Node? node)
    {
        if (node is not null)
        {
            TraverseInorderDescending(traversalDelegate, node.Right);
            traversalDelegate.Invoke(node.Data);
            TraverseInorderDescending(traversalDelegate, node.Left);
        }
    }

    private Node Add(T data, Node? node)
    {
        if (node is null)
        {
            return new Node(data);
        }

        int comparison = data.CompareTo(node.Data);

        if (comparison < 0)
        {
            node.Left = Add(data, node.Left);
        }
        else if (comparison > 0)
        {
            node.Right = Add(data, node.Right);
        }

        return node;
    }

    public void Add(T data)
    {
        Node newNode = new(data);

        if (_root is null)
        {
            _root = newNode;
            return;
        }

        Add(data, _root);
    }

    // Yes this could have been implemented with IEnumerable and IEnumerator
    // but that's a too much boilerplate for me right now!

    public void Traverse(Action<T> traversalDelegate)
    {
        TraverseInorderDescending(traversalDelegate, _root);
    }
}
