using UnityEngine;

public class SecondPlatform : MonoBehaviour
{
    public float shakeDuration = 1.0f;  // 흔들리는 시간
    public float shakeAmount = 0.1f;    // 흔들림의 세기
    public float stepHoldTime = 1.0f;   // 밟은 상태로 유지하는 시간 (1초)

    private bool isShaking = false;     // 흔들림 상태
    private bool hasBeenSteppedOn = false; // 첫 번째 밟은 상태
    private float stepTimer = 0f;       // 밟은 시간
    private Vector3 originalPosition;   // 원래 위치

    private bool playerIsOnPlatform = false; // 플레이어가 플랫폼에 있는지 확인

    void Start()
    {
        // 원래 위치 저장
        originalPosition = transform.position;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            playerIsOnPlatform = true;

            if (!hasBeenSteppedOn)
            {
                // 첫 번째로 밟았을 때, 상태 기록
                hasBeenSteppedOn = true;
                stepTimer = 0f; // 타이머 초기화
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            playerIsOnPlatform = false; // 플레이어가 플랫폼을 떠났을 때
        }
    }

    void Update()
    {
        if (hasBeenSteppedOn && playerIsOnPlatform)
        {
            stepTimer += Time.deltaTime;  // 밟고 있는 시간이 누적됨

            // 밟은 후 일정 시간 이상 지났을 때 (1초)
            if (stepTimer >= stepHoldTime && !isShaking)
            {
                StartShake();
            }
        }

        if (isShaking)
        {
            Shake();
            shakeDuration -= Time.deltaTime; // 흔들림 시간 감소

            if (shakeDuration <= 0f)
            {
                DestroyPlatform(); // 흔들림 종료 후 플랫폼 삭제
            }
        }
    }

    void StartShake()
    {
        isShaking = true;
        Debug.Log("Second platform started shaking.");
    }

    void Shake()
    {
        // 플랫폼의 위치를 랜덤하게 변경
        Vector3 shakeOffset = new Vector3(
            Random.Range(-shakeAmount, shakeAmount),
            Random.Range(-shakeAmount, shakeAmount),
            0
        );

        transform.position = originalPosition + shakeOffset;
    }

    void DestroyPlatform()
    {
        isShaking = false; // 흔들림 종료
        transform.position = originalPosition; // 원래 위치로 복원

        // 플랫폼 삭제
        Destroy(gameObject);
        Debug.Log("Second platform destroyed after shaking.");
    }
}