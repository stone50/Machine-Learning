using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class BrainDisplay : MonoBehaviour
{
    public GameObject displayNodePrefab;

    [SerializeField]
    public BrainComponent brainComponent;

    public float inputNodeSize = 1f;
    public float inputNodeSpacing = 1.5f;
    public float inputToMiddleSpacing = 1.5f;
    public float middleNodeSize = 1f;
    public float middleNodeVerticalSpacing = 1.5f;
    public float middleNodeHorizontalSpacing = 1.5f;
    public float middleToOutputSpacing = 1.5f;
    public float outputNodeSize = 1f;
    public float outputNodeSpacing = 1.5f;
    public float inputToMiddleConnectionWidth = 0.05f;
    public float middleToMiddleConnectionWidth = 0.05f;
    public float middleToOutputConnectionWidth = 0.05f;

    [Serializable]
    private class MiddleDisplayNodeMatrix : IEnumerable<MiddleDisplayNodeCol>
    {
        public List<MiddleDisplayNodeCol> nodeMatrix;

        public MiddleDisplayNodeMatrix(int cols, int rows)
        {
            nodeMatrix = new List<MiddleDisplayNodeCol>(cols);
            for (int i = 0; i < cols; i++)
            {
                nodeMatrix.Add(new MiddleDisplayNodeCol(rows));
            }
        }

        public MiddleDisplayNodeCol this[int index]
        {
            get => nodeMatrix[index];
            set => nodeMatrix[index] = value;
        }

        public IEnumerator<MiddleDisplayNodeCol> GetEnumerator()
        {
            return nodeMatrix.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    [Serializable]
    private class MiddleDisplayNodeCol : IEnumerable<BrainDisplayNode>
    {
        public List<BrainDisplayNode> nodeCol;

        public MiddleDisplayNodeCol(int rows)
        {
            nodeCol = new List<BrainDisplayNode>(rows);
            for (int i = 0; i < rows; i++)
            {
                nodeCol.Add(null);
            }
        }

        public BrainDisplayNode this[int index]
        {
            get => nodeCol[index];
            set => nodeCol[index] = value;
        }

        public IEnumerator<BrainDisplayNode> GetEnumerator()
        {
            return nodeCol.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    [SerializeField]
    [HideInInspector]
    private List<BrainDisplayNode> inputDisplayNodes;
    [SerializeField]
    [HideInInspector]
    private MiddleDisplayNodeMatrix middleDisplayNodes;
    [SerializeField]
    [HideInInspector]
    private List<BrainDisplayNode> outputDisplayNodes;

    public void InitNodes()
    {
        if (!brainComponent || !displayNodePrefab)
        {
            return;
        }

        // init input nodes
        inputDisplayNodes = new List<BrainDisplayNode>(brainComponent.brain.inputNodes.Count);
        for (int i = 0; i < brainComponent.brain.inputNodes.Count; i++)
        {
            inputDisplayNodes.Add(Instantiate(displayNodePrefab).GetComponent<BrainDisplayNode>());
            inputDisplayNodes[i].name = "Input Node " + i;
            inputDisplayNodes[i].transform.parent = transform;
            inputDisplayNodes[i].CreateLineChildren(brainComponent.brain.middleNodes[0].Count);
        }

        // init middle nodes
        middleDisplayNodes = new MiddleDisplayNodeMatrix(brainComponent.brain.middleNodes.Count, brainComponent.brain.middleNodes[0].Count);
        for (int col = 0; col < brainComponent.brain.middleNodes.Count; col++)
        {
            for (int row = 0; row < brainComponent.brain.middleNodes[0].Count; row++)
            {
                middleDisplayNodes[col][row] = Instantiate(displayNodePrefab).GetComponent<BrainDisplayNode>();
                middleDisplayNodes[col][row].name = "Hidden Layer Node " + col + " " + row;
                middleDisplayNodes[col][row].transform.parent = transform;
                middleDisplayNodes[col][row].meshRenderer.material = new Material(Shader.Find("Standard"));
                if (col == brainComponent.brain.middleNodes.Count - 1)
                {
                    middleDisplayNodes[col][row].CreateLineChildren(brainComponent.brain.outputValues.Count);
                }
                else
                {
                    middleDisplayNodes[col][row].CreateLineChildren(brainComponent.brain.middleNodes[0].Count);
                }
            }
        }

        // init output nodes
        outputDisplayNodes = new List<BrainDisplayNode>(brainComponent.brain.outputValues.Count);
        for (int i = 0; i < brainComponent.brain.outputValues.Count; i++)
        {
            outputDisplayNodes.Add(Instantiate(displayNodePrefab).GetComponent<BrainDisplayNode>());
            outputDisplayNodes[i].name = "Output Node " + i;
            outputDisplayNodes[i].transform.parent = transform;
        }

        UpdateNodes();
    }

    public void UpdateNodes()
    {
        if (!brainComponent || !displayNodePrefab)
        {
            return;
        }

        float halfHiddenLayerWidth = middleNodeHorizontalSpacing * (brainComponent.brain.middleNodes.Count - 1) / 2f;

        // update output nodes
        float halfOutputLayerHeight = outputNodeSpacing * (brainComponent.brain.outputValues.Count - 1) / 2f;
        float outputLayerX = halfHiddenLayerWidth + middleToOutputSpacing;
        for (int i = 0; i < brainComponent.brain.outputValues.Count; i++)
        {
            BrainDisplayNode currentNode = outputDisplayNodes[i];
            if (!currentNode)
            {
                continue;
            }

            // update node position
            currentNode.transform.localPosition = new Vector3(
                outputLayerX,
                0f,
                halfOutputLayerHeight - (outputNodeSpacing * i)
            );

            // update node scale
            currentNode.transform.localScale = Vector3.one * outputNodeSize;

            // update node color
            Material newMaterial = new Material(currentNode.meshRenderer.sharedMaterial);
            newMaterial.color = new Color(
                (1f - brainComponent.brain.outputValues[i]) / 2f,
                (brainComponent.brain.outputValues[i] + 1f) / 2f,
                0f
            );
            currentNode.meshRenderer.sharedMaterial = newMaterial;
        }

        // update middle nodes
        float halfHiddenLayerHeight = middleNodeVerticalSpacing * (brainComponent.brain.middleNodes.Count - 1) / 2f;
        for (int col = brainComponent.brain.middleNodes.Count - 1; col >= 0; col--)
        {
            for (int row = 0; row < brainComponent.brain.middleNodes[0].Count; row++)
            {
                BrainDisplayNode currentNode = middleDisplayNodes[col][row];
                if (!currentNode)
                {
                    continue;
                }

                // update node position
                currentNode.transform.localPosition = new Vector3(
                    (middleNodeHorizontalSpacing * col) - halfHiddenLayerWidth,
                    0f,
                    halfHiddenLayerHeight - (middleNodeVerticalSpacing * row)
                );

                // update node scale
                currentNode.transform.localScale = Vector3.one * middleNodeSize;

                // update node color
                Material newMaterial = new Material(currentNode.meshRenderer.sharedMaterial);
                newMaterial.color = new Color(
                    (1f - brainComponent.brain.middleNodes[col][row].currentValue) / 2f,
                    (brainComponent.brain.middleNodes[col][row].currentValue + 1f) / 2f,
                    0f
                );
                currentNode.meshRenderer.sharedMaterial = newMaterial;

                // update weight connections
                // middle nodes connected to output nodes
                if (col == brainComponent.brain.middleNodes.Count - 1)
                {
                    for (int weightIndex = 0; weightIndex < brainComponent.brain.outputValues.Count; weightIndex++)
                    {
                        LineRenderer currentLineRenderer = currentNode.connectedLines[weightIndex];

                        // update connection points
                        currentLineRenderer.SetPositions(new Vector3[]{
                            currentNode.transform.position,
                            outputDisplayNodes[weightIndex].transform.position
                        });

                        // update connection color
                        Material newLineMaterial = new Material(currentLineRenderer.sharedMaterial);
                        newLineMaterial.color = new Color(
                            (1f - brainComponent.brain.middleNodes[col][row].weights[weightIndex]) / 2f,
                            (brainComponent.brain.middleNodes[col][row].weights[weightIndex] + 1f) / 2f,
                            0f
                        );
                        currentLineRenderer.sharedMaterial = newLineMaterial;

                        // update connection width
                        currentLineRenderer.startWidth = middleToOutputConnectionWidth;
                        currentLineRenderer.endWidth = middleToOutputConnectionWidth;
                    }
                }
                // middle nodes connected to other middle nodes
                else
                {
                    for (int weightIndex = 0; weightIndex < brainComponent.brain.middleNodes[0].Count; weightIndex++)
                    {
                        LineRenderer currentLineRenderer = currentNode.connectedLines[weightIndex];

                        // update connection points
                        currentLineRenderer.SetPositions(new Vector3[]{
                            currentNode.transform.position,
                            middleDisplayNodes[col + 1][weightIndex].transform.position
                        });

                        // update connection color
                        Material newLineMaterial = new Material(currentLineRenderer.sharedMaterial);
                        newLineMaterial.color = new Color(
                            (1f - brainComponent.brain.middleNodes[col][row].weights[weightIndex]) / 2f,
                            (brainComponent.brain.middleNodes[col][row].weights[weightIndex] + 1f) / 2f,
                            0f
                        );
                        currentLineRenderer.sharedMaterial = newLineMaterial;

                        // update connection width
                        currentLineRenderer.startWidth = middleToMiddleConnectionWidth;
                        currentLineRenderer.endWidth = middleToMiddleConnectionWidth;
                    }
                }
            }
        }

        // update input nodes
        float halfInputLayerHeight = inputNodeSpacing * (brainComponent.brain.inputNodes.Count - 1) / 2f;
        float inputLayerX = -halfHiddenLayerWidth - inputToMiddleSpacing;
        for (int i = 0; i < brainComponent.brain.inputNodes.Count; i++)
        {
            BrainDisplayNode currentNode = inputDisplayNodes[i];
            if (!currentNode)
            {
                continue;
            }

            // update node position
            currentNode.transform.localPosition = new Vector3(
                inputLayerX,
                0f,
                halfInputLayerHeight - (inputNodeSpacing * i)
            );

            // update node scale
            currentNode.transform.localScale = Vector3.one * inputNodeSize;

            // update node color
            Material newMaterial = new Material(currentNode.meshRenderer.sharedMaterial);
            newMaterial.color = new Color(
                (1f - brainComponent.brain.inputNodes[i].currentValue) / 2f,
                (brainComponent.brain.inputNodes[i].currentValue + 1f) / 2f,
                0f
            );
            currentNode.meshRenderer.sharedMaterial = newMaterial;

            // update weight connections
            for (int weightIndex = 0; weightIndex < inputDisplayNodes[i].connectedLines.Count; weightIndex++)
            {
                LineRenderer currentLineRenderer = currentNode.connectedLines[weightIndex];

                // update connection points
                currentLineRenderer.SetPositions(new Vector3[]{
                    currentNode.transform.position,
                    middleDisplayNodes[0][weightIndex].transform.position
                });

                // update connection color
                Material newLineMaterial = new Material(currentLineRenderer.sharedMaterial);
                newLineMaterial.color = new Color(
                    (1f - brainComponent.brain.inputNodes[i].weights[weightIndex]) / 2f,
                    (brainComponent.brain.inputNodes[i].weights[weightIndex] + 1f) / 2f,
                    0f
                );
                currentLineRenderer.sharedMaterial = newLineMaterial;

                // update connection width
                currentLineRenderer.startWidth = inputToMiddleConnectionWidth;
                currentLineRenderer.endWidth = inputToMiddleConnectionWidth;
            }
        }
    }

    public void DestroyNodes()
    {
        // destroy input display nodes
        foreach (BrainDisplayNode displayNode in inputDisplayNodes)
        {
            if (displayNode)
            {
                // destroy connections
                foreach (LineRenderer lineRenderer in displayNode.connectedLines)
                {
                    if (lineRenderer)
                    {
                        DestroyImmediate(lineRenderer.gameObject);
                    }
                }

                DestroyImmediate(displayNode.gameObject);
            }
        }
        inputDisplayNodes = new List<BrainDisplayNode>();

        // destroy middle display nodes
        foreach (MiddleDisplayNodeCol displayNodeCol in middleDisplayNodes)
        {
            foreach (BrainDisplayNode displayNode in displayNodeCol)
            {
                if (displayNode)
                {
                    // destroy connections
                    foreach (LineRenderer lineRenderer in displayNode.connectedLines)
                    {
                        if (lineRenderer)
                        {
                            DestroyImmediate(lineRenderer.gameObject);
                        }
                    }
                    DestroyImmediate(displayNode.gameObject);
                }
            }

        }
        middleDisplayNodes = new MiddleDisplayNodeMatrix(0, 0);

        // destroy output display nodes
        foreach (BrainDisplayNode displayNode in outputDisplayNodes)
        {
            if (displayNode)
            {
                DestroyImmediate(displayNode.gameObject);
            }
        }
        outputDisplayNodes = new List<BrainDisplayNode>();
    }

    public void ResetNodes()
    {
        DestroyNodes();
        InitNodes();
    }

    void Update()
    {
        ResetNodes();
    }
}