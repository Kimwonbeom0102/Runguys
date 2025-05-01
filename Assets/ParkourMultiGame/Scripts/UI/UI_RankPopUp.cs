using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Practices.UGUI_Management.UI;  // UI_Popup ��ӹޱ� ����
using UnityEngine.UI;
using System.Text;

public class UI_RankPopUp : UI_Popup
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI rankListText;
    // ��) Canvas - RankPopUp ������ "Text (TMP) - RankListText" ��� GameObject�� �ִٰ� ����

    // [ADDED] ���� ��� ����� ���� ����
    public void SetResults(List<GamePlayManager.PlayerResult> sortedResults)
    {
        // sortedResults �ȿ��� GamePlayManager �ʿ���
        // PlayerResult( nickName, finished, finishTime, rank...) ����ü���� ����

        if (rankListText == null)
        {
            Debug.LogError("[UI_RankPopUp] rankListText is not assigned!");
            return;
        }

        StringBuilder sb = new StringBuilder();
        int count = 0;
        foreach (var resultObj in sortedResults)
        {
            // ĳ����
            // (GamePlayManager.PlayerResult) �Ǵ� dynamic ����
            // ���÷� PlayerResult ����ü�� public�� �ƴ϶� private��� ������ �ȵǹǷ�
            // ����ü�� public���� �ٲٰų�, string���� �ѱ�� ��� �� ���� �ʿ�

            // �Ʒ��� "public struct PlayerResult" ��� ���� �����ϴٴ� ����.
            dynamic pr = resultObj;
            count++;

            // ���� ���߰ų� �ߵ���Ż�� ���
            if (!pr.finished && !pr.leftEarly)
            {
                sb.AppendLine($"{count}��   {pr.nickName} : DNF");
            }
            else if (pr.leftEarly)
            {
                sb.AppendLine($"{count}��   {pr.nickName} : Ż��");
            }
            else
            {
                // rank�� 0�̸� DNF�� ����
                if (pr.rank == 0)
                {
                    sb.AppendLine($"{count}��   {pr.nickName} : DNF");
                }
                else
                {
                    sb.AppendLine($"{pr.rank}�� {pr.nickName} : {pr.finishTime:F2}��");
                }
            }
        }

        rankListText.text = sb.ToString();
    }
}
