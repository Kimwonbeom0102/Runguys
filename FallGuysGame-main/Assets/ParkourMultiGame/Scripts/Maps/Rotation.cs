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

    public RotationType rotationType = RotationType.Fix; // Fix �Ǵ� Range ����
    public RotationAxis rotationAxis = RotationAxis.Y;   // X, Y, Z �� ����
    public float fixedSpeed = 50.0f;                     // Fix ����� ���� �ӵ�
    public Vector2 speedRange = new Vector2(10.0f, 100.0f); // Range ��忡�� �ӵ� ���� (�ּ�, �ִ�)

    private float currentSpeed;   // ���� ȸ�� �ӵ�
    private float timer = 0.0f;   // 10�� Ÿ�̸�

    void Start()
    {
        // ���� �� ���� �ӵ��� �ʱ�ȭ
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

        // 10�ʸ��� �ӵ� ���� (Range ��忡����)
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