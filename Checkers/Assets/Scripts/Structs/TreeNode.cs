using System.Collections.Generic;

public class TreeNode<T>
{
    public T Data { get; set; }
    public TreeNode<T> Parent { get; private set; }
    public LinkedList<TreeNode<T>> Children { get; }

    public TreeNode(T data)
    {
        Data = data;
        Children = new LinkedList<TreeNode<T>>();
    }

    public TreeNode<T> AddChild(T child)
    {
        TreeNode<T> childNode = new TreeNode<T>(child) {Parent = this};
        Children.AddLast(childNode);
        return childNode;
    }

    public bool RemoveChild(TreeNode<T> child)
    {
        return Children.Remove(child);
    }
}