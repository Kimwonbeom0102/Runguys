using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class SignupManager : MonoBehaviour
{
    public TMP_InputField IDInputField;
    public TMP_InputField PasswordInputField;
    public TMP_InputField NicknameInputField;
    public GameObject NotificationCanvas;
    public TextMeshProUGUI NotificationText;
    public GameObject SignupCanvas;

    [SerializeField] private CloudSaveManager cloudSaveManager;

    private void Start()
    {
        if (cloudSaveManager == null)
        {
            Debug.LogError("CloudSaveManager가 할당되지 않았습니다.");
        }

        // [ADDED] Nickname 입력 필드에서 엔터키 입력 시 회원가입 완료 처리
        NicknameInputField.onEndEdit.AddListener(OnNicknameEndEdit);
    }

    public async void OnSignupCompleteButtonClicked()
    {
        // 입력 필드가 비어있는 경우 처리
        if (string.IsNullOrEmpty(IDInputField.text) ||
            string.IsNullOrEmpty(PasswordInputField.text) ||
            string.IsNullOrEmpty(NicknameInputField.text))
        {
            ShowNotification("모든 칸을 채워주세요!");
            return;
        }

        // 비밀번호 길이 확인
        if (PasswordInputField.text.Length < 6)
        {
            ShowNotification("비밀번호는 최소 6자 이상이어야 합니다.");
            return;
        }

        // 모든 유저 데이터 로드
        var existingUsers = await cloudSaveManager.LoadAllUserData();
        if (existingUsers != null)
        {
            // 동일한 ID 또는 닉네임이 있는지 확인
            if (existingUsers.Any(user => user["UserID"] == IDInputField.text))
            {
                ShowNotification("이미 사용 중인 ID입니다. 다른 ID를 입력하세요.");
                return;
            }

            if (existingUsers.Any(user => user["Nickname"] == NicknameInputField.text))
            {
                ShowNotification("이미 사용 중인 닉네임입니다. 다른 닉네임을 입력하세요.");
                return;
            }
        }

        // Cloud Save로 유저 데이터 저장
        await cloudSaveManager.SaveUserData(
            IDInputField.text,
            PasswordInputField.text,
            NicknameInputField.text);

        ShowNotification("회원가입 완료!");
        SignupCanvas.SetActive(false);
    }

    public void OnCancelButtonClicked()
    {
        SignupCanvas.SetActive(false);
    }

    private void ShowNotification(string message)
    {
        NotificationCanvas.SetActive(true);
        NotificationText.text = message;
    }

    // 확인 버튼 클릭 시 알림창 닫기
    public void OnNotificationConfirmButtonClicked()
    {
        NotificationCanvas.SetActive(false);
    }

    // [ADDED] Nickname 입력 필드에서 엔터키 입력 시 회원가입 완료 버튼 실행
    private void OnNicknameEndEdit(string text)
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            OnSignupCompleteButtonClicked();
        }
    }
}
