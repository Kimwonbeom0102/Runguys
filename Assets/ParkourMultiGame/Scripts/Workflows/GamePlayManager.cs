using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using ExitGames.Client.Photon;
using Practices.UGUI_Management.UI; // [ADDED] 랭킹 팝업(UI_RankPopUp) 접근 위해

// 게임 모드 (생존 & 레이스)
public enum GameMode
{
    Survival,
    Race
}

public class GamePlayManager : MonoBehaviourPunCallbacks
{
    // [ADDED] 맵 제한시간 등 기존 로직 생략
    [SerializeField] private float _timeLimit = 300f;
    private float _startTime = 0f;
    private bool _gameFinished = false;

    // [ADDED] 레이스 결과 저장용 구조체
    private Dictionary<int, PlayerResult> _playerResults = new Dictionary<int, PlayerResult>();

    public struct PlayerResult
    {
        public string nickName;
        public bool finished;
        public double finishTime;
        public int rank;
        public bool leftEarly;
    }

    // [ADDED] 로딩된 맵 인스턴스를 참조
    private GameObject _mapInstance;

    //GameManager(규민)
    public GameMode currentGameMode;
    public List<string> survivalMaps;
    public List<string> raceMaps;
    private List<GameObject> activePlayers = new List<GameObject>();
    private int playersRequiredToEliminate;
    private string currentMap;
    private List<GameObject> raceFinishers = new List<GameObject>();
    public float roundTime = 60f;

    public GameObject soundManagerPrefab; // SoundManager 프리팹

    private void Awake()
    {
        // 필요 시 싱글톤 혹은 초기화
    }

    void Start()
    {
        _startTime = Time.time;

        // [ADDED] 방 참가한 모든 플레이어 Result 초기화
        foreach (var p in PhotonNetwork.PlayerList)
        {
            _playerResults[p.ActorNumber] = new PlayerResult
            {
                nickName = p.NickName,
                finished = false,
                finishTime = 0.0,
                rank = 0,
                leftEarly = false
            };
        }

        // [ADDED] 방에서 선택한 맵 prefab 스폰
        LoadSelectedMap();

        // [CHANGED] (필요하다면 캐릭터 스폰 로직 등)
        //SpawnPlayerCharacter();
    }

    private void Update()
    {
        // 마스터가 제한 시간 체크
        if (!PhotonNetwork.IsMasterClient) return;
        if (_gameFinished) return;

        float elapsed = Time.time - _startTime;
        float remain = _timeLimit - elapsed;
        if (remain <= 0f)
        {
            Debug.Log("[GamePlayManager] Time Limit Reached => Finalize!");
            FinalizeGameResults();
        }
    }

