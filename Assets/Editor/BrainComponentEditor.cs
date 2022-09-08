using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BrainComponent))]
public class BrainComponentEditor : Editor
{
    public BrainComponent brainComponent;

    #region serialized properties
    public SerializedProperty brainName;
    public SerializedProperty inputCount;
    public SerializedProperty middleColCount;
    public SerializedProperty middleRowCount;
    public SerializedProperty outputCount;
    public SerializedProperty radioIndex;
    #endregion serialized properties

    void OnEnable()
    {
        brainComponent = target as BrainComponent;

        EditorUtility.SetDirty(brainComponent);

        brainComponent.InitBrain();

        InitSerialProps();
    }

    private void InitSerialProps()
    {
        brainName = serializedObject.FindProperty("brainName");
        inputCount = serializedObject.FindProperty("inputCount");
        middleColCount = serializedObject.FindProperty("middleColCount");
        middleRowCount = serializedObject.FindProperty("middleRowCount");
        outputCount = serializedObject.FindProperty("outputCount");
        radioIndex = serializedObject.FindProperty("radioIndex");
    }

    override public void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUI.BeginChangeCheck();

        EditorGUILayout.PropertyField(brainName);

        EditorGUILayout.Separator();

        EditorGUILayout.LabelField("Creation Method");

        radioIndex.intValue = RadioGroup(radioIndex.intValue, new string[] { "Random", "Load From File" });

        EditorGUILayout.Separator();

        switch (radioIndex.intValue)
        {
            case 0: // random
                NodeCountGUI();
                break;

            case 1: // load from file
                LoadFromFileGUI();
                break;
        }

        serializedObject.ApplyModifiedProperties();

        if (EditorGUI.EndChangeCheck())
        {
            brainComponent.InitBrain();
        }
    }

    private int RadioGroup(int selectedIndex, string[] labels)
    {
        int index = selectedIndex;
        for (int i = 0; i < labels.Length; i++)
        {
            EditorGUILayout.BeginHorizontal();
            if (EditorGUILayout.Toggle(index == i, EditorStyles.radioButton, GUILayout.Width(20)))
            {
                index = i;
            }
            EditorGUILayout.LabelField(labels[i]);
            EditorGUILayout.EndHorizontal();
        }
        return index;
    }

    private void NodeCountGUI()
    {
        inputCount.intValue = Mathf.Max(EditorGUILayout.IntField("Inputs", inputCount.intValue), 0);

        middleColCount.intValue = Mathf.Max(EditorGUILayout.IntField("Hidden Layer Columns", middleColCount.intValue), 1);

        middleRowCount.intValue = Mathf.Max(EditorGUILayout.IntField("Hidden Layer Rows", middleRowCount.intValue), 1);

        outputCount.intValue = Mathf.Max(EditorGUILayout.IntField("Outputs", outputCount.intValue), 1);
    }

    private void LoadFromFileGUI()
    {
        string path = Brain.brainsPath + "/" + brainName.stringValue + ".brain";
        if (!File.Exists(path))
        {
            EditorGUILayout.HelpBox(path + " file not found. Please generate a new brain.", MessageType.Warning);
            NodeCountGUI();
            if (GUILayout.Button("Generate", GUILayout.Width(200)))
            {
                new Brain(inputCount.intValue, middleColCount.intValue, middleRowCount.intValue, outputCount.intValue).SaveAs(brainName.stringValue);
            }
        }
    }
}