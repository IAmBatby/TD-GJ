using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utilities
{
    public static float GetScaledValue(float value, float scaleRate, int waveCount)
    {
        return (value + (waveCount + (scaleRate / 100)));
    }
}
