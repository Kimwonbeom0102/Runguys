using Photon.Pun;    // [ADDED] Photon
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class LobbyGameManager : MonoBehaviourPunCallbacks
{
    // [CHANGED] 씬에 배치된 "로비 카메라"와 "로비 UI"를 참조
    [Header("Lobby Objects")]
    [SerializeField] private Camera lobbyCamera = null;    // 씬에 있는 카메라(메인 카메라 역할)
    [SerializeField] private Canvas lobbyCanvas = null;    // 로비 UI (방 목록, 버튼 등등)

    [Header("UI Buttons")]
    [SerializeField] private Button startGameButton = null; // "게임시작" 버튼

    [Header("Photon Settings")]
    [SerializeField] private string characterPrefabName = "Archer";
    // Resources/Characters/Archer 등 실제 경로 맞게 설정

    private bool isGameStarted = false;  // 로비 vs 게임 상태 판단용

    private void Start()
    {
        // [CHANGED] 로비 상태 초기화
        isGameStarted = false;

        // 로비카메라, 로비Canvas 켜기
        if (lobbyCamera) lobbyCamera.gameObject.SetActive(true);
        if (lobbyCanvas) lobbyCanvas.gameObject.SetActive(true);

        // 게임 시작 버튼 리스너
        if (startGameButton)
        {
            startGameButton.onClick.AddListener(OnClickStartGame);
        }
    }

    // [CHANGED] "StartGame" 버튼 클릭 시 호출
    private void OnClickStartGame()
    {
        if (isGameStarted) return;

        isGameStarted = true;

        // 로비 카메라와 UI 끄기
        if (lobbyCamera) lobbyCamera.gameObject.SetActive(false);
        if (lobbyCanvas) lobbyCanvas.gameObject.SetActive(false);

        // PhotonNetwork.Instantiate를 통해 캐릭터 생성
        // 실제로는 Vector3, Quaternion값을 원하는 스폰 위치/회전으로 조정
        Vector3 spawnPos = new Vector3(Random.Range(-3f, 3f), 0f, Random.Range(-3f, 3f));
        Quaternion spawnRot = Quaternion.identity;

        PhotonNetwork.Instantiate(characterPrefabName, spawnPos, spawnRot);
        // => 캐릭터 프리팹에 카메라가 들어있고, photonView.IsMine == true인 오브젝트만
        //    카메라.enabled = true 등으로 로컬 시점을 갖게 됨
    }

    // [CHANGED] Photon 콜백 (예: 다른 유저가 나갔을 때 등)
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        Debug.Log($"[LobbyGameManager] Player left: {otherPlayer.NickName}");
    }

    // [CHANGED] Optional: 게임이 끝나고 로비로 복귀하는 로직
    // 하나의 씬에서 로비로 돌아가려면?
    public void ReturnToLobby()
    {
        // 캐릭터, 게임오브젝트 정리
        // PhotonNetwork.Destroy(??) 등...

        isGameStarted = false;

        // 로비카메라, 로비UI 다시 켜기
        if (lobbyCamera) lobbyCamera.gameObject.SetActive(true);
        if (lobbyCanvas) lobbyCanvas.gameObject.SetActive(true);
    }
}
