using UnityEngine;

public class FirstPlatform : MonoBehaviour
{
    public GameObject alternateObject; // �� ��° �÷��� ������

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            SwapPlatform();
        }
    }

    void SwapPlatform()
    {
        if (alternateObject != null)
        {
            // �� ��° �÷��� ����
            Instantiate(alternateObject, transform.position, transform.rotation);
        }

        // ù ��° �÷��� ����
        Destroy(gameObject);

        Debug.Log("First platform swapped with alternate object.");
    }
}