using System.IO; // 파일 작업을 위해 필요
using UnityEditor; // AssetDatabase API를 사용하기 위해 필요
using UnityEngine;

public class SaveAssetPathsToTxt : MonoBehaviour
{
    // Unity 메뉴에서 실행할 수 있는 옵션 추가
    [MenuItem("Tools/Save All Asset Paths to Txt")]
    private static void SaveAllAssetPathsToTxt()
    {
        // 1. 모든 에셋을 검색
        string[] allGuids = AssetDatabase.FindAssets(""); // 모든 에셋 검색
        Debug.Log($"총 에셋 개수: {allGuids.Length}");

        // 2. 현재 날짜와 시간을 기반으로 파일 이름 생성
        string currentDate = System.DateTime.Now.ToString("yyyyMMdd_HHmmss"); // 날짜 형식: 20250120_153045
        string fileName = $"AllAssetPaths_{currentDate}.txt"; // 파일 이름: AllAssetPaths_20250120_153045.txt
        string outputPath = Path.Combine(Application.dataPath, fileName); // 최종 파일 경로

        // 3. 파일에 경로 정보 저장
        using (StreamWriter writer = new StreamWriter(outputPath))
        {
            writer.WriteLine("=== Unity 프로젝트 내 모든 에셋 경로 ===");
            writer.WriteLine($"총 에셋 개수: {allGuids.Length}");
            writer.WriteLine();

            foreach (string guid in allGuids)
            {
                // GUID를 경로로 변환
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                writer.WriteLine(assetPath); // 파일에 경로 기록
            }
        }

        // 4. 완료 메시지 출력
        Debug.Log($"에셋 경로가 파일에 저장되었습니다: {outputPath}");
    }
}
