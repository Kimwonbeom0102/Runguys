using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    // 설정창 Canvas (비활성화/활성화할 대상)
    [SerializeField] private GameObject canvasSettings;

    // 로비에서 설정창을 여는 버튼
    [SerializeField] private Button optionsButton;

    // 설정창에서 설정을 닫는 버튼
    [SerializeField] private Button confirmButton;

    // 게임을 종료하는 Exit 버튼
    [SerializeField] private Button exitButton;

    private void Start()
    {
        // 설정창을 처음에는 비활성화 상태로 설정
        if (canvasSettings != null)
            canvasSettings.SetActive(false);
        else
            Debug.LogError("Canvas Settings가 연결되지 않았습니다!");

        // 옵션 버튼 클릭 시 설정창 토글
        if (optionsButton != null)
            optionsButton.onClick.AddListener(ToggleSettings);
        else
            Debug.LogError("Options Button이 연결되지 않았습니다!");

        // 확인 버튼 클릭 시 설정창 닫기
        if (confirmButton != null)
            confirmButton.onClick.AddListener(ToggleSettings);
        else
            Debug.LogError("Confirm Button이 연결되지 않았습니다!");

        // Exit 버튼 클릭 시 게임 종료
        if (exitButton != null)
            exitButton.onClick.AddListener(ExitGame);
        else
            Debug.LogError("Exit Button이 연결되지 않았습니다!");
    }

    /// <summary>
    /// 설정창의 활성화/비활성화 상태를 토글합니다.
    /// </summary>
    private void ToggleSettings()
    {
        if (canvasSettings != null)
        {
            // 현재 상태 반대로 토글
            bool isActive = canvasSettings.activeSelf;
            canvasSettings.SetActive(!isActive);
        }
        else
        {
            Debug.LogError("Canvas Settings가 설정되지 않았습니다!");
        }
    }

    /// <summary>
    /// 게임을 종료합니다.
    /// </summary>
    private void ExitGame()
    {
        Debug.Log("게임 종료 버튼이 눌렸습니다!");

#if UNITY_EDITOR
        // 에디터에서 실행 중인 경우 플레이 모드를 중지
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // 빌드된 게임에서 프로그램 종료
        Application.Quit();
#endif
    }
}