    // [ADDED] Room CustomProperty "SelectedMap" 읽어서 PhotonNetwork.InstantiateRoomObject
    private void LoadSelectedMap()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("SelectedMap", out object mapObj))
        {
            string selectedMap = mapObj as string;
            if (string.IsNullOrEmpty(selectedMap))
            {
                selectedMap = "Forest"; // fallback
            }

            if (PhotonNetwork.IsMasterClient)
            {
                string mapPath = $"Maps/{selectedMap}";
                GameObject mapPrefab = Resources.Load<GameObject>(mapPath);
                if (mapPrefab != null)
                {
                    _mapInstance = PhotonNetwork.InstantiateRoomObject(mapPath, Vector3.zero, Quaternion.identity);
                    Debug.Log($"[GamePlayManager] Map Loaded : {selectedMap}");
                }
                else
                {
                    Debug.LogError($"[GamePlayManager] Map not found : {mapPath}");
                }
            }
        }
        else
        {
            if (PhotonNetwork.IsMasterClient)
            {
                string defaultMap = "Forest";
                GameObject mapPrefab = Resources.Load<GameObject>($"Maps/{defaultMap}");
                if (mapPrefab != null)
                {
                    _mapInstance = PhotonNetwork.InstantiateRoomObject($"Maps/{defaultMap}", Vector3.zero, Quaternion.identity);
                    Debug.Log($"[GamePlayManager] Default Map Loaded: {defaultMap}");
                }
                else
                {
                    Debug.LogError("[GamePlayManager] Default map not found!");
                }
            }
        }
    }

    // [ADDED] 최종 결과 정산
    private void FinalizeGameResults()
    {
        if (_gameFinished) return;
        _gameFinished = true;

        // 등수 매기기
        var finishedList = _playerResults
            .Where(k => k.Value.finished)
            .OrderBy(k => k.Value.finishTime)
            .ToList();

        int rank = 1;
        foreach (var kvp in finishedList)
        {
            var pr = kvp.Value;
            pr.rank = rank++;
            _playerResults[kvp.Key] = pr;
        }

        // 로그 출력
        foreach (var kvp in _playerResults)
        {
            string msg = kvp.Value.leftEarly
                ? "(DNF-EarlyLeft)"
                : (!kvp.Value.finished ? "(DNF)" : $"Rank={kvp.Value.rank}, Time={kvp.Value.finishTime:F2}");

            Debug.Log($"[RESULT] Actor={kvp.Key}, Nick={kvp.Value.nickName} => {msg}");
        }

        // [ADDED] 최종 순위 랭킹 팝업 표시
        photonView.RPC(nameof(RpcShowRankingPopup), RpcTarget.All);

        // [ADDED] 예: 3초 후 룸에서 나가도록
        StartCoroutine(CoFinishGame());
    }

    // [ADDED] 모든 클라이언트에서 랭킹 팝업을 띄운다
    [PunRPC]
    private void RpcShowRankingPopup()
    {
        // UI_RankPopUp을 Resolve하고, 데이터를 세팅한다.
        UI_RankPopUp rankPopup = UI_Manager.instance.Resolve<UI_RankPopUp>();
        // 랭킹 표시 위해 Dictionary 복사 (정렬된 결과를 넘기도록)
        // 정렬 로직은 Finalize 시점에 했으므로, 여기서는 Sort해서 넘기기만...
        var sorted = _playerResults
            .OrderByDescending(p => p.Value.finished)  // 완주 여부 우선
            .ThenBy(p => p.Value.finishTime)          // 완주시간 오름차
            .Select(kv => kv.Value)
            .ToList();

        rankPopup.SetResults(sorted);
        rankPopup.Show();
    }

    IEnumerator CoFinishGame()
    {
        yield return new WaitForSeconds(3f);
        PhotonNetwork.LeaveRoom();
    }

    // [ADDED] 중도퇴장 처리
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        if (!_gameFinished && PhotonNetwork.IsMasterClient)
        {
            if (_playerResults.TryGetValue(otherPlayer.ActorNumber, out PlayerResult pr))
            {
                pr.leftEarly = true;
                _playerResults[otherPlayer.ActorNumber] = pr;
            }

            if (PhotonNetwork.CurrentRoom.PlayerCount <= 1)
            {
                FinalizeGameResults();
            }
        }
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        SceneManager.LoadScene("Lobby");
    }

    //==================================================
    // 아래는 규민님 original 생존모드 / 레이스모드 로직들
    //==================================================

    // 탈락 규칙 설정 (생존 모드 전용)
    private void UpdateEliminationRules()
    {
        if (currentGameMode == GameMode.Survival)
        {
            playersRequiredToEliminate = activePlayers.Count >= 5 ? 2 : 1;
        }
        Debug.Log($"Players remaining: {activePlayers.Count}, Players to eliminate: {playersRequiredToEliminate}");
    }

    [PunRPC]
    public void EliminatePlayer(string playerID)
    {
        GameObject playerToEliminate = activePlayers.Find(p => p.GetPhotonView().Owner.UserId == playerID);
        if (playerToEliminate != null)
        {
            activePlayers.Remove(playerToEliminate);
            playerToEliminate.SetActive(false);
            Debug.Log($"Player {playerID} eliminated!");
            CheckGameState();
        }
    }

    public void ForceEliminatePlayer(GameObject player)
    {
        if (player != null)
        {
            string playerID = player.GetPhotonView().Owner.UserId;
            Debug.Log($"Forcing elimination for Player ID: {playerID}");
            photonView.RPC("EliminatePlayer", RpcTarget.All, playerID);
        }
        else
        {
            Debug.LogError("ForceEliminatePlayer called with null player!");
        }
    }

    public void PlayerFinishedRace(GameObject player)
    {
        if (!raceFinishers.Contains(player))
        {
            raceFinishers.Add(player);
            Debug.Log($"Player {player.GetPhotonView().Owner.NickName} finished the race!");

            // [ADDED] PlayerResult에 완주/시간 기록
            if (PhotonNetwork.IsMasterClient)
            {
                int actorNum = player.GetPhotonView().Owner.ActorNumber;
                if (_playerResults.ContainsKey(actorNum))
                {
                    var pr = _playerResults[actorNum];
                    pr.finished = true;
                    pr.finishTime = PhotonNetwork.Time; // 완주 시점
                    _playerResults[actorNum] = pr;
                }
            }
        }
        CheckGameState();
    }

    private void CheckGameState()
    {
        if (currentGameMode == GameMode.Survival)
        {
            if (activePlayers.Count == 1)
                DeclareWinner(activePlayers[0]);
            else
                UpdateEliminationRules();
        }
        else if (currentGameMode == GameMode.Race)
        {
            if (raceFinishers.Count > 0 && raceFinishers.Count == activePlayers.Count - playersRequiredToEliminate)
                DeclareWinner(raceFinishers[0]);
        }
    }

    [PunRPC]
    private void DeclareWinner(GameObject winner)
    {
        Debug.Log($"Game Over! Winner: {winner.GetPhotonView().Owner.NickName}");
        // UI 관련 코드 추가 가능
    }

    private IEnumerator RoundTimer()
    {
        yield return new WaitForSeconds(roundTime);
        HandleTimeout();
    }

    private void HandleTimeout()
    {
        if (currentGameMode == GameMode.Survival)
        {
            Debug.Log("Time is up! Remaining players survive.");
        }
        else if (currentGameMode == GameMode.Race)
        {
            List<GameObject> playersToEliminate = new List<GameObject>(activePlayers);
            foreach (var finisher in raceFinishers)
            {
                playersToEliminate.Remove(finisher);
            }
            foreach (var player in playersToEliminate)
            {
                ForceEliminatePlayer(player);
            }
        }
    }

    public void StartNewRound()
    {
        raceFinishers.Clear();
        //SpawnPlayers(); 
        UpdateEliminationRules();
        StartCoroutine(RoundTimer());
    }
}


