using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;       // �÷��̾��� �̵� �ӵ�
    public float jumpForce = 7f;      // ���� ��
    public LayerMask groundLayer;     // �� ���̾� ����
    public Transform groundCheck;     // �� Ȯ�� ��ġ
    public float groundCheckRadius = 0.2f; // �� Ȯ�� �ݰ�

    private Rigidbody rb;             // �÷��̾��� Rigidbody
    private bool isGrounded;          // �÷��̾ ���� �ִ��� ����

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // �̵� ó��
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(moveX, 0, moveZ) * moveSpeed;
        move = transform.TransformDirection(move); // ���� ������ ���� �������� ��ȯ
        rb.linearVelocity = new Vector3(move.x, rb.linearVelocity.y, move.z);

        // ���� ó��
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void OnDrawGizmosSelected()
    {
        // �� Ȯ�� Gizmo
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(groundCheck.position, groundCheckRadius);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // '    ' 태그 충돌 처리
        if (other.CompareTag("Deadline"))
        {
            // 현재 리스폰 위치로 플레이어 이동
            RespawnManager.Instance.RespawnPlayer(gameObject);
        }

        // 'Checkpoint' 태그 충돌 처리
        if (other.CompareTag("Checkpoint"))
        {
            // 새 체크포인트를 리스폰 매니저에 전달
            RespawnManager.Instance.UpdateRespawnPoint(other.transform);
        }
    }
}
