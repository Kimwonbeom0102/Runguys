using UnityEngine;

public class FloorManager : MonoBehaviour
{
    public GameObject[] floors; // 바닥 배열
    public float timeInterval = 15.0f; // 바닥이 사라지는 간격
    public float shakeDuration = 3.0f; // 흔들리는 시간
    public float shakeIntensity = 0.2f; // 흔들림 강도

    private int currentFloorIndex = 0; // 현재 사라질 바닥의 인덱스
    private float timer = 0.0f; // 시간 계산용 타이머
    private float shakeTimer = 0.0f; // 흔들림 타이머
    private bool isShaking = false; // 현재 흔들림 상태

    private Vector3 originalPosition; // 흔들리기 전 바닥의 원래 위치

    void Update()
    {
        // 흔들림이 아닌 상태에서 일반 타이머 증가
        if (!isShaking)
        {
            timer += Time.deltaTime;

            // 지정된 간격에 도달하면 흔들림 시작
            if (timer >= timeInterval && currentFloorIndex < floors.Length)
            {
                StartShaking(floors[currentFloorIndex]);
            }
        }
        else
        {
            // 흔들림 중일 때 흔들림 효과 처리
            ShakeFloor(floors[currentFloorIndex]);

            // 흔들림 타이머 증가
            shakeTimer += Time.deltaTime;

            // 흔들림 시간이 끝나면 바닥 비활성화
            if (shakeTimer >= shakeDuration)
            {
                StopShaking(floors[currentFloorIndex]);
                currentFloorIndex++; // 다음 바닥으로 이동
            }
        }
    }

    // 흔들림 시작
    private void StartShaking(GameObject floor)
    {
        isShaking = true;
        shakeTimer = 0.0f; // 흔들림 타이머 초기화
        originalPosition = floor.transform.position; // 원래 위치 저장
    }

    // 흔들림 처리
    private void ShakeFloor(GameObject floor)
    {
        float x = Random.Range(-shakeIntensity, shakeIntensity);
        float y = Random.Range(-shakeIntensity, shakeIntensity);
        float z = Random.Range(-shakeIntensity, shakeIntensity);

        floor.transform.position = originalPosition + new Vector3(x, y, z);
    }

    // 흔들림 종료 및 바닥 비활성화
    private void StopShaking(GameObject floor)
    {
        isShaking = false;
        floor.transform.position = originalPosition; // 원래 위치로 복구
        floor.SetActive(false); // 바닥 비활성화
        timer = 0.0f; // 메인 타이머 초기화
    }
}