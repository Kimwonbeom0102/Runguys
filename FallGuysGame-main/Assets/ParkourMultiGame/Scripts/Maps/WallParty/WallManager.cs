using UnityEngine;

public class WallManager : MonoBehaviour
{
    public GameObject[] wallPatterns; // 벽 패턴 Prefab 배열
    public Transform startPoint; // 벽이 시작되는 위치
    public Transform endPoint; // 벽이 도달해야 할 끝 지점
    public float spawnInterval = 3.0f; // 벽 생성 간격
    public float moveSpeed = 5.0f; // 벽 이동 속도

    private float timer = 0.0f; // 생성 타이머

    private void Start()
    {
        SpawnWall();
    }
    void Update()
    {
        // 타이머 증가
        timer += Time.deltaTime;

        // 벽 생성 간격에 도달했을 때
        if (timer >= spawnInterval)
        {
            timer = 0.0f; // 타이머 초기화
            SpawnWall(); // 벽 생성
        }
    }

    // 벽 생성
    void SpawnWall()
    {
        // 랜덤 패턴 선택
        int randomIndex = Random.Range(0, wallPatterns.Length);

        // 벽 생성
        GameObject newWall = Instantiate(wallPatterns[randomIndex], startPoint.position, Quaternion.identity);

        // 벽 이동 스크립트 추가 및 설정
        WallMover wallMover = newWall.AddComponent<WallMover>();
        wallMover.Setup(endPoint.position, moveSpeed);
    }
}