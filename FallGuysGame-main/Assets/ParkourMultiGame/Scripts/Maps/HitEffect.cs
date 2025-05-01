using UnityEngine;

public class ClubHitEffect : MonoBehaviour
{
    public float knockbackForce = 30.0f; // 플레이어가 날아가는 힘
    public float upwardForce = 5.0f;    // 위쪽으로 날아가는 힘 추가 (점프 효과)

    private void OnCollisionEnter(Collision collision)
    {
        // 봉에 닿은 오브젝트가 "Player" 태그를 가지고 있는지 확인
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody playerRigidbody = collision.gameObject.GetComponent<Rigidbody>();

            // Rigidbody가 있는 경우에만 작동
            if (playerRigidbody != null)
            {
                // 충돌한 지점에서 플레이어에게 날아가는 힘 계산
                Vector3 knockbackDirection = (collision.transform.position - transform.position).normalized;
                knockbackDirection.y = 0.0f; // 수평 방향으로만 힘을 가하도록 초기화
                Vector3 finalForce = knockbackDirection * knockbackForce + Vector3.up * upwardForce;

                // 플레이어 Rigidbody에 힘을 가함
                playerRigidbody.AddForce(finalForce, ForceMode.Impulse);
            }
        }
    }
}