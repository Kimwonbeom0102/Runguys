using UnityEngine;

public class SecondPlatform : MonoBehaviour
{
    public float shakeDuration = 1.0f;  // ��鸮�� �ð�
    public float shakeAmount = 0.1f;    // ��鸲�� ����
    public float stepHoldTime = 1.0f;   // ���� ���·� �����ϴ� �ð� (1��)

    private bool isShaking = false;     // ��鸲 ����
    private bool hasBeenSteppedOn = false; // ù ��° ���� ����
    private float stepTimer = 0f;       // ���� �ð�
    private Vector3 originalPosition;   // ���� ��ġ

    private bool playerIsOnPlatform = false; // �÷��̾ �÷����� �ִ��� Ȯ��

    void Start()
    {
        // ���� ��ġ ����
        originalPosition = transform.position;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            playerIsOnPlatform = true;

            if (!hasBeenSteppedOn)
            {
                // ù ��°�� ����� ��, ���� ���
                hasBeenSteppedOn = true;
                stepTimer = 0f; // Ÿ�̸� �ʱ�ȭ
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            playerIsOnPlatform = false; // �÷��̾ �÷����� ������ ��
        }
    }

    void Update()
    {
        if (hasBeenSteppedOn && playerIsOnPlatform)
        {
            stepTimer += Time.deltaTime;  // ��� �ִ� �ð��� ������

            // ���� �� ���� �ð� �̻� ������ �� (1��)
            if (stepTimer >= stepHoldTime && !isShaking)
            {
                StartShake();
            }
        }

        if (isShaking)
        {
            Shake();
            shakeDuration -= Time.deltaTime; // ��鸲 �ð� ����

            if (shakeDuration <= 0f)
            {
                DestroyPlatform(); // ��鸲 ���� �� �÷��� ����
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
        // �÷����� ��ġ�� �����ϰ� ����
        Vector3 shakeOffset = new Vector3(
            Random.Range(-shakeAmount, shakeAmount),
            Random.Range(-shakeAmount, shakeAmount),
            0
        );

        transform.position = originalPosition + shakeOffset;
    }

    void DestroyPlatform()
    {
        isShaking = false; // ��鸲 ����
        transform.position = originalPosition; // ���� ��ġ�� ����

        // �÷��� ����
        Destroy(gameObject);
        Debug.Log("Second platform destroyed after shaking.");
    }
}