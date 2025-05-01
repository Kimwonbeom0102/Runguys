using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems; // [ADDED] EventSystem 사용을 위해 추가
using TMPro;
using Photon.Pun;
using Photon.Realtime;  //[ADDED]
using System.Threading.Tasks;

public class LoginManager : MonoBehaviour
{
    public TMP_InputField IDInputField;
    public TMP_InputField PasswordInputField;
    public GameObject SignupCanvas;
    public GameObject NotificationCanvas;
    public TextMeshProUGUI NotificationText;

    [SerializeField] private CloudSaveManager cloudSaveManager;

    private void Start()
    {
        if (cloudSaveManager == null)
        {
            Debug.LogError("CloudSaveManager가 인스펙터에 할당되지 않았습니다.");
        }

        // [ADDED] ID와 패스워드 입력 필드에서 엔터키 입력 이벤트 리스너 등록
        IDInputField.onEndEdit.AddListener(OnIDEndEdit);      // [ADDED]
        PasswordInputField.onEndEdit.AddListener(OnPasswordEndEdit);  // [ADDED]
    }

    public async void OnLoginButtonClicked()
    {
        var userDataList = await cloudSaveManager.LoadAllUserData();
        if (userDataList != null)
        {
            foreach (var userData in userDataList)
            {
                if (userData["UserID"] == IDInputField.text &&
                    userData["UserPassword"] == PasswordInputField.text)
                {
                    // [CHANGED] ID/PW가 올바르면 해당 UserID로 Photon에 접속
                    PhotonNetwork.Disconnect(); // 깨끗한 상태를 위해 먼저 연결 해제
                    await Task.Delay(50);

                    // [ADDED] Photon 인증 설정
                    PhotonNetwork.AuthValues = new AuthenticationValues();
                    PhotonNetwork.AuthValues.UserId = IDInputField.text;
                    // 동일한 ID로 다른 클라이언트가 로그인 시 기존 세션은 종료됨

                    // [ADDED] Cloud Save에서 닉네임 가져오기
                    string nickname = userData["Nickname"];
                    if (!string.IsNullOrEmpty(nickname))
                    {
                        PhotonNetwork.NickName = nickname;
                    }
                    else
                    {
                        PhotonNetwork.NickName = IDInputField.text; // fallback
                    }

                    // [ADDED] 또는 아래와 같이도 가능:
                    // string foundNick = await cloudSaveManager.GetNicknameByUserID(IDInputField.text);
                    // PhotonNetwork.NickName = foundNick ?? IDInputField.text;

                    PhotonNetwork.ConnectUsingSettings();

                    Debug.Log("로그인 성공!");
                    SceneManager.LoadScene("Lobby");
                    return;
                }
            }
        }
        ShowNotification("아이디 또는 비밀번호가 올바르지 않습니다.");
    }

    public void OnSignupButtonClicked()
    {
        SignupCanvas.SetActive(true);
    }

    private void ShowNotification(string message)
    {
        NotificationCanvas.SetActive(true);
        NotificationText.text = message;
    }

    // [ADDED] ID 입력 필드에서 엔터키 입력 시 패스워드 입력 필드로 포커스 이동
    private void OnIDEndEdit(string text)
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            EventSystem.current.SetSelectedGameObject(PasswordInputField.gameObject);
        }
    }

    // [ADDED] 패스워드 입력 필드에서 엔터키 입력 시 로그인 버튼 클릭 처리
    private void OnPasswordEndEdit(string text)
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (!string.IsNullOrEmpty(IDInputField.text) && !string.IsNullOrEmpty(PasswordInputField.text))
            {
                OnLoginButtonClicked(); // 로그인 버튼 클릭 메서드 호출
            }
        }
    }
}
