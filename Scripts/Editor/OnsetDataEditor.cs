using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[CustomEditor(typeof(OnsetData))]
public class OnsetDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        OnsetData data = (OnsetData)target;
        EditorGUILayout.Space(10);

        if (data.audioClip == null)
        {
            EditorGUILayout.HelpBox("Add AudioClip before analysis!", MessageType.Warning);
            return;
        }

        if (GUILayout.Button("Analyze Track", GUILayout.Height(30)))
        {
            AnalyzeTrack(data);
        }

        if (data.onsetTimes.Count > 0)
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.HelpBox($"Found onsets: {data.onsetTimes.Count}", MessageType.Info);
        }
    }

    private void AnalyzeTrack(OnsetData data)
    {
        Debug.Log($"[OnsetDataEditor] Preparing analysis for: {data.audioClip.name}");

        data.onsetTimes.Clear();


        string assetPath = AssetDatabase.GetAssetPath(data);
        EditorPrefs.SetString("OnsetDataPath", assetPath);


        Debug.Log("[OnsetDataEditor] OnsetData path saved to EditorPrefs");
        Debug.Log("[OnsetDataEditor] *** MANUALLY OPEN AudioAnalysisScene AND PRESS PLAY! ***");
    }
}