using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class ExportScriptUsage : EditorWindow
{
    [MenuItem("Tools/Export Script Usage")]
    public static void ShowWindow()
    {
        GetWindow<ExportScriptUsage>("Export Script Usage");
    }

    private MonoScript targetScript;
    private string outputFileName = "ScriptUsageReport.txt";

    private void OnGUI()
    {
        GUILayout.Label("Export Script Usage to .txt", EditorStyles.boldLabel);

        // 스크립트를 선택하는 필드
        targetScript = (MonoScript)EditorGUILayout.ObjectField("Script:", targetScript, typeof(MonoScript), false);

        // 파일 이름 입력
        outputFileName = EditorGUILayout.TextField("Output File Name:", outputFileName);

        if (GUILayout.Button("Export"))
        {
            if (targetScript == null)
            {
                Debug.LogWarning("Please select a script to analyze!");
                return;
            }

            ExportScriptUsageToFile(targetScript, outputFileName);
        }
    }

    private void ExportScriptUsageToFile(MonoScript script, string fileName)
    {
        string scriptName = script.name;
        string[] guids = AssetDatabase.FindAssets("t:GameObject"); // 모든 GameObject 찾기

        List<string> results = new List<string>();
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            if (obj == null) continue;

            // 스크립트가 적용된 컴포넌트를 찾기
            Component[] components = obj.GetComponentsInChildren<Component>(true);
            foreach (Component component in components)
            {
                if (component == null) continue;

                if (component.GetType().Name == scriptName)
                {
                    results.Add($"Name: {obj.name}, Path: {path}");
                    break;
                }
            }
        }

        // 결과를 txt 파일로 저장
        string outputPath = Path.Combine(Application.dataPath, fileName);
        File.WriteAllLines(outputPath, results);

        Debug.Log($"Script usage report saved to: {outputPath}");
        AssetDatabase.Refresh();
    }
}
