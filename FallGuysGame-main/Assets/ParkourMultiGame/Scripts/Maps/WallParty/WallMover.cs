using UnityEngine;

public class WallMover : MonoBehaviour
{
    private Vector3 targetPosition; // 끝 지점 위치
    private float speed; // 이동 속도
    private bool isMoving = false; // 이동 상태

    // 초기 설정
    public void Setup(Vector3 targetPos, float moveSpeed)
    {
        targetPosition = targetPos;
        speed = moveSpeed;
        isMoving = true; // 이동 시작
    }

    void Update()
    {
        if (isMoving)
        {
            // 끝 지점으로 이동
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

            // 끝 지점에 도달하면 삭제
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                Destroy(gameObject);
            }
        }
    }
}