using System.Collections.Generic;
using UnityEngine;

public class PrintFPS : MonoBehaviour
{
    private Queue<float> recentFPSs = new Queue<float>();
    private float maxFPS = 0f;
    private float minFPS = Mathf.Infinity;

    void Update()
    {
        float fps = 1f / Time.deltaTime;

        recentFPSs.Enqueue(fps);
        if (recentFPSs.Count > 300)
        {
            recentFPSs.Dequeue();
        }

        maxFPS = Mathf.Max(maxFPS, fps);
        minFPS = Mathf.Min(minFPS, fps);

        float avgFPS = 0f;
        foreach (float fpsInQueue in recentFPSs)
        {
            avgFPS += fpsInQueue;
        }
        avgFPS /= recentFPSs.Count;

        print($"Average FPS: {avgFPS}\nFPS: {fps}\nMin FPS: {minFPS}\nMax FPS: {maxFPS}");
    }
}
