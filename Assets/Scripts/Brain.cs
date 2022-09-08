using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class Brain
{
    public static readonly string brainsPath = Application.persistentDataPath + "/Brains";

    [Serializable]
    public struct Node
    {
        public float currentValue;
        public float[] weights;

        public Node(float _currentValue, float[] _weights)
        {
            currentValue = _currentValue;
            weights = _weights;
        }
    }

    public Node[] inputNodes { get; private set; }
    public Node[,] middleNodes;
    public float[] outputValues { get; private set; }

    public Brain(int inputNodeCount, int middleNodeCols, int middleNodeRows, int outputValueCount)
    {
        int inNodeCount = Mathf.Max(inputNodeCount, 0);
        int midNodeCols = Mathf.Max(middleNodeCols, 1);
        int midNodeRows = Mathf.Max(middleNodeRows, 1);
        int outValCount = Mathf.Max(outputValueCount, 1);

        inputNodes = new Node[inNodeCount + 1];
        for (int i = 0; i < inNodeCount; i++)
        {
            inputNodes[i] = new Node(new float(), new float[midNodeRows]);
        }
        inputNodes[inNodeCount] = new Node(1f, new float[midNodeRows]);

        middleNodes = new Node[midNodeCols, midNodeRows];
        for (int col = 0; col < midNodeCols; col++)
        {
            for (int row = 0; row < midNodeRows; row++)
            {
                middleNodes[col, row] = new Node(new float(), new float[col == midNodeCols - 1 ? outValCount : midNodeRows]);
            }
        }

        outputValues = new float[outValCount];
    }

    public Brain(Node[] _inputNodes, Node[,] _middleNodes, float[] _outputValues)
    {
        inputNodes = _inputNodes;
        middleNodes = _middleNodes;
        outputValues = _outputValues;
    }

    public Brain(Brain other)
    {
        inputNodes = other.inputNodes;
        middleNodes = other.middleNodes;
        outputValues = other.outputValues;
    }

    public void SaveAs(string name)
    {
        if (!Directory.Exists(brainsPath))
        {
            Directory.CreateDirectory(brainsPath);
        }

        BinaryFormatter formatter = new BinaryFormatter();
        string path = brainsPath + "/" + name + ".brain";
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, this);
        stream.Close();
    }

    public static Brain Load(string name)
    {
        string path = brainsPath + "/" + name + ".brain";
        if (!File.Exists(path))
        {
            return null;
        }

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Open);
        Brain brain = formatter.Deserialize(stream) as Brain;
        stream.Close();

        return brain;
    }

    public void Randomize()
    {
        foreach (Node inputNode in inputNodes)
        {
            for (int weightIndex = 0; weightIndex < inputNode.weights.Length; weightIndex++)
            {
                inputNode.weights[weightIndex] = UnityEngine.Random.Range(-1f, 1f);
            }
        }

        foreach (Node middleNode in middleNodes)
        {
            for (int weightIndex = 0; weightIndex < middleNode.weights.Length; weightIndex++)
            {
                middleNode.weights[weightIndex] = UnityEngine.Random.Range(-1f, 1f);
            }
        }
    }

    public static float Sigmoid(float num)
    {
        return Mathf.Atan(num);
    }

    public bool SetInputs(float[] inputs)
    {
        if (inputs.Length != inputNodes.Length - 1)
        {
            return false;
        }

        for (int i = 0; i < inputs.Length; i++)
        {
            inputNodes[i].currentValue = inputs[i];
        }
        return true;
    }

    public void Think()
    {
        for (int midNodeIndex = 0; midNodeIndex < middleNodes.GetLength(1); midNodeIndex++)
        {
            middleNodes[0, midNodeIndex].currentValue = 0f;
            foreach (Node inputNode in inputNodes)
            {
                middleNodes[0, midNodeIndex].currentValue += inputNode.currentValue * inputNode.weights[midNodeIndex];
            }
        }
        for (int i = 0; i < middleNodes.GetLength(1); i++)
        {
            middleNodes[0, i].currentValue = Brain.Sigmoid(middleNodes[0, i].currentValue);
        }

        for (int midNodeColIndex = 1; midNodeColIndex < middleNodes.GetLength(0); midNodeColIndex++)
        {
            for (int midNodeRowIndex = 0; midNodeRowIndex < middleNodes.GetLength(1); midNodeRowIndex++)
            {
                middleNodes[midNodeColIndex, midNodeRowIndex].currentValue = 0f;
                for (int otherMidNodeRowIndex = 0; otherMidNodeRowIndex < middleNodes.GetLength(1); otherMidNodeRowIndex++)
                {
                    middleNodes[midNodeColIndex, midNodeRowIndex].currentValue += middleNodes[midNodeColIndex - 1, otherMidNodeRowIndex].currentValue * middleNodes[midNodeColIndex - 1, otherMidNodeRowIndex].weights[midNodeRowIndex];
                }
            }
            for (int i = 0; i < middleNodes.GetLength(1); i++)
            {
                middleNodes[midNodeColIndex, i].currentValue = Brain.Sigmoid(middleNodes[midNodeColIndex, i].currentValue);
            }
        }

        for (int outValIndex = 0; outValIndex < outputValues.Length; outValIndex++)
        {
            outputValues[outValIndex] = 0f;
            for (int midNodeIndex = 0; midNodeIndex < middleNodes.GetLength(1); midNodeIndex++)
            {
                outputValues[outValIndex] += middleNodes[middleNodes.GetLength(0) - 1, midNodeIndex].currentValue * middleNodes[middleNodes.GetLength(0) - 1, midNodeIndex].weights[outValIndex];
            }
        }
        for (int i = 0; i < outputValues.Length; i++)
        {
            outputValues[i] = Brain.Sigmoid(outputValues[i]);
        }
    }

    public void Mutate(float range)
    {
        float rng = Mathf.Abs(range);
        foreach (Node inputNode in inputNodes)
        {
            for (int weightIndex = 0; weightIndex < inputNode.weights.Length; weightIndex++)
            {
                inputNode.weights[weightIndex] = Mathf.Clamp(inputNode.weights[weightIndex] + UnityEngine.Random.Range(-rng, rng), -1f, 1f);
            }
        }

        foreach (Node middleNode in middleNodes)
        {
            for (int weightIndex = 0; weightIndex < middleNode.weights.Length; weightIndex++)
            {
                middleNode.weights[weightIndex] = Mathf.Clamp(middleNode.weights[weightIndex] + UnityEngine.Random.Range(-rng, rng), -1f, 1f);
            }
        }
    }
}
