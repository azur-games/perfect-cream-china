using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangesFloat
{
    private class Range
    {
        public float Min { get; set; }
        public float Max { get; set; }

        public Range(float min, float max)
        {
            Min = min;
            Max = max;
        }

        public void Extend(float val)
        {
            if (Min > val) Min = val;
            if (Max < val) Max = val;
        }

        public float Length
        {
            get
            {
                return (Max - Min);
            }
        }
    }

    private List<Range> ranges = new List<Range>();

    public RangesFloat()
    {

    }

    public void Add(float min, float max)
    {
        Range newRange = new Range(min, max);

        // add first
        if (ranges.Count == 0)
        {
            ranges.Add(newRange);
            return;
        }

        int newRangeIndex = 0;

        for (int r = (ranges.Count - 1); r >= 0; r--)
        {
            if (ranges[r].Min < newRange.Min)
            {
                newRangeIndex = r + 1;
                break;
            }
        }

        if (newRangeIndex == ranges.Count)
        {
            ranges.Add(newRange);
        }
        else
        {
            ranges.Insert(newRangeIndex, newRange);
        }

        int checkingIndex;

        // combine ranges forward
        checkingIndex = newRangeIndex + 1;
        while (checkingIndex < ranges.Count)
        {
            Range checkingRange = ranges[checkingIndex];
            if (checkingRange.Min < newRange.Max)
            {
                newRange.Extend(checkingRange.Min);
                newRange.Extend(checkingRange.Max);
                ranges.RemoveAt(checkingIndex);
            }
            else
            {
                break;
            }
        }

        // combine ranges backward
        checkingIndex = newRangeIndex - 1;
        while (checkingIndex >= 0)
        {
            Range checkingRange = ranges[checkingIndex];
            if (checkingRange.Max > newRange.Min)
            {
                newRange.Extend(checkingRange.Min);
                newRange.Extend(checkingRange.Max);
                ranges.RemoveAt(checkingIndex);
            }
            else
            {
                break;
            }

            checkingIndex--;
        }
    }

    public float GetSummLength()
    {
        float length = 0.0f;

        foreach (Range range in ranges)
        {
            length += range.Length;
        }

        return length;
    }
}
