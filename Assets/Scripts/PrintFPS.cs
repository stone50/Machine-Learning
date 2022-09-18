using System.Collections.Generic;
using UnityEngine;

public class PrintFPS : MonoBehaviour
{
    private Queue<float> updateFPSs = new Queue<float>();
    private Queue<float> fixedUpdateFPSs = new Queue<float>();

    public bool updateAverageFPS = true;
    public bool fixedUpdateAverageFPS = true;

    void Update()
    {
        float fps = 1f / Time.deltaTime;

        if (!updateAverageFPS)
        {
            print($"Update FPS: {fps}");
            return;
        }

        updateFPSs.Enqueue(fps);

        while (updateFPSs.Count > fps)
        {
            updateFPSs.Dequeue();
        }

        float avgFPS = 0f;
        foreach (float fpsInQueue in updateFPSs)
        {
            avgFPS += fpsInQueue;
        }
        avgFPS /= updateFPSs.Count;

        print($"Update Average FPS: {avgFPS}");
    }

    void FixedUpdate()
    {
        float fps = 1f / Time.deltaTime;

        if (!fixedUpdateAverageFPS)
        {
            print($"Fixed Update FPS: {fps}");
            return;
        }

        fixedUpdateFPSs.Enqueue(fps);

        while (fixedUpdateFPSs.Count > fps)
        {
            fixedUpdateFPSs.Dequeue();
        }

        float avgFPS = 0f;
        foreach (float fpsInQueue in fixedUpdateFPSs)
        {
            avgFPS += fpsInQueue;
        }
        avgFPS /= fixedUpdateFPSs.Count;

        print($"Fixed Update Average FPS: {avgFPS}");
    }
}
