using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    public static RespawnManager Instance;

    [Header("Respawn Points")]
    public Transform[] respawnPoints; // 인스펙터에서 반드시 할당할 것
    private Transform currentRespawnPoint; // 현재 리스폰 위치

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (respawnPoints != null && respawnPoints.Length > 0)
        {
            currentRespawnPoint = respawnPoints[0];
        }
        else
        {
            Debug.LogError("Respawn points are not assigned in the inspector!");
            // [ADDED] 기본값(현재 위치)를 fallback으로 사용
            currentRespawnPoint = this.transform;
        }
    }

    public void UpdateRespawnPoint(Transform newRespawnPoint)
    {
        currentRespawnPoint = newRespawnPoint;
        Debug.Log($"Respawn point updated to: {newRespawnPoint.name}");
    }

    public void RespawnPlayer(GameObject player)
    {
        if (currentRespawnPoint != null)
        {
            player.transform.position = currentRespawnPoint.position;
        }
        else
        {
            Debug.LogError("Current respawn point is not set!");
        }
    }
}



/*using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    public static RespawnManager Instance;

    [Header("Respawn Points")]
    public Transform[] respawnPoints; // 리스폰 포인트 배열 (인스펙터에서 설정)
    private Transform currentRespawnPoint; // 현재 저장된 리스폰 위치

    private void Awake()
    {
        // 싱글톤 패턴 설정
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // 초기 리스폰 위치 설정 (인스펙터의 첫 번째 위치 사용)
        if (respawnPoints != null && respawnPoints.Length > 0)
        {
            currentRespawnPoint = respawnPoints[0];
        }
        else
        {
            Debug.LogError("Respawn points are not assigned in the inspector!");
        }
    }

    // 리스폰 위치 갱신 (새로운 체크포인트 도달 시)
    public void UpdateRespawnPoint(Transform newRespawnPoint)
    {
        currentRespawnPoint = newRespawnPoint;
        Debug.Log($"Respawn point updated to: {newRespawnPoint.name}");
    }

    // 플레이어 리스폰 처리
    public void RespawnPlayer(GameObject player)
    {
        if (currentRespawnPoint != null)
        {
            player.transform.position = currentRespawnPoint.position;
        }
        else
        {
            Debug.LogError("Current respawn point is not set!");
        }
    }
}*/