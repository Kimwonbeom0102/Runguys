using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    public static RespawnManager Instance;

    [Header("Respawn Points")]
    public Transform[] respawnPoints; // �ν����Ϳ��� �ݵ�� �Ҵ��� ��
    private Transform currentRespawnPoint; // ���� ������ ��ġ

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
            // [ADDED] �⺻��(���� ��ġ)�� fallback���� ���
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
    public Transform[] respawnPoints; // ������ ����Ʈ �迭 (�ν����Ϳ��� ����)
    private Transform currentRespawnPoint; // ���� ����� ������ ��ġ

    private void Awake()
    {
        // �̱��� ���� ����
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
        // �ʱ� ������ ��ġ ���� (�ν������� ù ��° ��ġ ���)
        if (respawnPoints != null && respawnPoints.Length > 0)
        {
            currentRespawnPoint = respawnPoints[0];
        }
        else
        {
            Debug.LogError("Respawn points are not assigned in the inspector!");
        }
    }

    // ������ ��ġ ���� (���ο� üũ����Ʈ ���� ��)
    public void UpdateRespawnPoint(Transform newRespawnPoint)
    {
        currentRespawnPoint = newRespawnPoint;
        Debug.Log($"Respawn point updated to: {newRespawnPoint.name}");
    }

    // �÷��̾� ������ ó��
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