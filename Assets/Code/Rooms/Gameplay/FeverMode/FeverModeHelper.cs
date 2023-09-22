using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeverModeHelper
{
    private const float MIN_CHUNK_FILLING = 0.5f;

    public bool IsActuallyEnabled { get; private set; } = false;

    private int totalFeverShapesCount;
    private int goodFillingCount = 0;

    public bool Start(int totalFeverShapesCount)
    {
        if (IsActuallyEnabled) return false;

        this.totalFeverShapesCount = totalFeverShapesCount;
        goodFillingCount = 0;

        IsActuallyEnabled = true;
        return true;
    }

    public void OnChunkFinished(ShapesChunk chunk, float filling, out string shapeCaption)
    {
        shapeCaption = null;
        if (!IsActuallyEnabled) return;

        bool goodFilling = (filling > MIN_CHUNK_FILLING);

        if (goodFilling)
        {
            goodFillingCount++;
            shapeCaption = goodFillingCount.ToString() + "/" + totalFeverShapesCount.ToString();
        }
    }

    public void Finish(out int prizeCoinsCount, out bool isPerfect)
    {
        prizeCoinsCount = 0;
        isPerfect = false;

        if (!IsActuallyEnabled) return;
        IsActuallyEnabled = false;

        prizeCoinsCount = goodFillingCount;
        if (goodFillingCount == totalFeverShapesCount)
        {
            isPerfect = true;
            prizeCoinsCount = (int)(prizeCoinsCount * 1.5f);
        }
            

    }
}
