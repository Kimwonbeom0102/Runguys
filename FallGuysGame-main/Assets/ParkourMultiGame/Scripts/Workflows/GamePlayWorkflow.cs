// [CHANGED FILE] GamePlayWorkflow.cs
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;

namespace Practices.PhotonPunClient
{
    public class GamePlayWorkflow : MonoBehaviour
    {
        WaitForSeconds _waitFor1Seconds = new WaitForSeconds(1);

        [SerializeField] private Transform planeTransform;

        private void Start()
        {
            StartCoroutine(C_Workflow());
        }

        IEnumerator C_Workflow()
        {
            SpawnPlayerCharacter();
            yield return StartCoroutine(C_WaitUntilAllPlayerCharactersAreSpawned());
            // TODO: Player input enable or Countdown
        }

        Vector3 GetRandomPositionOnPlane(Transform planeTransform)
        {
            // Plane�� ũ�⸦ ��� (�⺻ ũ�� 10x10 �ݿ�)
            Vector3 planeSize = new Vector3(
                planeTransform.localScale.x,
                planeTransform.localScale.y,
                planeTransform.localScale.z
            );

            // Plane �߽��� �������� ���� ��ġ ����
            float randomX = Random.Range(-planeSize.x / 2, planeSize.x / 2);
            float randomZ = Random.Range(-planeSize.z / 2, planeSize.z / 2);

            // Plane�� ���� ��ǥ�迡�� ������ ���� ��ġ�� ���� ��ǥ��� ��ȯ
            Vector3 localRandomPosition = new Vector3(randomX, 0f, randomZ);
            return planeTransform.TransformPoint(localRandomPosition);
        }

        // [CHANGED] SpawnPlayerCharacter: ���õ� ĳ����(SelectedCharacter) ���
        void SpawnPlayerCharacter()
        {
            // Ŀ���� ������Ƽ���� �������ų�, �⺻ "Warrior"
            string charName = "Arrowbot";
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("SelectedCharacter", out object obj))
            {
                charName = (string)obj;
            }

            Vector3 spawnPosition = GetRandomPositionOnPlane(planeTransform); // Plane������Ʈ ���� ���� ��ġ

            // [CHANGED] Characters/{charName} �������� Instantiate
            PhotonNetwork.Instantiate($"Characters/{charName}", spawnPosition, Quaternion.identity);
        }

        IEnumerator C_WaitUntilAllPlayerCharactersAreSpawned()
        {
            while (true)
            {
                bool allReady = true;

                foreach (Player player in PhotonNetwork.PlayerList)
                {
                    if (player.CustomProperties.TryGetValue(PlayerInGamePlayPropertyKey.IS_CHARACTER_SPAWNED, out object isSpawnedObj))
                    {
                        if (!(bool)isSpawnedObj)
                        {
                            allReady = false;
                            break;
                        }
                    }
                    else
                    {
                        allReady = false;
                        break;
                    }
                }

                if (allReady) break;

                yield return _waitFor1Seconds;
            }
        }
    }
}


