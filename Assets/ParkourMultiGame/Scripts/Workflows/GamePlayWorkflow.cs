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
            // Plane의 크기를 계산 (기본 크기 10x10 반영)
            Vector3 planeSize = new Vector3(
                planeTransform.localScale.x,
                planeTransform.localScale.y,
                planeTransform.localScale.z
            );

            // Plane 중심을 기준으로 랜덤 위치 생성
            float randomX = Random.Range(-planeSize.x / 2, planeSize.x / 2);
            float randomZ = Random.Range(-planeSize.z / 2, planeSize.z / 2);

            // Plane의 로컬 좌표계에서 생성된 랜덤 위치를 월드 좌표계로 변환
            Vector3 localRandomPosition = new Vector3(randomX, 0f, randomZ);
            return planeTransform.TransformPoint(localRandomPosition);
        }

        // [CHANGED] SpawnPlayerCharacter: 선택된 캐릭터(SelectedCharacter) 사용
        void SpawnPlayerCharacter()
        {
            // 커스텀 프로퍼티에서 가져오거나, 기본 "Warrior"
            string charName = "Arrowbot";
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("SelectedCharacter", out object obj))
            {
                charName = (string)obj;
            }

            Vector3 spawnPosition = GetRandomPositionOnPlane(planeTransform); // Plane오브젝트 위에 랜덤 위치

            // [CHANGED] Characters/{charName} 프리팹을 Instantiate
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


