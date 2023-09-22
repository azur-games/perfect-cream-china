using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathUtils
{
    public static float LerpFloat(float pA, float pB, float step, bool clamped = true)
    {
        float res = pA * (1.0f - step) + pB * step;

        if (clamped)
        {
            if (pA > pB)
            {
                res = Mathf.Clamp(res, pB, pA);
            }
            else
            {
                res = Mathf.Clamp(res, pA, pB);
            }
        }

        return res;
    }

    public static float LerpFloat(float stepA, float valA, float stepB, float valB, float step, bool clamped = true)
    {
        float normalizedStep = (step - stepA) / (stepB - stepA);
        return LerpFloat(valA, valB, normalizedStep, clamped);
    }
}
