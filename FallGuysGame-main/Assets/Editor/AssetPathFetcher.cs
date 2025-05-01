using System.IO; // ���� �۾��� ���� �ʿ�
using UnityEditor; // AssetDatabase API�� ����ϱ� ���� �ʿ�
using UnityEngine;

public class SaveAssetPathsToTxt : MonoBehaviour
{
    // Unity �޴����� ������ �� �ִ� �ɼ� �߰�
    [MenuItem("Tools/Save All Asset Paths to Txt")]
    private static void SaveAllAssetPathsToTxt()
    {
        // 1. ��� ������ �˻�
        string[] allGuids = AssetDatabase.FindAssets(""); // ��� ���� �˻�
        Debug.Log($"�� ���� ����: {allGuids.Length}");

        // 2. ���� ��¥�� �ð��� ������� ���� �̸� ����
        string currentDate = System.DateTime.Now.ToString("yyyyMMdd_HHmmss"); // ��¥ ����: 20250120_153045
        string fileName = $"AllAssetPaths_{currentDate}.txt"; // ���� �̸�: AllAssetPaths_20250120_153045.txt
        string outputPath = Path.Combine(Application.dataPath, fileName); // ���� ���� ���

        // 3. ���Ͽ� ��� ���� ����
        using (StreamWriter writer = new StreamWriter(outputPath))
        {
            writer.WriteLine("=== Unity ������Ʈ �� ��� ���� ��� ===");
            writer.WriteLine($"�� ���� ����: {allGuids.Length}");
            writer.WriteLine();

            foreach (string guid in allGuids)
            {
                // GUID�� ��η� ��ȯ
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                writer.WriteLine(assetPath); // ���Ͽ� ��� ���
            }
        }

        // 4. �Ϸ� �޽��� ���
        Debug.Log($"���� ��ΰ� ���Ͽ� ����Ǿ����ϴ�: {outputPath}");
    }
}
