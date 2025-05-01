using UnityEngine;

public class Rotation : MonoBehaviour
{
    public enum RotationType
    {
        Fix,
        Range
    }

    public enum RotationAxis
    {
        X,
        Y,
        Z
    }

    public RotationType rotationType = RotationType.Fix; // Fix 또는 Range 선택
    public RotationAxis rotationAxis = RotationAxis.Y;   // X, Y, Z 축 선택
    public float fixedSpeed = 50.0f;                     // Fix 모드의 고정 속도
    public Vector2 speedRange = new Vector2(10.0f, 100.0f); // Range 모드에서 속도 범위 (최소, 최대)

    private float currentSpeed;   // 현재 회전 속도
    private float timer = 0.0f;   // 10초 타이머

    void Start()
    {
        // 시작 시 현재 속도를 초기화
        if (rotationType == RotationType.Fix)
        {
            currentSpeed = fixedSpeed;
        }
        else if (rotationType == RotationType.Range)
        {
            currentSpeed = Random.Range(speedRange.x, speedRange.y);
        }
    }

    void Update()
    {
        float rotationValue = currentSpeed * Time.deltaTime;

        // 10초마다 속도 변경 (Range 모드에서만)
        if (rotationType == RotationType.Range)
        {
            timer += Time.deltaTime;
            if (timer >= 10.0f)
            {
                timer = 0.0f;
                currentSpeed = Random.Range(speedRange.x, speedRange.y);
            }
        }

        // Rotation Axis
        Vector3 axis = Vector3.zero;
        switch (rotationAxis)
        {
            case RotationAxis.X:
                axis = Vector3.right;
                break;
            case RotationAxis.Y:
                axis = Vector3.up;
                break;
            case RotationAxis.Z:
                axis = Vector3.forward;
                break;
        }

        // Rotate object
        transform.Rotate(axis, rotationValue);
    }
}