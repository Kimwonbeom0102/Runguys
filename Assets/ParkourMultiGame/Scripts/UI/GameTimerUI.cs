// [ADDED FILE] GameTimerUI.cs
// Canvas + TMP_Text 오브젝트에 연결 후, _timerText에 참조
using Photon.Pun;
using UnityEngine;
using TMPro;

public class GameTimerUI : MonoBehaviourPun
{
    [SerializeField] TMP_Text _timerText;
    // 레이스 총 시간을 초 단위로 지정 (예: 5분 = 300초)
    public float raceDuration = 300f;
    double _startTime;

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            _startTime = PhotonNetwork.Time;
            photonView.RPC(nameof(SyncStartTime), RpcTarget.Others, _startTime);
        }
    }

    [PunRPC]
    void SyncStartTime(double t)
    {
        _startTime = t;
    }

    void Update()
    {
        // 경과 시간 계산
        double elapsed = PhotonNetwork.Time - _startTime;
        // 남은 시간 계산 (음수 방지)
        double remaining = raceDuration - elapsed;
        if (remaining < 0) remaining = 0;

        // 남은 시간을 MM:SS:CS 형식(분:초:센티초)으로 표시 (예: 04:59:99)
        int minutes = (int)(remaining / 60);
        int seconds = (int)(remaining % 60);
        //int centiseconds = (int)((remaining - minutes * 60 - seconds) * 100);

        _timerText.text = string.Format("{0:D2}:{1:D2}", minutes, seconds);
        //_timerText.text = string.Format("{0:D2}:{1:D2}:{2:D2}", minutes, seconds, centiseconds);
    }
}