using System.Collections.Generic;

public static class LinkedListExtensions
{
    public static void AppendRange<T>(this LinkedList<T> linkedList, IEnumerable<T> elementsToAdd)
    {
        foreach (T element in elementsToAdd)
            linkedList.AddLast(element);
    }
}