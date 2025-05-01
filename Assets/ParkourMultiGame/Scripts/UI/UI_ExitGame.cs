using Photon.Pun;
using Practices.UGUI_Management.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Practices.PhotonPunClient.UI
{
    // Photon 콜백 사용을 위해 MonoBehaviourPunCallbacks 상속
    public class UI_ExitGame : MonoBehaviourPunCallbacks
    {
        [SerializeField] private Button _exitButton;
        // Button을 Inspector에서 참조할 수 있도록 SerializeField

        private void Start()
        {
            // 버튼이 누락되지 않았는지 체크
            if (_exitButton == null)
            {
                Debug.LogError($"[{nameof(UI_ExitGame)}] _exitButton is not assigned!");
                return;
            }

            // onClick 에 메서드 바인딩
            _exitButton.onClick.AddListener(OnClickExitGame);
        }

        /// <summary>
        /// 종료 버튼 클릭 시 호출
        /// </summary>
        private void OnClickExitGame()
        {
            // Photon 방을 떠나는 로직
            // 방에서 나가면 OnLeftRoom 콜백이 자동 호출됩니다.
            PhotonNetwork.LeaveRoom();
        }

        /// <summary>
        /// Photon 룸을 떠난 뒤에 호출되는 콜백
        /// </summary>
        public override void OnLeftRoom()
        {
            base.OnLeftRoom();

            // Lobby 씬으로 돌아가거나, 원하는 씬으로 이동
            SceneManager.LoadScene("Lobby");
        }
    }
}
