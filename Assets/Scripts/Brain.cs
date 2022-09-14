using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class Brain
{
    public static readonly string brainsPath = Application.persistentDataPath + "/Brains";

    [Serializable]
    public class Node
    {
        public float currentValue;
        public List<float> weights;

        public Node(float _currentValue, List<float> _weights)
        {
            currentValue = _currentValue;
            weights = _weights;
        }

        public Node(float _currentValue, int weightCount)
        {
            currentValue = _currentValue;

            int weightLen = Mathf.Max(weightCount, 1);
            weights = new List<float>(weightLen);
            for (int i = 0; i < weightLen; i++)
            {
                weights.Add(0f);
            }
        }
    }

    // a wrapper class for columns in the matrix of hidden layer nodes
    // this allows the data to be properly serialized
    [Serializable]
    public class MiddleNodeCol : IEnumerable<Node>
    {
        public List<Node> nodeCol;

        public int Count
        {
            get
            {
                return nodeCol.Count;
            }
        }

        public MiddleNodeCol(int rows)
        {
            nodeCol = new List<Node>(rows);
            for (int i = 0; i < rows; i++)
            {
                nodeCol.Add(null);
            }
        }

        public MiddleNodeCol(List<Node> nodes)
        {
            nodeCol = new List<Node>(nodes.Count);
            for (int i = 0; i < nodes.Count; i++)
            {
                nodeCol.Add(nodes[i]);
            }
        }

        public Node this[int index]
        {
            get => nodeCol[index];
            set => nodeCol[index] = value;
        }

        public IEnumerator<Node> GetEnumerator()
        {
            return nodeCol.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

    }

    public List<Node> inputNodes { get; private set; }
    public List<MiddleNodeCol> middleNodes { get; private set; }
    public List<float> outputValues { get; private set; }

    public Brain(int inputNodeCount, int middleNodeCols, int middleNodeRows, int outputValueCount)
    {
        int inNodeCount = Mathf.Max(inputNodeCount, 0);
        int midNodeCols = Mathf.Max(middleNodeCols, 1);
        int midNodeRows = Mathf.Max(middleNodeRows, 1);
        int outValCount = Mathf.Max(outputValueCount, 1);

        inputNodes = new List<Node>(inNodeCount + 1);
        for (int i = 0; i < inNodeCount; i++)
        {
            inputNodes.Add(new Node(0f, midNodeRows));
        }
        inputNodes.Add(new Node(1f, midNodeRows));

        middleNodes = new List<MiddleNodeCol>(midNodeCols);
        for (int col = 0; col < midNodeCols; col++)
        {
            middleNodes.Add(new MiddleNodeCol(midNodeRows));
            for (int row = 0; row < midNodeRows; row++)
            {
                middleNodes[col][row] = new Node(0f, col == midNodeCols - 1 ? outValCount : midNodeRows);
            }
        }

        outputValues = new List<float>(outValCount);
        for (int i = 0; i < outValCount; i++)
        {
            outputValues.Add(0f);
        }
    }

    public Brain(Brain other)
    {
        inputNodes = other.inputNodes;
        middleNodes = other.middleNodes;
        outputValues = other.outputValues;
    }

    public static string GetFilepath(string name)
    {
        return $"{brainsPath}/{name}.brain";
    }

    public void SaveAs(string name)
    {
        if (!Directory.Exists(brainsPath))
        {
            Directory.CreateDirectory(brainsPath);
        }

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(GetFilepath(name), FileMode.Create);
        formatter.Serialize(stream, this);
        stream.Close();
    }

    public static Brain Load(string name)
    {
        string path = GetFilepath(name);

        if (!File.Exists(path))
        {
            return null;
        }

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Open);
        Brain brain;

        try
        {
            brain = formatter.Deserialize(stream) as Brain;
        }
        catch
        {
            stream.Close();
            return null;
        }

        stream.Close();

        return brain;
    }

    public void Randomize()
    {
        // randomize input node weights
        foreach (Node inputNode in inputNodes)
        {
            for (int weightIndex = 0; weightIndex < inputNode.weights.Count; weightIndex++)
            {
                inputNode.weights[weightIndex] = UnityEngine.Random.Range(-1f, 1f);
            }
        }

        // randomize middle node weights
        foreach (MiddleNodeCol middleNodeCol in middleNodes)
        {
            foreach (Node middleNode in middleNodeCol)
            {
                for (int weightIndex = 0; weightIndex < middleNode.weights.Count; weightIndex++)
                {
                    middleNode.weights[weightIndex] = UnityEngine.Random.Range(-1f, 1f);
                }
            }
        }
    }

    // a function that takes any float and returns a float between 1 and -1
    public static float Sigmoid(float num)
    {
        return Mathf.Atan(num);
    }

    public bool SetInputs(List<float> inputs)
    {
        // inputs.Count should always be 1 less than inputNodes.Count
        // because there is always 1 input node whose value is a constant 1
        if (inputs.Count != inputNodes.Count - 1)
        {
            return false;
        }

        for (int i = 0; i < inputs.Count; i++)
        {
            inputNodes[i].currentValue = inputs[i];
        }
        return true;
    }

    // runs the assigned inputs through the network
    // and assigns outputs
    public void Think()
    {
        // send inputs to the first column of the middle layer
        for (int midNodeIndex = 0; midNodeIndex < middleNodes[0].Count; midNodeIndex++)
        {
            middleNodes[0][midNodeIndex].currentValue = 0f;
            foreach (Node inputNode in inputNodes)
            {
                middleNodes[0][midNodeIndex].currentValue += inputNode.currentValue * inputNode.weights[midNodeIndex];
            }
        }
        for (int i = 0; i < middleNodes[0].Count; i++)
        {
            middleNodes[0][i].currentValue = Brain.Sigmoid(middleNodes[0][i].currentValue);
        }

        // send middle layer columns through other middle layer columns
        for (int midNodeColIndex = 1; midNodeColIndex < middleNodes.Count; midNodeColIndex++)
        {
            for (int midNodeRowIndex = 0; midNodeRowIndex < middleNodes[0].Count; midNodeRowIndex++)
            {
                middleNodes[midNodeColIndex][midNodeRowIndex].currentValue = 0f;
                for (int otherMidNodeRowIndex = 0; otherMidNodeRowIndex < middleNodes[0].Count; otherMidNodeRowIndex++)
                {
                    middleNodes[midNodeColIndex][midNodeRowIndex].currentValue += middleNodes[midNodeColIndex - 1][otherMidNodeRowIndex].currentValue * middleNodes[midNodeColIndex - 1][otherMidNodeRowIndex].weights[midNodeRowIndex];
                }
            }
            for (int i = 0; i < middleNodes[0].Count; i++)
            {
                middleNodes[midNodeColIndex][i].currentValue = Brain.Sigmoid(middleNodes[midNodeColIndex][i].currentValue);
            }
        }

        // send last column of middle layer to outputs
        for (int outValIndex = 0; outValIndex < outputValues.Count; outValIndex++)
        {
            outputValues[outValIndex] = 0f;
            for (int midNodeIndex = 0; midNodeIndex < middleNodes[0].Count; midNodeIndex++)
            {
                outputValues[outValIndex] += middleNodes[middleNodes.Count - 1][midNodeIndex].currentValue * middleNodes[middleNodes.Count - 1][midNodeIndex].weights[outValIndex];
            }
        }
        for (int i = 0; i < outputValues.Count; i++)
        {
            outputValues[i] = Brain.Sigmoid(outputValues[i]);
        }
    }

    public void Mutate(float range)
    {
        float rng = Mathf.Abs(range);

        // mutate inputs
        foreach (Node inputNode in inputNodes)
        {
            for (int weightIndex = 0; weightIndex < inputNode.weights.Count; weightIndex++)
            {
                inputNode.weights[weightIndex] = Mathf.Clamp(inputNode.weights[weightIndex] + UnityEngine.Random.Range(-rng, rng), -1f, 1f);
            }
        }

        // mutate middle layer
        foreach (MiddleNodeCol middleNodeCol in middleNodes)
        {
            foreach (Node middleNode in middleNodeCol)
            {
                for (int weightIndex = 0; weightIndex < middleNode.weights.Count; weightIndex++)
                {
                    middleNode.weights[weightIndex] = Mathf.Clamp(middleNode.weights[weightIndex] + UnityEngine.Random.Range(-rng, rng), -1f, 1f);
                }
            }
        }
    }
}
