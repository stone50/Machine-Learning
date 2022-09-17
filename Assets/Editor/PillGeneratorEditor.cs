using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PillGenerator))]
public class PillGeneratorEditor : Editor
{
    PillGenerator pillGenerator;

    SerializedProperty bounds;

    void OnEnable()
    {
        pillGenerator = target as PillGenerator;

        InitSerialProps();
    }

    private void InitSerialProps()
    {
        bounds = serializedObject.FindProperty("bounds");
    }

    public void OnSceneGUI()
    {
        Handles.Label(pillGenerator.transform.position, pillGenerator.gameObject.name);

        Handles.color = Color.magenta;
        Handles.DrawWireCube(bounds.boundsValue.center, bounds.boundsValue.size);
    }
}