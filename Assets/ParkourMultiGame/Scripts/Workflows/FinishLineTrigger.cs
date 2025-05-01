// FinishLineTrigger.cs (Updated)
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FinishLineTrigger : MonoBehaviourPun
{
    // 마스터에서 도착 순서 기록
    private List<(int actorNumber, double finishTime)> _finishList = new List<(int, double)>();
    private bool _finishAnnounced = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        if (other.CompareTag("Player"))
        {
            PhotonView pv = other.GetComponent<PhotonView>();
            if (pv != null)
            {
                int actorNumber = pv.OwnerActorNr;
                bool alreadyFinished = _finishList.Any(x => x.actorNumber == actorNumber);

                if (!alreadyFinished)
                {
                    double t = PhotonNetwork.Time;
                    _finishList.Add((actorNumber, t));
                    photonView.RPC(nameof(RpcOnPlayerFinished), RpcTarget.All, actorNumber, t);

                    // 전원 도착 체크
                    if (_finishList.Count >= PhotonNetwork.CurrentRoom.PlayerCount)
                    {
                        // 전원 도착
                        AnnounceFinalResult();
                    }
                }
            }
        }
    }

    [PunRPC]
    private void RpcOnPlayerFinished(int actorNumber, double finishTime)
    {
        Debug.Log($"Player {actorNumber} finished at time={finishTime}");
        // 필요시 UI 표기
    }

    // 마스터 전용 결과 발표
    void AnnounceFinalResult()
    {
        if (_finishAnnounced) return;
        _finishAnnounced = true;

        var ranking = _finishList.OrderBy(x => x.finishTime).ToList();
        for (int i = 0; i < ranking.Count; i++)
        {
            Debug.Log($"Rank {i + 1}: Actor={ranking[i].actorNumber}, time={ranking[i].finishTime}");
        }

        StartCoroutine(CoFinishInSeconds(5f)); // 5초 후 종료/로비
    }

    IEnumerator CoFinishInSeconds(float sec)
    {
        yield return new WaitForSeconds(sec);
        PhotonNetwork.LeaveRoom();
        UnityEngine.SceneManagement.SceneManager.LoadScene("Lobby");
    }
}
