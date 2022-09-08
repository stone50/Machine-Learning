using UnityEngine;

[ExecuteInEditMode]
public class BrainDisplay : MonoBehaviour
{
    public GameObject displayNodePrefab;

    public BrainComponent brainComponent;
    private Brain brain;

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

    private BrainDisplayNode[] inputDisplayNodes = new BrainDisplayNode[0];
    private BrainDisplayNode[,] middleDisplayNodes = new BrainDisplayNode[0, 0];
    private BrainDisplayNode[] outputDisplayNodes = new BrainDisplayNode[0];

    public void InitNodes()
    {
        if (!brainComponent || !displayNodePrefab)
        {
            return;
        }

        // init input nodes
        inputDisplayNodes = new BrainDisplayNode[brainComponent.brain.inputNodes.Length];
        for (int i = 0; i < brainComponent.brain.inputNodes.Length; i++)
        {
            inputDisplayNodes[i] = Instantiate(displayNodePrefab).GetComponent<BrainDisplayNode>();
            inputDisplayNodes[i].name = "Input Node " + i;
            inputDisplayNodes[i].transform.parent = transform;
            inputDisplayNodes[i].CreateLineChildren(brainComponent.brain.middleNodes.GetLength(1));
        }

        // init middle nodes
        middleDisplayNodes = new BrainDisplayNode[brainComponent.brain.middleNodes.GetLength(0), brainComponent.brain.middleNodes.GetLength(1)];
        for (int col = 0; col < brainComponent.brain.middleNodes.GetLength(0); col++)
        {
            for (int row = 0; row < brainComponent.brain.middleNodes.GetLength(1); row++)
            {
                middleDisplayNodes[col, row] = Instantiate(displayNodePrefab).GetComponent<BrainDisplayNode>();
                middleDisplayNodes[col, row].name = "Hidden Layer Node " + col + " " + row;
                middleDisplayNodes[col, row].transform.parent = transform;
                middleDisplayNodes[col, row].meshRenderer.material = new Material(Shader.Find("Standard"));
                if (col == brainComponent.brain.middleNodes.GetLength(0) - 1)
                {
                    middleDisplayNodes[col, row].CreateLineChildren(brainComponent.brain.outputValues.Length);
                }
                else
                {
                    middleDisplayNodes[col, row].CreateLineChildren(brainComponent.brain.middleNodes.GetLength(1));
                }
            }
        }

        // init output nodes
        outputDisplayNodes = new BrainDisplayNode[brainComponent.brain.outputValues.Length];
        for (int i = 0; i < brainComponent.brain.outputValues.Length; i++)
        {
            outputDisplayNodes[i] = Instantiate(displayNodePrefab).GetComponent<BrainDisplayNode>();
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

        float halfHiddenLayerWidth = middleNodeHorizontalSpacing * (middleDisplayNodes.GetLength(0) - 1) / 2f;

        // update output nodes
        float halfOutputLayerHeight = outputNodeSpacing * (outputDisplayNodes.Length - 1) / 2f;
        float outputLayerX = halfHiddenLayerWidth + middleToOutputSpacing;
        for (int i = 0; i < outputDisplayNodes.Length; i++)
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
        float halfHiddenLayerHeight = middleNodeVerticalSpacing * (middleDisplayNodes.GetLength(1) - 1) / 2f;
        for (int col = middleDisplayNodes.GetLength(0) - 1; col >= 0; col--)
        {
            for (int row = 0; row < middleDisplayNodes.GetLength(1); row++)
            {
                BrainDisplayNode currentNode = middleDisplayNodes[col, row];
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
                    (1f - brainComponent.brain.middleNodes[col, row].currentValue) / 2f,
                    (brainComponent.brain.middleNodes[col, row].currentValue + 1f) / 2f,
                    0f
                );
                currentNode.meshRenderer.sharedMaterial = newMaterial;

                // update weight connections
                // middle nodes connected to output nodes
                if (col == middleDisplayNodes.GetLength(0) - 1)
                {
                    for (int weightIndex = 0; weightIndex < outputDisplayNodes.Length; weightIndex++)
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
                            (1f - brainComponent.brain.middleNodes[col, row].weights[weightIndex]) / 2f,
                            (brainComponent.brain.middleNodes[col, row].weights[weightIndex] + 1f) / 2f,
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
                    for (int weightIndex = 0; weightIndex < middleDisplayNodes.GetLength(1); weightIndex++)
                    {
                        LineRenderer currentLineRenderer = currentNode.connectedLines[weightIndex];

                        // update connection points
                        currentLineRenderer.SetPositions(new Vector3[]{
                            currentNode.transform.position,
                            middleDisplayNodes[col + 1, weightIndex].transform.position
                        });

                        // update connection color
                        Material newLineMaterial = new Material(currentLineRenderer.sharedMaterial);
                        newLineMaterial.color = new Color(
                            (1f - brainComponent.brain.middleNodes[col, row].weights[weightIndex]) / 2f,
                            (brainComponent.brain.middleNodes[col, row].weights[weightIndex] + 1f) / 2f,
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
        float halfInputLayerHeight = inputNodeSpacing * (inputDisplayNodes.Length - 1) / 2f;
        float inputLayerX = -halfHiddenLayerWidth - inputToMiddleSpacing;
        for (int i = 0; i < inputDisplayNodes.Length; i++)
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
            for (int weightIndex = 0; weightIndex < inputDisplayNodes[i].connectedLines.Length; weightIndex++)
            {
                LineRenderer currentLineRenderer = currentNode.connectedLines[weightIndex];

                // update connection points
                currentLineRenderer.SetPositions(new Vector3[]{
                    currentNode.transform.position,
                    middleDisplayNodes[0, weightIndex].transform.position
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
        inputDisplayNodes = new BrainDisplayNode[0];

        // destroy middle display nodes
        foreach (BrainDisplayNode displayNode in middleDisplayNodes)
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
        middleDisplayNodes = new BrainDisplayNode[0, 0];

        // destroy output display nodes
        foreach (BrainDisplayNode displayNode in outputDisplayNodes)
        {
            if (displayNode)
            {
                DestroyImmediate(displayNode.gameObject);
            }
        }
        outputDisplayNodes = new BrainDisplayNode[0];
    }

    public void ResetNodes()
    {
        DestroyNodes();
        InitNodes();
    }

    void OnDisable()
    {
        Debug.Log("disable");
        DestroyNodes();
    }

    void OnValidate()
    {
        UpdateNodes();
    }

    void Update()
    {
        if (brainComponent && displayNodePrefab && brainComponent.brain != brain)
        {
            ResetNodes();
            brain = brainComponent.brain;
        }
    }
}