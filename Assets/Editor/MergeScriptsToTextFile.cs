using System.IO; // ���� �۾� ����
using UnityEditor; // Unity ������ ����
using UnityEngine; // Unity ���� ����

public class MergeSpecificScripts
{
    [MenuItem("Tools/Merge Scripts to Text File")]
    public static void MergeScripts()
    {
        // ����� ���� ��� (cs ������ �ִ� ��Ʈ ���)
        string targetPath = @"C:\Data\GitHub\FallGuysGame\Assets\ParkourMultiGame\";

        // 1. ���� �̸��� ��¥ ����
        string currentDate = System.DateTime.Now.ToString("yyyyMMdd_HHmmss"); // ��¥ ����: 20250120_153045
        string mergedFileName = $"MergedScripts_{currentDate}.txt"; // ��: MergedScripts_20250120_153045.txt
        string outputPath = Path.Combine(targetPath, mergedFileName); // ���� ���� ���

        try
        {
            using (StreamWriter writer = new StreamWriter(outputPath)) // StreamWriter�� �⺻������ ������ ���
            {
                // ������ ��ο��� ��� .cs ���� �˻�
                string[] scriptFiles = Directory.GetFiles(targetPath, "*.cs", SearchOption.AllDirectories);
                int totalFiles = scriptFiles.Length; // ���� ����

                // 2. �� ���� �ۼ� ��¥, ���� ����, ���� ��� �ۼ�
                writer.WriteLine("=== Script Merge Summary ===");
                writer.WriteLine($"Date: {System.DateTime.Now:yyyy-MM-dd HH:mm:ss}"); // ���� ��¥ �� �ð�
                writer.WriteLine($"Total Files: {totalFiles}");
                writer.WriteLine("File Names:");
                foreach (string filePath in scriptFiles)
                {
                    writer.WriteLine($"- {Path.GetFileName(filePath)}");
                }
                writer.WriteLine("============================");
                writer.WriteLine(); // �� �� �߰�

                // 3. �� ������ ������ ����
                foreach (string filePath in scriptFiles)
                {
                    writer.WriteLine($"// --- {Path.GetFileName(filePath)} ---"); // ���� �̸� ���
                    writer.WriteLine(File.ReadAllText(filePath)); // ���� ���� �߰�
                    writer.WriteLine(); // �� �� �߰�
                }
            }

            // �۾� �Ϸ� �޽��� ���
            Debug.Log($"Scripts have been merged into: {outputPath}");
            EditorUtility.DisplayDialog("Merge Complete", $"All scripts have been merged into:\n{outputPath}", "OK");
        }
        catch (System.Exception ex)
        {
            // ���� �޽��� ���
            Debug.LogError($"Error while merging scripts: {ex.Message}");
            EditorUtility.DisplayDialog("Error", "An error occurred while merging scripts.\nCheck the console for details.", "OK");
        }
    }
}
