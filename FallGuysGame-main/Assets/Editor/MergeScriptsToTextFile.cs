using System.IO; // 파일 작업 관련
using UnityEditor; // Unity 에디터 관련
using UnityEngine; // Unity 엔진 관련

public class MergeSpecificScripts
{
    [MenuItem("Tools/Merge Scripts to Text File")]
    public static void MergeScripts()
    {
        // 사용자 지정 경로 (cs 파일이 있는 루트 경로)
        string targetPath = @"C:\Data\GitHub\FallGuysGame\Assets\ParkourMultiGame\";

        // 1. 파일 이름에 날짜 포함
        string currentDate = System.DateTime.Now.ToString("yyyyMMdd_HHmmss"); // 날짜 형식: 20250120_153045
        string mergedFileName = $"MergedScripts_{currentDate}.txt"; // 예: MergedScripts_20250120_153045.txt
        string outputPath = Path.Combine(targetPath, mergedFileName); // 최종 파일 경로

        try
        {
            using (StreamWriter writer = new StreamWriter(outputPath)) // StreamWriter는 기본적으로 파일을 덮어씀
            {
                // 지정된 경로에서 모든 .cs 파일 검색
                string[] scriptFiles = Directory.GetFiles(targetPath, "*.cs", SearchOption.AllDirectories);
                int totalFiles = scriptFiles.Length; // 파일 개수

                // 2. 맨 위에 작성 날짜, 파일 개수, 파일 목록 작성
                writer.WriteLine("=== Script Merge Summary ===");
                writer.WriteLine($"Date: {System.DateTime.Now:yyyy-MM-dd HH:mm:ss}"); // 현재 날짜 및 시간
                writer.WriteLine($"Total Files: {totalFiles}");
                writer.WriteLine("File Names:");
                foreach (string filePath in scriptFiles)
                {
                    writer.WriteLine($"- {Path.GetFileName(filePath)}");
                }
                writer.WriteLine("============================");
                writer.WriteLine(); // 빈 줄 추가

                // 3. 각 파일의 내용을 병합
                foreach (string filePath in scriptFiles)
                {
                    writer.WriteLine($"// --- {Path.GetFileName(filePath)} ---"); // 파일 이름 출력
                    writer.WriteLine(File.ReadAllText(filePath)); // 파일 내용 추가
                    writer.WriteLine(); // 빈 줄 추가
                }
            }

            // 작업 완료 메시지 출력
            Debug.Log($"Scripts have been merged into: {outputPath}");
            EditorUtility.DisplayDialog("Merge Complete", $"All scripts have been merged into:\n{outputPath}", "OK");
        }
        catch (System.Exception ex)
        {
            // 오류 메시지 출력
            Debug.LogError($"Error while merging scripts: {ex.Message}");
            EditorUtility.DisplayDialog("Error", "An error occurred while merging scripts.\nCheck the console for details.", "OK");
        }
    }
}
