using System.Collections.Generic;

public class TreeNode<T>
{
    public T Value { get; set; }
    public LinkedList<TreeNode<T>> Children { get; }

    public TreeNode(T value)
    {
        Value = value;
        Children = new LinkedList<TreeNode<T>>();
    }

    public TreeNode<T> AddChild(T child)
    {
        TreeNode<T> childNode = new TreeNode<T>(child);
        Children.AddLast(childNode);
        return childNode;
    }

    public bool RemoveChild(TreeNode<T> child)
    {
        return Children.Remove(child);
    }
}