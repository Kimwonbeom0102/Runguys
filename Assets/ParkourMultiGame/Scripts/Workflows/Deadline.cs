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
        // ������ο� ���� ��ü�� �÷��̾����� Ȯ��
        if (other.CompareTag("Player"))
        {
            GameObject player = other.gameObject;

            // GameManager�� ���� �÷��̾� Ż�� ó��
            gamePlayManager.ForceEliminatePlayer(player);

            Debug.Log($"Player {player.GetPhotonView().Owner.NickName} has hit the deadline!");
        }
    }
}