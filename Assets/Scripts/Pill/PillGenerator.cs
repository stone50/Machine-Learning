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
        if (!pillPrefab)
        {
            return;
        }
        for (int i = 0; i < pillCount; i++)
        {
            GameObject pill = PrefabUtility.InstantiatePrefab(pillPrefab) as GameObject;
            pill.transform.parent = transform;
            pill.transform.position = new Vector3(
                bounds.center.x + UnityEngine.Random.Range(-bounds.extents.x, bounds.extents.x),
                bounds.center.y,
                bounds.center.z + UnityEngine.Random.Range(-bounds.extents.z, bounds.extents.z)
            );
            pill.name = $"Pill {i + 1}";
        }
    }
}
