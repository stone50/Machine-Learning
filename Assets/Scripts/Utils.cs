using UnityEngine;

public static class Utils
{
    public static float RandomRange(float min, float max)
    {
        return UnityEngine.Random.Range(min, max);
    }
}