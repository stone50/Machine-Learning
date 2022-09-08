using UnityEngine;

public class BrainDisplayNode : MonoBehaviour
{
    public GameObject connectionPrefab;
    public MeshRenderer meshRenderer;

    [HideInInspector]
    public LineRenderer[] connectedLines = new LineRenderer[0];

    public void CreateLineChildren(int childCount)
    {
        connectedLines = new LineRenderer[childCount];
        for (int i = 0; i < childCount; i++)
        {
            GameObject child = Instantiate(connectionPrefab);
            child.name = "Connection " + i;
            child.transform.parent = transform;
            child.transform.localPosition = Vector3.zero;
            connectedLines[i] = child.GetComponent<LineRenderer>();
        }
    }
}