using UnityEngine;

public class WallManager : MonoBehaviour
{
    public GameObject[] wallPatterns; // �� ���� Prefab �迭
    public Transform startPoint; // ���� ���۵Ǵ� ��ġ
    public Transform endPoint; // ���� �����ؾ� �� �� ����
    public float spawnInterval = 3.0f; // �� ���� ����
    public float moveSpeed = 5.0f; // �� �̵� �ӵ�

    private float timer = 0.0f; // ���� Ÿ�̸�

    private void Start()
    {
        SpawnWall();
    }
    void Update()
    {
        // Ÿ�̸� ����
        timer += Time.deltaTime;

        // �� ���� ���ݿ� �������� ��
        if (timer >= spawnInterval)
        {
            timer = 0.0f; // Ÿ�̸� �ʱ�ȭ
            SpawnWall(); // �� ����
        }
    }

    // �� ����
    void SpawnWall()
    {
        // ���� ���� ����
        int randomIndex = Random.Range(0, wallPatterns.Length);

        // �� ����
        GameObject newWall = Instantiate(wallPatterns[randomIndex], startPoint.position, Quaternion.identity);

        // �� �̵� ��ũ��Ʈ �߰� �� ����
        WallMover wallMover = newWall.AddComponent<WallMover>();
        wallMover.Setup(endPoint.position, moveSpeed);
    }
}