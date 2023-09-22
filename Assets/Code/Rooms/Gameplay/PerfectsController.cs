using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerfectsController
{
    public enum ChunkResultType
    {
        None = 0,
        Positive = 1,
        Negative = 2
    }

    public event Action<ChunkResultType> OnChunkFinishedAction = (chunkResultType) => { };

    public int EmptyChunksCounter { get; private set; } = 0;
    public int FailedChunksCounter { get; private set; } = 0;
    public int SuccessChunksCounter { get; private set; } = 0;

    public int TotalChunksCounter
    {
        get
        {
            return SuccessChunksCounter + FailedChunksCounter + EmptyChunksCounter;
        }
    }

    public float LevelFillingProgress
    {
        get
        {
            return (float)SuccessChunksCounter / (float)(TotalChunksCounter);
        }
    }

    public int LevelStarsCount
    {
        get
        {
            if (TotalChunksCounter == EmptyChunksCounter) return 0;

            if (LevelFillingProgress >= 0.8f) return 3;
            if (LevelFillingProgress >= 0.5f) return 2;
            return 1;
        }
    }

    public bool AddResult(float filling, bool agregateTuResults = true)
    {
        float minFillingForGoodResult = (Env.Instance.Rooms.GameplayRoom.Config as GameplayRoomConfig).MinFillingForGoodResult;

        if (filling >= minFillingForGoodResult)
        {
            if (agregateTuResults) SuccessChunksCounter++;
            OnChunkFinishedAction.Invoke(ChunkResultType.Positive);
            return true;
        }
        else
        {
            if (Mathf.Approximately(filling, 0.0f))
            {
                if (agregateTuResults) EmptyChunksCounter++;
                OnChunkFinishedAction.Invoke(ChunkResultType.Negative);
                return false;
            }
            else
            {
                if (agregateTuResults) FailedChunksCounter++;
                OnChunkFinishedAction.Invoke(ChunkResultType.Negative);
                return false;
            }
        }
    }
}
