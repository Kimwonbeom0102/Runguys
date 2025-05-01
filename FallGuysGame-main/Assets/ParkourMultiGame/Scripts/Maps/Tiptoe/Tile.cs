using System.Collections;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [Header("Tile Settings")]
    public bool isReal = false; // 진짜 타일 여부
    public Color highlightColor = Color.green; // 진짜 타일 하이라이트 색상

    private Renderer tileRenderer;
    private Collider tileCollider;
    private Material tileMaterial; // 타일의 머티리얼

    private bool isEffectActive = false; // 셰이더 효과가 활성화 중인지 여부
    private Coroutine resetEffectCoroutine; // 효과를 초기화하는 코루틴

    private void Start()
    {
        // 타일의 Renderer와 Collider 가져오기
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

        // 타일 머티리얼을 저장
        tileMaterial = tileRenderer.material;

        // 기본 상태로 초기화
        tileMaterial.SetColor("_BaseColor", Color.white);  // "_BaseColor"는 셰이더 프로퍼티
        tileMaterial.SetColor("_EmissionColor", Color.black); // 기본 Emission 비활성화
        tileRenderer.material.DisableKeyword("_EMISSION");

        // 충돌 상태 초기화
        SetCollisionState();

        // 디버그 출력
        Debug.Log($"{gameObject.name} - isReal: {isReal}");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isReal && collision.collider.CompareTag("Player")) // 진짜 타일과 플레이어 충돌
        {
            ActivateEffect(); // 효과 활성화
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (isReal && collision.collider.CompareTag("Player")) // 진짜 타일에서 플레이어가 떠날 때
        {
            if (resetEffectCoroutine != null)
            {
                StopCoroutine(resetEffectCoroutine); // 기존 초기화 코루틴 중지
            }
            resetEffectCoroutine = StartCoroutine(ResetEffectAfterDelay(2f)); // 2초 후 효과 제거
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isReal && other.CompareTag("Player")) // 가짜 타일과 플레이어 트리거
        {
            ActivateEffect(); // 효과 활성화

            // 가짜 타일을 삭제
            StartCoroutine(DestroyFakeTile());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!isReal && other.CompareTag("Player")) // 가짜 타일에서 플레이어가 떠날 때
        {
            if (resetEffectCoroutine != null)
            {
                StopCoroutine(resetEffectCoroutine); // 기존 초기화 코루틴 중지
            }
            resetEffectCoroutine = StartCoroutine(ResetEffectAfterDelay(2f)); // 2초 후 효과 제거
        }
    }

    private void ActivateEffect()
    {
        // 셰이더 효과 활성화
        tileMaterial.SetColor("_EmissionColor", highlightColor);
        tileRenderer.material.EnableKeyword("_EMISSION");
        isEffectActive = true;

        // 기존 초기화 코루틴이 실행 중이면 중지
        if (resetEffectCoroutine != null)
        {
            StopCoroutine(resetEffectCoroutine);
            resetEffectCoroutine = null;
        }
    }

    private IEnumerator ResetEffectAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // 셰이더 효과 비활성화
        tileMaterial.SetColor("_EmissionColor", Color.black);
        tileRenderer.material.DisableKeyword("_EMISSION");
        isEffectActive = false;
    }

    private IEnumerator DestroyFakeTile()
    {
        yield return new WaitForSeconds(0.5f); // 약간의 딜레이 후
        Destroy(gameObject); // 가짜 타일 제거
    }

    public void ResetTile()
    {
        // 타일 상태 초기화
        isReal = false;

        // 셰이더 효과 제거
        if (resetEffectCoroutine != null)
        {
            StopCoroutine(resetEffectCoroutine); // 초기화 코루틴 중지
        }

        tileMaterial.SetColor("_EmissionColor", Color.black);
        tileRenderer.material.DisableKeyword("_EMISSION");
        isEffectActive = false;

        // 충돌 상태 초기화
        SetCollisionState();
    }

    private void SetCollisionState()
    {
        // 진짜 타일은 물리적 충돌을 활성화, 가짜 타일은 Trigger로 설정
        if (isReal)
        {
            tileCollider.isTrigger = false; // 충돌을 활성화
        }
        else
        {
            tileCollider.isTrigger = true; // Trigger로 설정
        }
    }
}