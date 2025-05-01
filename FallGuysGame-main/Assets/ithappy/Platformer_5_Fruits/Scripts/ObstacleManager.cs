using System.Collections.Generic;
using UnityEngine;

public enum ObstacleType
{
    Moving,       //�̵� ��ֹ�
    Rotating,     //ȸ�� ��ֹ�
    Visibility,   //���� ��ֹ�
    ShrinkingPlatform, //ũ�Ⱑ ���� �پ��� ��ֹ�
    DamagingPlatform //HP�� �����ϴ� ��ֹ�
}


public class ObstacleManager : MonoBehaviour
{
    public ObstacleType obstacleType;

    public List<Transform> points; //�̵� ��� ����Ʈ
    public float moveSpeed = 20f; //�̵� �ӵ�
    private int currenPointIndex = 0; //���� ��ǥ ���� �ε���

    public Vector3 rotationAxis = Vector3.up;
    public float rotationSpeed = 50f;

    private Renderer objectRenderer;
    public bool isVisible = true;

    public float shrinkRate = 0.1f; //�� ������ ũ�� ���� ����
    private Vector3 initialScale; //�ʱ� ũ��

    public float damageRate = 10.0f;

    void Start()
    {

        objectRenderer = GetComponent<Renderer>();

        if (points.Count > 0)
        {
            transform.position = points[0].position;
        }

        initialScale = transform.localScale;
    }

    void Update()
    {
        switch (obstacleType)
        {
            case ObstacleType.Moving:
                Movement();
                break;
            case ObstacleType.Rotating:
                Rotation();
                break;
            case ObstacleType.Visibility:
                break;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Player")
        {
            switch (obstacleType)
            {
                case ObstacleType.ShrinkingPlatform:
                    ShrinkingPlatform();
                    break;
                case ObstacleType.DamagingPlatform:
                    DamagingPlatform();
                    break;
            }
        }
    }

    void Movement()
    {
        if (points.Count == 0) return;

        //���� ��ǥ ����
        Transform targetPoint = points[currenPointIndex];

        // ���� ��ġ���� ��ǥ �������� �̵�
        transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, moveSpeed * Time.deltaTime);

        //��ǥ ������ �����ϸ� ���� �������� �ε����� ��ȯ
        if (transform.position == targetPoint.position)
        {
            currenPointIndex = (currenPointIndex + 1) % points.Count;
            Debug.Log(currenPointIndex);
        }
    }

    void Rotation()
    {
        transform.Rotate(rotationAxis * rotationSpeed * Time.deltaTime);
    }

    public void Visibleility()
    {
        objectRenderer.enabled = isVisible;
        gameObject.SetActive(isVisible);
    }

    void ShrinkingPlatform()
    {
        if (transform.localScale.x > 0.1f)
        {
            transform.localScale -= Vector3.one * shrinkRate;
        }
    }

    void DamagingPlatform()
    {
        //PlayerManager.Instance.TakeDamage(damageRate);
    }
}
