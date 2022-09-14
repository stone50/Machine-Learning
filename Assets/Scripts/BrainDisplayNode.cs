using System.Collections.Generic;
using UnityEngine;

public class BrainDisplayNode : MonoBehaviour
{
    public GameObject connectionPrefab;
    public MeshRenderer meshRenderer;

    [HideInInspector]
    public List<LineRenderer> connectedLines;

    public void CreateLineChildren(int childCount)
    {
        connectedLines = new List<LineRenderer>(childCount);
        for (int i = 0; i < childCount; i++)
        {
            GameObject child = Instantiate(connectionPrefab);
            child.name = "Connection " + i;
            child.transform.parent = transform;
            child.transform.localPosition = Vector3.zero;
            connectedLines.Add(child.GetComponent<LineRenderer>());
        }
    }
}