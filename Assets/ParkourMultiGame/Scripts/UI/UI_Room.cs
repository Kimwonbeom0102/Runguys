// --- UI_Room.cs ---
using TMPro;
using UnityEngine.UI;
using Practices.UGUI_Management.Utilities;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Practices.UGUI_Management.UI;
using Practices.PhotonPunClient.UI;
using Practices.PhotonPunClient;

public class UI_Room : UI_Screen, IInRoomCallbacks
{
    [Resolve] private RoomPlayerInfoSlot _roomPlayerInfoSlot;
    [Resolve] private RectTransform _roomPlayerInfoPanel;
    [Resolve] private Button _startGame;
    [Resolve] private Button _gameReady;
    [Resolve] private Button _leftRoom;

    [Resolve] private Button _transferMasterButton;
    [Resolve] private TMP_Dropdown _playerListDropdown;
    [Resolve] private TMP_Text _playerCountText;

    [Resolve] private Button _openMapSelectButton;

    // ������ �ʵ� - [Resolve] ����
    private Dictionary<int, (Player player, RoomPlayerInfoSlot slot)> _roomPlayerInfoPairs;

    protected override void Awake()
    {
        base.Awake();
        _roomPlayerInfoPairs = new Dictionary<int, (Player, RoomPlayerInfoSlot)>(16);
    }

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    protected override void Start()
    {
        base.Start();

        // [ADDED] Null üũ �α�
        if (_startGame == null)
            Debug.LogError("[UI_Room] _startGame is null!");
        if (_transferMasterButton == null)
            Debug.LogError("[UI_Room] _transferMasterButton is null!");
        if (_gameReady == null)
            Debug.LogError("[UI_Room] _gameReady is null!");
        if (_openMapSelectButton == null)
            Debug.LogError("[UI_Room] _openMapSelectButton is null!");

        _roomPlayerInfoSlot.gameObject.SetActive(false);

        if (_startGame != null)
            _startGame.onClick.AddListener(OnClickStartGame);
        else
            Debug.LogError("[UI_Room] _startGame is not assigned.");

        if (_gameReady != null)
            _gameReady.onClick.AddListener(OnClickReady);
        else
            Debug.LogError("[UI_Room] _gameReady is not assigned.");

        if (_leftRoom != null)
            _leftRoom.onClick.AddListener(OnClickLeaveRoom);
        else
            Debug.LogError("[UI_Room] _leftRoom is not assigned.");

        if (_transferMasterButton != null)
            _transferMasterButton.onClick.AddListener(OnClickTransferMaster);
        else
            Debug.LogError("[UI_Room] _transferMasterButton is not assigned.");

        // [ADDED] "�� ����" ��ư -> �����͸� �˾� ǥ��
        if (_openMapSelectButton != null)
        {
            _openMapSelectButton.onClick.AddListener(() =>
            {
                if (!PhotonNetwork.LocalPlayer.IsMasterClient)
                {
                    Debug.LogWarning("[UI_Room] �� ������ ���常 �����մϴ�.");
                    return;
                }

                // UI_MapSelect�� ��Ȱ��ȭ�Ǿ� �ִٸ� �ٽ� Ȱ��ȭ�ϱ�
                UI_MapSelect mapSelect = UI_Manager.instance.Resolve<UI_MapSelect>();
                if (mapSelect != null)
                {
                    if (!mapSelect.gameObject.activeSelf) // ��Ȱ��ȭ ���¸� �ٽ� Ȱ��ȭ
                    {
                        mapSelect.gameObject.SetActive(true);
                        //Debug.Log("[UI_Room] UI_MapSelect �ٽ� Ȱ��ȭ��!");
                        //mapSelect.Show(); // UI ���� ����
                    }

                    mapSelect.Show(); // UI ���� ����
                }
                else
                {
                    Debug.LogError("[UI_Room] UI_MapSelect�� ã�� �� ����!");
                }
            });
        }
        else
        {
            Debug.LogError("[UI_Room] _openMapSelectButton is not assigned.");
        }

        TogglePlayerButtons(PhotonNetwork.LocalPlayer);

    }

    public override void Show()
    {
        base.Show();

        // ���� UI ���� ����
        foreach (var kvp in _roomPlayerInfoPairs)
        {
            Destroy(kvp.Value.slot.gameObject);
        }
        _roomPlayerInfoPairs.Clear();

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            CreatePlayerSlot(player);
        }

