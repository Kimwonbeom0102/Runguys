using UnityEngine;

public class WallMover : MonoBehaviour
{
    private Vector3 targetPosition; // �� ���� ��ġ
    private float speed; // �̵� �ӵ�
    private bool isMoving = false; // �̵� ����

    // �ʱ� ����
    public void Setup(Vector3 targetPos, float moveSpeed)
    {
        targetPosition = targetPos;
        speed = moveSpeed;
        isMoving = true; // �̵� ����
    }

    void Update()
    {
        if (isMoving)
        {
            // �� �������� �̵�
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

            // �� ������ �����ϸ� ����
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                Destroy(gameObject);
            }
        }
    }
}