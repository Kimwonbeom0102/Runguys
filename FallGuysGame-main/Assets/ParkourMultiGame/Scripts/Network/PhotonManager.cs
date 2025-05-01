
using Photon.Pun;
using Photon.Realtime;
using Practices.UGUI_Management.UI;
using UnityEngine;

namespace Practices.PhotonPunClient.Network
{
    public class PhotonManager : MonoBehaviourPunCallbacks
    {
        public static PhotonManager instance
        {
            get
            {
                if (s_instance == null)
                {
                    s_instance = new GameObject(nameof(PhotonManager)).AddComponent<PhotonManager>();
                }
                return s_instance;
            }
        }

        static PhotonManager s_instance;

        private void Awake()
        {
            if (s_instance)
            {
                Destroy(gameObject);
                return;
            }
            else
            {
                s_instance = this;
            }

            if (!PhotonNetwork.IsConnected)
            {
#if UNITY_EDITOR
                PhotonNetwork.LogLevel = PunLogLevel.Full;
                Application.runInBackground = true;
#endif
                PhotonNetwork.AuthValues = new AuthenticationValues();
                PhotonNetwork.AuthValues.UserId = System.Guid.NewGuid().ToString(); // [CHANGED] 임의의 UserId 설정
                PhotonNetwork.NickName = "Player_" + Random.Range(1000, 9999).ToString();
                bool isConnected = PhotonNetwork.ConnectUsingSettings();
                Debug.Assert(isConnected, $"[{nameof(PhotonManager)}] Photon 서버 연결 실패.");
            }

            DontDestroyOnLoad(gameObject);
        }

        public override void OnConnectedToMaster()
        {
            base.OnConnectedToMaster();

            PhotonNetwork.AutomaticallySyncScene = true;
            Debug.Log($"[{nameof(PhotonManager)}] 마스터 서버에 연결됨.");
            PhotonNetwork.JoinLobby();
        }

        public override void OnJoinedLobby()
        {
            base.OnJoinedLobby();
            Debug.Log($"[{nameof(PhotonManager)}] 로비에 가입됨.");
        }

        //[ADDED]
        public override void OnDisconnected(DisconnectCause cause)
        {
            base.OnDisconnected(cause);
            Debug.Log($"[PhotonManager] OnDisconnected 원인={cause}");

            if (cause == DisconnectCause.AuthenticationTicketExpired)
            {
                // [ADDED] 재로그인 메시지 표시
                Debug.LogWarning("재로그인 되었습니다. (동일 ID로 다른 PC에서 로그인)");
                UI_ConfirmWindow cw = UI_Manager.instance.Resolve<UI_ConfirmWindow>();
                cw.Show("재로그인 되었습니다.");
            }
        }
    }
}
