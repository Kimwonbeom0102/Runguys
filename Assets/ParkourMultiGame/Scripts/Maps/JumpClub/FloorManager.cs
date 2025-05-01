using UnityEngine;

public class FloorManager : MonoBehaviour
{
    public GameObject[] floors; // �ٴ� �迭
    public float timeInterval = 15.0f; // �ٴ��� ������� ����
    public float shakeDuration = 3.0f; // ��鸮�� �ð�
    public float shakeIntensity = 0.2f; // ��鸲 ����

    private int currentFloorIndex = 0; // ���� ����� �ٴ��� �ε���
    private float timer = 0.0f; // �ð� ���� Ÿ�̸�
    private float shakeTimer = 0.0f; // ��鸲 Ÿ�̸�
    private bool isShaking = false; // ���� ��鸲 ����

    private Vector3 originalPosition; // ��鸮�� �� �ٴ��� ���� ��ġ

    void Update()
    {
        // ��鸲�� �ƴ� ���¿��� �Ϲ� Ÿ�̸� ����
        if (!isShaking)
        {
            timer += Time.deltaTime;

            // ������ ���ݿ� �����ϸ� ��鸲 ����
            if (timer >= timeInterval && currentFloorIndex < floors.Length)
            {
                StartShaking(floors[currentFloorIndex]);
            }
        }
        else
        {
            // ��鸲 ���� �� ��鸲 ȿ�� ó��
            ShakeFloor(floors[currentFloorIndex]);

            // ��鸲 Ÿ�̸� ����
            shakeTimer += Time.deltaTime;

            // ��鸲 �ð��� ������ �ٴ� ��Ȱ��ȭ
            if (shakeTimer >= shakeDuration)
            {
                StopShaking(floors[currentFloorIndex]);
                currentFloorIndex++; // ���� �ٴ����� �̵�
            }
        }
    }

    // ��鸲 ����
    private void StartShaking(GameObject floor)
    {
        isShaking = true;
        shakeTimer = 0.0f; // ��鸲 Ÿ�̸� �ʱ�ȭ
        originalPosition = floor.transform.position; // ���� ��ġ ����
    }

    // ��鸲 ó��
    private void ShakeFloor(GameObject floor)
    {
        float x = Random.Range(-shakeIntensity, shakeIntensity);
        float y = Random.Range(-shakeIntensity, shakeIntensity);
        float z = Random.Range(-shakeIntensity, shakeIntensity);

        floor.transform.position = originalPosition + new Vector3(x, y, z);
    }

    // ��鸲 ���� �� �ٴ� ��Ȱ��ȭ
    private void StopShaking(GameObject floor)
    {
        isShaking = false;
        floor.transform.position = originalPosition; // ���� ��ġ�� ����
        floor.SetActive(false); // �ٴ� ��Ȱ��ȭ
        timer = 0.0f; // ���� Ÿ�̸� �ʱ�ȭ
    }
}