/*// --- GamePlayManager.cs ---
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using ExitGames.Client.Photon;

// 게임 모드 (생존 & 레이스)
public enum GameMode
{
    Survival,
    Race
}

public class GamePlayManager : MonoBehaviourPunCallbacks
{
    // [ADDED] 맵 제한시간 등 기존 로직 생략
    [SerializeField] private float _timeLimit = 300f;
    private float _startTime = 0f;
    private bool _gameFinished = false;

    // [ADDED] 레이스 결과 저장용 구조체
    private Dictionary<int, PlayerResult> _playerResults = new Dictionary<int, PlayerResult>();

    private struct PlayerResult
    {
        public string nickName;
        public bool finished;
        public double finishTime;
        public int rank;
        public bool leftEarly;
    }

    // [ADDED] 로딩된 맵 인스턴스를 참조
    private GameObject _mapInstance;

    //GameManager(규민)
    public GameMode currentGameMode; // 현재 게임 모드
    public List<string> survivalMaps; // 생존 모드 맵 목록
    public List<string> raceMaps; // 레이스 모드 맵 목록
    private List<GameObject> activePlayers = new List<GameObject>(); // 현재 생존 플레이어 목록
    private int playersRequiredToEliminate; // 이번 라운드에서 탈락할 플레이어 수
    private string currentMap; // 현재 선택된 맵
    private List<GameObject> raceFinishers = new List<GameObject>(); // 레이스에서 결승선을 통과한 플레이어 목록
    public float roundTime = 60f; // 라운드 시간 제한 (초)

    public GameObject soundManagerPrefab; // SoundManager 프리팹

    private void Awake()
    {
        // 필요 시 싱글톤
    }

    void Start()
    {
        _startTime = Time.time;

        // [ADDED] 방 참가한 모든 플레이어 Result 초기화
        foreach (var p in PhotonNetwork.PlayerList)
        {
            _playerResults[p.ActorNumber] = new PlayerResult
            {
                nickName = p.NickName,
                finished = false,
                finishTime = 0.0,
                rank = 0,
                leftEarly = false
            };
        }

        // [ADDED] 방에서 선택한 맵 prefab 스폰
        LoadSelectedMap();

        // [ADDED] 각 플레이어 캐릭터 스폰 (본인만 PhotonNetwork.Instantiate)
       // SpawnPlayerCharacter();
    }

    private void Update()
    {
        // 마스터가 제한 시간 체크
        if (!PhotonNetwork.IsMasterClient) return;
        if (_gameFinished) return;

        float elapsed = Time.time - _startTime;
        float remain = _timeLimit - elapsed;
        if (remain <= 0f)
        {
            Debug.Log("[GamePlayManager] Time Limit Reached => Finalize!");
            FinalizeGameResults();
        }
    }

    // [ADDED] Room CustomProperty "SelectedMap" 읽어서 PhotonNetwork.InstantiateRoomObject
    private void LoadSelectedMap()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("SelectedMap", out object mapObj))
        {
            Debug.Log($"[게임맵 호출 1 ]: {mapObj}");
            string selectedMap = mapObj as string;
            if (string.IsNullOrEmpty(selectedMap))
            {
                Debug.Log($"[게임맵 호출 Forest ]: {mapObj}");
                selectedMap = "Forest"; // fallback

            }

            if (PhotonNetwork.IsMasterClient)
            {
                string mapPath = $"Maps/{selectedMap}";
                GameObject mapPrefab = Resources.Load<GameObject>(mapPath);
                if (mapPrefab != null)
                {
                    _mapInstance = PhotonNetwork.InstantiateRoomObject(mapPath, Vector3.zero, Quaternion.identity);
                    Debug.Log($"[게임맵 찾았어 ] MapName : {selectedMap}");
                }
                else
                {
                    Debug.LogError($"[게임맵 null ]: {mapPath}");
                }
            }
        }
        else
        {
            if (PhotonNetwork.IsMasterClient)
            {
                string defaultMap = "Forest";
                GameObject mapPrefab = Resources.Load<GameObject>($"Maps/{defaultMap}");
                if (mapPrefab != null)
                {
                    _mapInstance = PhotonNetwork.InstantiateRoomObject($"Maps/{defaultMap}", Vector3.zero, Quaternion.identity);
                    Debug.Log($"[[게임맵 호출 마스터변경시 ]] Instantiate Default Map: {defaultMap}");
                }
                else
                {
                    Debug.LogError("게임맵 없음 마스터변경시");
                }
            }
        }
    }

   
    private void FinalizeGameResults()
    {
        if (_gameFinished) return;
        _gameFinished = true;

        // 등수 매기기
        var finishedList = _playerResults.Where(k => k.Value.finished)
                                         .OrderBy(k => k.Value.finishTime)
                                         .ToList();
        int rank = 1;
        foreach (var kvp in finishedList)
        {
            var pr = kvp.Value;
            pr.rank = rank++;
            _playerResults[kvp.Key] = pr;
        }

        // 로그 출력 or UI
        foreach (var kvp in _playerResults)
        {
            string msg = kvp.Value.leftEarly ? "(DNF-EarlyLeft)"
                      : (!kvp.Value.finished ? "(DNF)" : $"Rank={kvp.Value.rank}, Time={kvp.Value.finishTime:F2}");
            Debug.Log($"[RESULT] Actor={kvp.Key}, Nick={kvp.Value.nickName} => {msg}");
        }

        StartCoroutine(CoFinishGame());
    }

    IEnumerator CoFinishGame()
    {
        yield return new WaitForSeconds(3f);
        PhotonNetwork.LeaveRoom();
    }

    // [ADDED] 중도퇴장 처리
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        if (!_gameFinished && PhotonNetwork.IsMasterClient)
        {
            if (_playerResults.TryGetValue(otherPlayer.ActorNumber, out PlayerResult pr))
            {
                pr.leftEarly = true;
                _playerResults[otherPlayer.ActorNumber] = pr;
            }

            // 남은 인원 <=1이면 종료
            if (PhotonNetwork.CurrentRoom.PlayerCount <= 1)
            {
                FinalizeGameResults();
            }
        }
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        SceneManager.LoadScene("Lobby");
    }


    //규민
    // 탈락 규칙 설정 (생존 모드 전용)
    private void UpdateEliminationRules()
    {
        if (currentGameMode == GameMode.Survival)
        {
            playersRequiredToEliminate = activePlayers.Count >= 5 ? 2 : 1;
        }
        Debug.Log($"Players remaining: {activePlayers.Count}, Players to eliminate: {playersRequiredToEliminate}");
    }

    // 플레이어 탈락 처리 (생존 모드 & 레이스 모드 공통)
    [PunRPC]
    public void EliminatePlayer(string playerID)
    {
        GameObject playerToEliminate = activePlayers.Find(p => p.GetPhotonView().Owner.UserId == playerID);
        if (playerToEliminate != null)
        {
            activePlayers.Remove(playerToEliminate);
            playerToEliminate.SetActive(false);
            Debug.Log($"Player {playerID} eliminated!");
            CheckGameState();
        }
    }

    // 강제로 플레이어 탈락 (예: 시간 초과 등)
    public void ForceEliminatePlayer(GameObject player)
    {
        if (player != null)
        {
            string playerID = player.GetPhotonView().Owner.UserId;
            Debug.Log($"Forcing elimination for Player ID: {playerID}");
            photonView.RPC("EliminatePlayer", RpcTarget.All, playerID);
        }
        else
        {
            Debug.LogError("ForceEliminatePlayer called with null player!");
        }
    }

    // 레이스 모드에서 플레이어가 결승선에 도착했을 때
    public void PlayerFinishedRace(GameObject player)
    {
        if (!raceFinishers.Contains(player))
        {
            raceFinishers.Add(player);
            Debug.Log($"Player {player.GetPhotonView().Owner.NickName} finished the race!");
        }
        CheckGameState();
    }

    // 게임 상태 확인 및 승자 결정
    private void CheckGameState()
    {
        if (currentGameMode == GameMode.Survival)
        {
            if (activePlayers.Count == 1)
                DeclareWinner(activePlayers[0]);
            else
                UpdateEliminationRules();
        }
        else if (currentGameMode == GameMode.Race)
        {
            if (raceFinishers.Count > 0 && raceFinishers.Count == activePlayers.Count - playersRequiredToEliminate)
                DeclareWinner(raceFinishers[0]);
        }
    }

    // 승자 선언 (모든 클라이언트 동기화)
    [PunRPC]
    private void DeclareWinner(GameObject winner)
    {
        Debug.Log($"Game Over! Winner: {winner.GetPhotonView().Owner.NickName}");
        // UI 관련 코드 추가 가능
    }

    // 라운드 타이머 시작
    private IEnumerator RoundTimer()
    {
        yield return new WaitForSeconds(roundTime);
        HandleTimeout();
    }

    // 시간 초과 처리
    private void HandleTimeout()
    {
        if (currentGameMode == GameMode.Survival)
        {
            // 생존 모드에서는 시간 초과 시 남은 플레이어가 전원 생존
            Debug.Log("Time is up! Remaining players survive.");
        }
        else if (currentGameMode == GameMode.Race)
        {
            // 레이스 모드에서는 결승선에 도착하지 못한 플레이어를 탈락시킴
            List<GameObject> playersToEliminate = new List<GameObject>(activePlayers);
            foreach (var finisher in raceFinishers)
            {
                playersToEliminate.Remove(finisher);
            }
            foreach (var player in playersToEliminate)
            {
                ForceEliminatePlayer(player);
            }
        }
    }

    // 새로운 라운드 시작
    public void StartNewRound()
    {
        raceFinishers.Clear(); // 레이스 모드 플레이어 초기화
        //SpawnPlayers(); // 플레이어 다시 생성
        UpdateEliminationRules(); // 탈락 규칙 갱신
        StartCoroutine(RoundTimer()); // 새로운 라운드 타이머 시작
    }
}
*/