        UpdatePlayerListDropdown();
        TogglePlayerButtons(PhotonNetwork.LocalPlayer);
        UpdatePlayerCountUI();
        UpdateStartGameButtonInteractable(); // �ʱ�ȭ �� interactable ���� ����
    }

    private void CreatePlayerSlot(Player player)
    {
        RoomPlayerInfoSlot slot = Instantiate(_roomPlayerInfoSlot, _roomPlayerInfoPanel);
        slot.gameObject.SetActive(true);
        slot.actorNumber = player.ActorNumber;
        slot.isMasterClient = player.IsMasterClient;

        if (player.CustomProperties.TryGetValue(PlayerInRoomPropertyKey.IS_READY, out object val))
            slot.isReady = (bool)val;
        else
            slot.isReady = false;

        // ĳ���� ����/�г��� ��
        slot.playerName = player.NickName;
        if (player.CustomProperties.TryGetValue("SelectedCharacter", out object charObj))
        {
            slot.SetCharacterImage((string)charObj);
        }

        _roomPlayerInfoPairs.Add(player.ActorNumber, (player, slot));
    }

    private void TogglePlayerButtons(Player localPlayer)
    {
        bool isMaster = localPlayer.IsMasterClient;
        Debug.Log($"isMaster = {isMaster}");
        _startGame.gameObject.SetActive(isMaster);
        _transferMasterButton.gameObject.SetActive(isMaster);

        // �񸶽��ʹ� Ready ��ư
        _gameReady.gameObject.SetActive(!isMaster);
    }

    private void UpdateStartGameButtonInteractable()
    {
        if (_startGame != null && _startGame.gameObject.activeInHierarchy)
        {
            _startGame.interactable = (PhotonNetwork.CurrentRoom.PlayerCount >= 2);
            Debug.Log($"_startGame.interactable set to {PhotonNetwork.CurrentRoom.PlayerCount >= 2}");
        }
        else
        {
            Debug.LogWarning("[UI_Room] _startGame is not active. Cannot set interactable.");
        }
    }

    private void UpdatePlayerListDropdown()
    {
        _playerListDropdown.ClearOptions();
        var names = _roomPlayerInfoPairs.Values.Select(v => v.player.NickName).ToList();
        _playerListDropdown.AddOptions(names);
        _playerListDropdown.value = 0;
    }

    private void UpdatePlayerCountUI()
    {
        _playerCountText.text = $"{PhotonNetwork.CurrentRoom.PlayerCount} / {PhotonNetwork.CurrentRoom.MaxPlayers}";
    }

    // ------------------
    // Button Handlers
    // ------------------
    private void OnClickStartGame()
    {
        if (PhotonNetwork.LocalPlayer.IsMasterClient &&
            PhotonNetwork.CurrentRoom.PlayerCount >= 2)
        {
            // [ADDED] �� ���� �˾��� ���� �ִٸ� �ݱ�
            UI_MapSelect mapSel = UI_Manager.instance.Resolve<UI_MapSelect>();
            if (mapSel.gameObject.activeSelf)   //true �̸�
            {
                
                mapSel.Hide();
            }

            // [CHANGED] �׳� �ٷ� �� �ε�
            PhotonNetwork.LoadLevel("GamePlay");
        }
        else
        {
            Debug.LogWarning("Not enough players or not master.");
        }
    }

    private void OnClickReady()
    {
        var player = PhotonNetwork.LocalPlayer;
        bool isReady = false;
        if (player.CustomProperties.TryGetValue(PlayerInRoomPropertyKey.IS_READY, out object val))
            isReady = (bool)val;

        player.SetCustomProperties(new Hashtable
        {
            { PlayerInRoomPropertyKey.IS_READY, !isReady }
        });
    }

    private void OnClickLeaveRoom()
    {
        // UI_MapSelect ��Ȱ��ȭ �߰�
        UI_MapSelect mapSelect = UI_Manager.instance.Resolve<UI_MapSelect>();
        if (mapSelect != null && mapSelect.gameObject.activeSelf)
        {
           mapSelect.gameObject.SetActive(false); // UI ��Ȱ��ȭ
           // Debug.Log("[OnClickLeaveRoom] UI_MapSelect ��Ȱ��ȭ��!"); // ����� �α�
            //mapSelect.Hide(); // UI ��Ȱ��ȭ
        }
        else
        {
            Debug.Log("[OnClickLeaveRoom] UI_MapSelect�� �̹� ��Ȱ��ȭ ���°ų� ����.");
        }

        PhotonNetwork.LeaveRoom(); // ���� �ڵ� ����
    }


    private void OnClickTransferMaster()
    {
        if (!PhotonNetwork.LocalPlayer.IsMasterClient) return;

        int idx = _playerListDropdown.value;
        var players = _roomPlayerInfoPairs.Values.Select(v => v.player).ToList();
        if (idx >= 0 && idx < players.Count)
        {
            Player target = players[idx];
            PhotonNetwork.SetMasterClient(target);
        }
    }

    // ------------------
    // IInRoomCallbacks
    // ------------------
    public void OnMasterClientSwitched(Player newMasterClient)
    {
        // �� ���� -> false
        var oldMaster = _roomPlayerInfoPairs.Values.FirstOrDefault(v => v.slot.isMasterClient);
        if (oldMaster.player != null)
            oldMaster.slot.isMasterClient = false;

        // �� ���� -> true
        if (_roomPlayerInfoPairs.TryGetValue(newMasterClient.ActorNumber, out var pair))
            pair.slot.isMasterClient = true;

        TogglePlayerButtons(PhotonNetwork.LocalPlayer);
        UpdateStartGameButtonInteractable();
    }

    public void OnPlayerEnteredRoom(Player newPlayer)
    {
        CreatePlayerSlot(newPlayer);
        UpdatePlayerListDropdown();
        UpdatePlayerCountUI();
        TogglePlayerButtons(PhotonNetwork.LocalPlayer);
        UpdateStartGameButtonInteractable();
    }

    public void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (_roomPlayerInfoPairs.TryGetValue(otherPlayer.ActorNumber, out var pair))
        {
            Destroy(pair.slot.gameObject);
            _roomPlayerInfoPairs.Remove(otherPlayer.ActorNumber);
        }
        UpdatePlayerListDropdown();
        UpdatePlayerCountUI();
        TogglePlayerButtons(PhotonNetwork.LocalPlayer);
        UpdateStartGameButtonInteractable();
    }

    public void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (_roomPlayerInfoPairs.TryGetValue(targetPlayer.ActorNumber, out var pair))
        {
            if (changedProps.TryGetValue(PlayerInRoomPropertyKey.IS_READY, out object readyObj))
                pair.slot.isReady = (bool)readyObj;

            if (changedProps.TryGetValue("SelectedCharacter", out object charObj))
                pair.slot.SetCharacterImage((string)charObj);
        }

        UpdateStartGameButtonInteractable();
    }

    public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        // ���⼭ "SelectedMap" ���� ���� ��, ���� ǥ�ø� ���Ѵٸ� ���� �߰� ����
    }
}
