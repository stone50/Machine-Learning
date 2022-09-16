using System.Collections.Generic;
using UnityEngine;

public class BrainDisplayNode : MonoBehaviour
{
    public MeshRenderer meshRenderer;

    [HideInInspector]
    public List<LineRenderer> connectedLines;

    private LineRenderer CreateLineChild(string lineName)
    {
        GameObject nodeObject = new GameObject();

        LineRenderer renderer = nodeObject.AddComponent<LineRenderer>();
        renderer.sharedMaterial = new Material(BrainDisplay.nodeShader);
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        renderer.receiveShadows = false;

        nodeObject.name = lineName;
        nodeObject.transform.parent = transform;
        nodeObject.transform.localPosition = Vector3.zero;

        return renderer;
    }

    public void CreateLineChildren(int childCount)
    {
        connectedLines = new List<LineRenderer>(childCount);
        for (int i = 0; i < childCount; i++)
        {
            connectedLines.Add(CreateLineChild($"Connection {i}"));
        }
    }
}