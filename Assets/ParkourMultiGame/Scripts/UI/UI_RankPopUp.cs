using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Practices.UGUI_Management.UI;  // UI_Popup 상속받기 위해
using UnityEngine.UI;
using System.Text;

public class UI_RankPopUp : UI_Popup
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI rankListText;
    // 예) Canvas - RankPopUp 하위에 "Text (TMP) - RankListText" 라는 GameObject가 있다고 가정

    // [ADDED] 최종 결과 목록을 세팅 받음
    public void SetResults(List<GamePlayManager.PlayerResult> sortedResults)
    {
        // sortedResults 안에는 GamePlayManager 쪽에서
        // PlayerResult( nickName, finished, finishTime, rank...) 구조체들이 들어옴

        if (rankListText == null)
        {
            Debug.LogError("[UI_RankPopUp] rankListText is not assigned!");
            return;
        }

        StringBuilder sb = new StringBuilder();
        int count = 0;
        foreach (var resultObj in sortedResults)
        {
            // 캐스팅
            // (GamePlayManager.PlayerResult) 또는 dynamic 접근
            // 예시로 PlayerResult 구조체가 public이 아니라 private라면 접근이 안되므로
            // 구조체를 public으로 바꾸거나, string으로 넘기는 방법 등 조정 필요

            // 아래는 "public struct PlayerResult" 라면 접근 가능하다는 전제.
            dynamic pr = resultObj;
            count++;

            // 완주 못했거나 중도이탈인 경우
            if (!pr.finished && !pr.leftEarly)
            {
                sb.AppendLine($"{count}등   {pr.nickName} : DNF");
            }
            else if (pr.leftEarly)
            {
                sb.AppendLine($"{count}등   {pr.nickName} : 탈주");
            }
            else
            {
                // rank가 0이면 DNF로 간주
                if (pr.rank == 0)
                {
                    sb.AppendLine($"{count}등   {pr.nickName} : DNF");
                }
                else
                {
                    sb.AppendLine($"{pr.rank}등 {pr.nickName} : {pr.finishTime:F2}초");
                }
            }
        }

        rankListText.text = sb.ToString();
    }
}
