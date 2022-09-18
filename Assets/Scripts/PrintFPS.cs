using System.Collections.Generic;
using UnityEngine;

public class PrintFPS : MonoBehaviour
{
    private Queue<float> updateFPSs = new Queue<float>();
    private Queue<float> fixedUpdateFPSs = new Queue<float>();

    public bool updateFPS = true;
    public bool updateAverageFPS = true;
    public bool fixedUpdateFPS = true;
    public bool fixedUpdateAverageFPS = true;

    void Update()
    {
        if (!updateFPS)
        {
            return;
        }

        float fps = 1f / Time.deltaTime;

        if (!updateAverageFPS)
        {
            print($"Update FPS: {fps}");
            return;
        }

        while (updateFPSs.Count > fps)
        {
            updateFPSs.Dequeue();
        }

        updateFPSs.Enqueue(fps);

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
        if (!fixedUpdateFPS)
        {
            return;
        }

        float fps = 1f / Time.fixedDeltaTime;

        if (!fixedUpdateAverageFPS)
        {
            print($"Fixed Update FPS: {fps}");
            return;
        }

        while (fixedUpdateFPSs.Count > fps)
        {
            fixedUpdateFPSs.Dequeue();
        }

        fixedUpdateFPSs.Enqueue(fps);

        float avgFPS = 0f;
        foreach (float fpsInQueue in fixedUpdateFPSs)
        {
            avgFPS += fpsInQueue;
        }
        avgFPS /= fixedUpdateFPSs.Count;

        print($"Fixed Update Average FPS: {avgFPS}");
    }
}
