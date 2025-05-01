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
            Debug.LogError("CloudSaveManager�� �Ҵ���� �ʾҽ��ϴ�.");
        }

        // [ADDED] Nickname �Է� �ʵ忡�� ����Ű �Է� �� ȸ������ �Ϸ� ó��
        NicknameInputField.onEndEdit.AddListener(OnNicknameEndEdit);
    }

    public async void OnSignupCompleteButtonClicked()
    {
        // �Է� �ʵ尡 ����ִ� ��� ó��
        if (string.IsNullOrEmpty(IDInputField.text) ||
            string.IsNullOrEmpty(PasswordInputField.text) ||
            string.IsNullOrEmpty(NicknameInputField.text))
        {
            ShowNotification("��� ĭ�� ä���ּ���!");
            return;
        }

        // ��й�ȣ ���� Ȯ��
        if (PasswordInputField.text.Length < 6)
        {
            ShowNotification("��й�ȣ�� �ּ� 6�� �̻��̾�� �մϴ�.");
            return;
        }

        // ��� ���� ������ �ε�
        var existingUsers = await cloudSaveManager.LoadAllUserData();
        if (existingUsers != null)
        {
            // ������ ID �Ǵ� �г����� �ִ��� Ȯ��
            if (existingUsers.Any(user => user["UserID"] == IDInputField.text))
            {
                ShowNotification("�̹� ��� ���� ID�Դϴ�. �ٸ� ID�� �Է��ϼ���.");
                return;
            }

            if (existingUsers.Any(user => user["Nickname"] == NicknameInputField.text))
            {
                ShowNotification("�̹� ��� ���� �г����Դϴ�. �ٸ� �г����� �Է��ϼ���.");
                return;
            }
        }

        // Cloud Save�� ���� ������ ����
        await cloudSaveManager.SaveUserData(
            IDInputField.text,
            PasswordInputField.text,
            NicknameInputField.text);

        ShowNotification("ȸ������ �Ϸ�!");
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

    // Ȯ�� ��ư Ŭ�� �� �˸�â �ݱ�
    public void OnNotificationConfirmButtonClicked()
    {
        NotificationCanvas.SetActive(false);
    }

    // [ADDED] Nickname �Է� �ʵ忡�� ����Ű �Է� �� ȸ������ �Ϸ� ��ư ����
    private void OnNicknameEndEdit(string text)
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            OnSignupCompleteButtonClicked();
        }
    }
}
