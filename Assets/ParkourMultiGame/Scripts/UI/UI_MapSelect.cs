// --- UI_MapSelect.cs ---
using Photon.Pun;
using Photon.Realtime;
using Practices.UGUI_Management.UI;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ExitGames.Client.Photon; // [ADDED] for Hashtable
using UnityEngine.SceneManagement;

// [CHANGED] IInRoomCallbacks -> 맵 변경사항을 즉시 반영하기 위해
public class UI_MapSelect : UI_Popup, IInRoomCallbacks
{
    [SerializeField] private TMP_Dropdown _mapDropdown;
    [SerializeField] private Image _mapPreviewImage;

    // [ADDED] 미리 정의된 맵 이름, 미리보기 Sprite
    private List<(string mapName, Sprite mapSprite)> _maps = new List<(string, Sprite)>();

    // [CHANGED] 현재 선택된 맵 이름 (Room CustomProperty "SelectedMap"와 동기화)
    private string _selectedMap = "";

    private bool _initialized = false; // [ADDED] 중복 초기화 방지 플래그

    protected override void Start()
    {
        base.Start();

        // [ADDED] 이 팝업이 뜰 때, Photon 콜백 등록
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDestroy()
    {
        // [ADDED] 파괴 시 콜백 해제
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public override void Show()
    {
        base.Show();

        // [ADDED] 필요한 초기화를 한 번만 수행
        if (!_initialized)
        {
            InitializeMapData();
            InitializeDropdown();
            _initialized = true;
        }

        // [ADDED] 방장 여부에 따라 드롭다운/Interactable 상태 조절
        _mapDropdown.interactable = PhotonNetwork.LocalPlayer.IsMasterClient;

        // [ADDED] 이미 Room에 "SelectedMap"이 있다면, 현재 선택 맵 표시
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("SelectedMap", out object mapObj))
        {
            _selectedMap = mapObj as string;
            SetDropdownToMap(_selectedMap);
        }
        else
        {
            // [CHANGED] 만약 없는 경우, 방장이면 기본 맵을 설정
            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                SetDefaultMap();
            }
        }
    }

    // [ADDED] 맵 목록 초기화(이름 + 스프라이트)
    private void InitializeMapData()
    {
        // 실제로는 Resources/Maps/ 폴더 구조에 맞춰 Sprite를 로드
        _maps.Clear();
        _maps.Add(("Forest", Resources.Load<Sprite>("Maps/Forest")));
        _maps.Add(("Desert", Resources.Load<Sprite>("Maps/Desert")));
        _maps.Add(("IceLand", Resources.Load<Sprite>("Maps/IceLand")));
        _maps.Add(("DoorDash", Resources.Load<Sprite>("Maps/DoorDash")));
        _maps.Add(("JumpClub", Resources.Load<Sprite>("Maps/JumpClub")));
        _maps.Add(("ThinIce", Resources.Load<Sprite>("Maps/ThinIce")));
        _maps.Add(("Tiptoe", Resources.Load<Sprite>("Maps/Tiptoe")));
        //_maps.Add(("WallParty", Resources.Load<Sprite>("Maps/WallParty")));

        // 필요에 따라 추가

        Debug.Log($"[UI_MapSelect] Map data loaded: {_maps.Count}개");
    }

    // [ADDED] 드롭다운 설정
    private void InitializeDropdown()
    {
        // 맵 이름들만 추출
        List<string> mapNames = new List<string>();
        foreach (var (mapName, _) in _maps)
        {
            mapNames.Add(mapName);
        }

        _mapDropdown.ClearOptions();
        _mapDropdown.AddOptions(mapNames);

        // 드롭다운 변경 시 -> 방장만 수정 가능
        _mapDropdown.onValueChanged.AddListener(index =>
        {
            if (!PhotonNetwork.LocalPlayer.IsMasterClient)
                return; // 방장이 아니라면 무시

            _selectedMap = _maps[index].mapName;
            _mapPreviewImage.sprite = _maps[index].mapSprite;

            // [ADDED] Room CustomProperties 동기화
            var props = new ExitGames.Client.Photon.Hashtable()
            {
                { "SelectedMap", _selectedMap }
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);

            // 맵 선택 후 맵 선택 창을 닫음
            //Hide();
        });
    }

    // [ADDED] 기본 맵을 드롭다운 0번으로 (ex: Forest) 선택해두기
    private void SetDefaultMap()
    {
        if (_maps.Count == 0) return;

        // 첫 맵을 기본으로
        _mapDropdown.value = 0;
        _selectedMap = _maps[0].mapName;
        _mapPreviewImage.sprite = _maps[0].mapSprite;

        // [ADDED] Room CustomProperties에도 반영
        var props = new ExitGames.Client.Photon.Hashtable()
        {
            { "SelectedMap", _selectedMap }
        };
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);

        Debug.Log($"[UI_MapSelect] Default map: {_selectedMap}");
    }

    // [ADDED] "SelectedMap" 문자열에 맞춰, 드롭다운/미리보기 갱신
    private void SetDropdownToMap(string mapName)
    {
        int idx = _maps.FindIndex(x => x.mapName == mapName);
        if (idx < 0) return;

        _mapDropdown.value = idx;
        _mapPreviewImage.sprite = _maps[idx].mapSprite;
    }

    // ----------------------------------
    // IInRoomCallbacks 구현
    // ----------------------------------
    public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        // [ADDED] 방장이 변경한 "SelectedMap"을 다른 클라이언트도 반영
        if (propertiesThatChanged.ContainsKey("SelectedMap"))
        {
            string newMap = (string)propertiesThatChanged["SelectedMap"];
            Debug.Log($"[UI_MapSelect] OnRoomPropertiesUpdate => SelectedMap='{newMap}'");

            // 본인 로컬 UI 갱신
            _selectedMap = newMap;
            SetDropdownToMap(_selectedMap);
        }
    }

    public void OnPlayerEnteredRoom(Player newPlayer) { }
    public void OnPlayerLeftRoom(Player otherPlayer) { }
    public void OnMasterClientSwitched(Player newMasterClient)
    {
        // [ADDED] 방장이 바뀌면 드롭다운 Interactable 상태 재설정
        _mapDropdown.interactable = PhotonNetwork.LocalPlayer.IsMasterClient;
    }
    public void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps) { }
}
