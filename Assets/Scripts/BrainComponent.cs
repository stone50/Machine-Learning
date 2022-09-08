using UnityEngine;

public class BrainComponent : MonoBehaviour
{
    [Tooltip("Used when saving and loading from a file")]
    public string brainName = "New Brain";
    public Brain brain;

    void Update()
    {
        if (Time.frameCount % 60 == 0)
        {
            brain.SetInputs(new float[] { UnityEngine.Random.Range(-1f, 1f) });
            brain.Think();
        }
    }

    public void SaveBrain()
    {
        brain.SaveAs(brainName);
    }

    #region UNITY_EDITOR
#if UNITY_EDITOR
    public int inputCount = 0;
    public int middleColCount = 1;
    public int middleRowCount = 1;
    public int outputCount = 1;

    public int radioIndex = 0;

    public void InitBrain()
    {
        switch (radioIndex)
        {
            case 0: // random
                brain = new Brain(inputCount, middleColCount, middleRowCount, outputCount);
                brain.Randomize();
                break;

            case 1: // load from file
                brain = Brain.Load(brainName);
                break;
        }
    }

    void Awake()
    {
        InitBrain();
    }
#endif  // UNITY_EDITOR
    #endregion UNITY_EDITOR
}