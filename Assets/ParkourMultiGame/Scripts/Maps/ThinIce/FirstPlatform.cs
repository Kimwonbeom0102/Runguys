using UnityEngine;

public class FirstPlatform : MonoBehaviour
{
    public GameObject alternateObject; // µÎ ¹øÂ° ÇÃ·§Æû ÇÁ¸®ÆÕ

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
            // µÎ ¹øÂ° ÇÃ·§Æû »ý¼º
            Instantiate(alternateObject, transform.position, transform.rotation);
        }

        // Ã¹ ¹øÂ° ÇÃ·§Æû »èÁ¦
        Destroy(gameObject);

        Debug.Log("First platform swapped with alternate object.");
    }
}