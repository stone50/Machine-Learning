using UnityEngine;
using UnityEditor;

public class PillGenerator : MonoBehaviour
{
    public GameObject pillPrefab;

    public Bounds bounds;
    [Range(0, 800)]
    public int pillCount;

    void Start()
    {
        for (int i = 0; i < pillCount; i++)
        {
            GameObject pill = PrefabUtility.InstantiatePrefab(pillPrefab) as GameObject;
            pill.transform.parent = transform;
            pill.transform.position = new Vector3(
                bounds.center.x + Utils.RandomRange(-bounds.extents.x, bounds.extents.x),
                bounds.center.y,
                bounds.center.z + Utils.RandomRange(-bounds.extents.z, bounds.extents.z)
            );
            pill.name = $"Pill {i + 1}";
        }
    }
}
