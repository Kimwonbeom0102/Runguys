using UnityEngine;

public class ClubHitEffect : MonoBehaviour
{
    public float knockbackForce = 30.0f; // �÷��̾ ���ư��� ��
    public float upwardForce = 5.0f;    // �������� ���ư��� �� �߰� (���� ȿ��)

    private void OnCollisionEnter(Collision collision)
    {
        // ���� ���� ������Ʈ�� "Player" �±׸� ������ �ִ��� Ȯ��
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody playerRigidbody = collision.gameObject.GetComponent<Rigidbody>();

            // Rigidbody�� �ִ� ��쿡�� �۵�
            if (playerRigidbody != null)
            {
                // �浹�� �������� �÷��̾�� ���ư��� �� ���
                Vector3 knockbackDirection = (collision.transform.position - transform.position).normalized;
                knockbackDirection.y = 0.0f; // ���� �������θ� ���� ���ϵ��� �ʱ�ȭ
                Vector3 finalForce = knockbackDirection * knockbackForce + Vector3.up * upwardForce;

                // �÷��̾� Rigidbody�� ���� ����
                playerRigidbody.AddForce(finalForce, ForceMode.Impulse);
            }
        }
    }
}