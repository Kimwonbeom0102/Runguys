using Photon.Pun;
using UnityEngine;

public class Deadline : MonoBehaviour
{
    GamePlayManager gamePlayManager;

    private void Awake()
    {
        gamePlayManager = GetComponent<GamePlayManager>();
    }
    private void OnTriggerEnter(Collider other)
    {
        // 데드라인에 닿은 객체가 플레이어인지 확인
        if (other.CompareTag("Player"))
        {
            GameObject player = other.gameObject;

            // GameManager를 통해 플레이어 탈락 처리
            gamePlayManager.ForceEliminatePlayer(player);

            Debug.Log($"Player {player.GetPhotonView().Owner.NickName} has hit the deadline!");
        }
    }
}