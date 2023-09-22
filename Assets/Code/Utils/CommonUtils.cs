using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CommonUtils
{
    public enum ElementsEntry
    {
        None,
        FirstIsAPartOfSecond,
        SecondIsAPartOfFirst,
        Equals,
    }

    public static bool Equals<T>(T a, T b)
    {
        return EqualityComparer<T>.Default.Equals(a, b);
    }

    public static ElementsEntry GetListsElementsEntry<T>(List<T> first, List<T> second)
    {
        int index = 0;
        int firstSetCount = first.Count;
        int secondSetCount = second.Count;

        while (true)
        {
            bool firstSetFinished = (index == firstSetCount);
            bool secondSetFinished = (index == secondSetCount);

            if (firstSetFinished)
            {
                if (secondSetFinished)
                {
                    return ElementsEntry.Equals;
                }
                else
                {
                    return ElementsEntry.FirstIsAPartOfSecond;
                }
            }
            else
            {
                if (secondSetFinished)
                {
                    return ElementsEntry.SecondIsAPartOfFirst;
                }
                else
                {
                    if (!Equals<T>(first[index], second[index]))
                    {
                        return ElementsEntry.None;
                    }
                }
            }
            
            index++;
        }
    }
}
