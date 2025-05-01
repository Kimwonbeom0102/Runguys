using System.Collections;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [Header("Tile Settings")]
    public bool isReal = false; // ��¥ Ÿ�� ����
    public Color highlightColor = Color.green; // ��¥ Ÿ�� ���̶���Ʈ ����

    private Renderer tileRenderer;
    private Collider tileCollider;
    private Material tileMaterial; // Ÿ���� ��Ƽ����

    private bool isEffectActive = false; // ���̴� ȿ���� Ȱ��ȭ ������ ����
    private Coroutine resetEffectCoroutine; // ȿ���� �ʱ�ȭ�ϴ� �ڷ�ƾ

    private void Start()
    {
        // Ÿ���� Renderer�� Collider ��������
        tileRenderer = GetComponent<Renderer>();
        tileCollider = GetComponent<Collider>();

        if (tileRenderer == null)
        {
            Debug.LogError("Tile Renderer is missing!");
            return;
        }

        if (tileCollider == null)
        {
            Debug.LogError("Tile Collider is missing!");
            return;
        }

        // Ÿ�� ��Ƽ������ ����
        tileMaterial = tileRenderer.material;

        // �⺻ ���·� �ʱ�ȭ
        tileMaterial.SetColor("_BaseColor", Color.white);  // "_BaseColor"�� ���̴� ������Ƽ
        tileMaterial.SetColor("_EmissionColor", Color.black); // �⺻ Emission ��Ȱ��ȭ
        tileRenderer.material.DisableKeyword("_EMISSION");

        // �浹 ���� �ʱ�ȭ
        SetCollisionState();

        // ����� ���
        Debug.Log($"{gameObject.name} - isReal: {isReal}");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isReal && collision.collider.CompareTag("Player")) // ��¥ Ÿ�ϰ� �÷��̾� �浹
        {
            ActivateEffect(); // ȿ�� Ȱ��ȭ
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (isReal && collision.collider.CompareTag("Player")) // ��¥ Ÿ�Ͽ��� �÷��̾ ���� ��
        {
            if (resetEffectCoroutine != null)
            {
                StopCoroutine(resetEffectCoroutine); // ���� �ʱ�ȭ �ڷ�ƾ ����
            }
            resetEffectCoroutine = StartCoroutine(ResetEffectAfterDelay(2f)); // 2�� �� ȿ�� ����
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isReal && other.CompareTag("Player")) // ��¥ Ÿ�ϰ� �÷��̾� Ʈ����
        {
            ActivateEffect(); // ȿ�� Ȱ��ȭ

            // ��¥ Ÿ���� ����
            StartCoroutine(DestroyFakeTile());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!isReal && other.CompareTag("Player")) // ��¥ Ÿ�Ͽ��� �÷��̾ ���� ��
        {
            if (resetEffectCoroutine != null)
            {
                StopCoroutine(resetEffectCoroutine); // ���� �ʱ�ȭ �ڷ�ƾ ����
            }
            resetEffectCoroutine = StartCoroutine(ResetEffectAfterDelay(2f)); // 2�� �� ȿ�� ����
        }
    }

    private void ActivateEffect()
    {
        // ���̴� ȿ�� Ȱ��ȭ
        tileMaterial.SetColor("_EmissionColor", highlightColor);
        tileRenderer.material.EnableKeyword("_EMISSION");
        isEffectActive = true;

        // ���� �ʱ�ȭ �ڷ�ƾ�� ���� ���̸� ����
        if (resetEffectCoroutine != null)
        {
            StopCoroutine(resetEffectCoroutine);
            resetEffectCoroutine = null;
        }
    }

    private IEnumerator ResetEffectAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // ���̴� ȿ�� ��Ȱ��ȭ
        tileMaterial.SetColor("_EmissionColor", Color.black);
        tileRenderer.material.DisableKeyword("_EMISSION");
        isEffectActive = false;
    }

    private IEnumerator DestroyFakeTile()
    {
        yield return new WaitForSeconds(0.5f); // �ణ�� ������ ��
        Destroy(gameObject); // ��¥ Ÿ�� ����
    }

    public void ResetTile()
    {
        // Ÿ�� ���� �ʱ�ȭ
        isReal = false;

        // ���̴� ȿ�� ����
        if (resetEffectCoroutine != null)
        {
            StopCoroutine(resetEffectCoroutine); // �ʱ�ȭ �ڷ�ƾ ����
        }

        tileMaterial.SetColor("_EmissionColor", Color.black);
        tileRenderer.material.DisableKeyword("_EMISSION");
        isEffectActive = false;

        // �浹 ���� �ʱ�ȭ
        SetCollisionState();
    }

    private void SetCollisionState()
    {
        // ��¥ Ÿ���� ������ �浹�� Ȱ��ȭ, ��¥ Ÿ���� Trigger�� ����
        if (isReal)
        {
            tileCollider.isTrigger = false; // �浹�� Ȱ��ȭ
        }
        else
        {
            tileCollider.isTrigger = true; // Trigger�� ����
        }
    }
}