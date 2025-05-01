// --- UI_Lobby.cs ---
using Photon.Pun;
using Photon.Realtime;
using Practices.UGUI_Management.UI;
using Practices.UGUI_Management.Utilities;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using ExitGames.Client.Photon;
using System.Linq;

namespace Practices.PhotonPunClient.UI
{
    public class UI_Lobby : UI_Screen, ILobbyCallbacks, IMatchmakingCallbacks
    {
        [Resolve] RectTransform _roomListSlotContent;
        [Resolve] RoomListSlot _roomListSlot;
        [Resolve] Button _createRoom;
        [Resolve] Button _joinRoom;
        private List<RoomListSlot> _roomListSlots = new List<RoomListSlot>(10);
        private List<RoomInfo> _roomInfosCached = new List<RoomInfo>(10);
        private int _roomIdSelected = -1;

        [Resolve] private Button _openCharacterSelectButton;
        [Resolve] private Image _selectedCharacterImage;

        private const string DEFAULT_CHARACTER = "Arrowbot"; //[ADDED] fallback

        protected override void Start()
        {
            base.Start();

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            _roomListSlot.gameObject.SetActive(false);
            playerInputActions.UI.Click.performed += OnClick;

            // [CHANGED] 기존 로직 + 캐릭터 선택 팝업 닫기 로직 추가
            _createRoom.onClick.AddListener(() =>
            {
                // [ADDED] 만약 캐릭터 선택 팝업(UI_CharacterSelect)이 열려있다면 닫아준다.
                UI_CharacterSelect charSelect = UI_Manager.instance.Resolve<UI_CharacterSelect>();
                if (charSelect.gameObject.activeSelf) // 팝업이 열려있는 상태
                {
                    charSelect.Hide();
                }

                UI_CreateRoomOption createRoomOption = UI_Manager.instance.Resolve<UI_CreateRoomOption>();
                createRoomOption.Show();
            });

            _joinRoom.interactable = false;
            _joinRoom.onClick.AddListener(() =>
            {
                // [ADDED] 캐릭터 선택 팝업이 열려 있다면 닫기
                UI_CharacterSelect charSelect = UI_Manager.instance.Resolve<UI_CharacterSelect>();
                if (charSelect.gameObject.activeSelf)
                {
                    charSelect.Hide();
                }

                UI_ConfirmWindow confirmWindow = UI_Manager.instance.Resolve<UI_ConfirmWindow>();
                RoomInfo roomInfo = _roomInfosCached[_roomIdSelected];

                if (!roomInfo.IsOpen)
                {
                    confirmWindow.Show("The room is closed.");
                    return;
                }

                if (roomInfo.PlayerCount >= roomInfo.MaxPlayers)
                {
                    confirmWindow.Show("The room is full.");
                    return;
                }

                PhotonNetwork.JoinRoom(roomInfo.Name);
            });

            // [ADDED] Character Select
            _openCharacterSelectButton.onClick.AddListener(() =>
            {
                UI_CharacterSelect popup = UI_Manager.instance.Resolve<UI_CharacterSelect>();
                popup.onCharacterSelected += OnCharacterSelected;
                popup.Show();
            });

            // [ADDED] 기본 캐릭터 설정 로직
            object existingChar;
            if (!PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("SelectedCharacter", out existingChar))
            {
                PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable
                {
                    { "SelectedCharacter", DEFAULT_CHARACTER }
                });
                _selectedCharacterImage.sprite = Resources.Load<Sprite>($"CharacterImages/{DEFAULT_CHARACTER}");
            }
            else
            {
                string charName = (string)existingChar;
                _selectedCharacterImage.sprite = Resources.Load<Sprite>($"CharacterImages/{charName}");
            }
        }

        private void OnClick(InputAction.CallbackContext context)
        {
            if (TryGraphicRaycast(Mouse.current.position.ReadValue(), out RoomListSlot slot))
            {
                SelectRoom(slot.roomId);
            }
        }

        private void SelectRoom(int roomId)
        {
            RoomInfo roomInfo = _roomInfosCached[roomId];
            if (!roomInfo.IsOpen || roomInfo.PlayerCount >= roomInfo.MaxPlayers)
            {
                _joinRoom.interactable = false;
                return;
            }

            _joinRoom.interactable = true;
            if (_roomIdSelected >= 0)
            {
                _roomListSlots[_roomIdSelected].isSelected = false;
            }
            _roomListSlots[roomId].isSelected = true;
            _roomIdSelected = roomId;
        }

        private void OnCharacterSelected(Sprite sprite)
        {
            _selectedCharacterImage.sprite = sprite;
        }

        private void OnEnable()
        {
            PhotonNetwork.AddCallbackTarget(this);
        }

        private void OnDisable()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }

        public void OnJoinedLobby()
        {
            if (SceneManager.GetActiveScene().name != "Lobby") return;
            UI_ConfirmWindow cw = UI_Manager.instance.Resolve<UI_ConfirmWindow>();
            cw.Show("Joined Lobby!");
        }

        public void OnLeftLobby()
        {
            UI_ConfirmWindow confirmWindow = UI_Manager.instance.Resolve<UI_ConfirmWindow>();
            confirmWindow.Show("Left lobby.");
        }

        public void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics) { }

        public void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            RefreshSlots(roomList);
        }

        void RefreshSlots(List<RoomInfo> roomList)
        {
            RoomListSlot slotSelected = _roomListSlots.Find(slot => slot.roomId == _roomIdSelected);
            string selectedRoomName = slotSelected?.name;
            _joinRoom.interactable = false;
            _roomIdSelected = -1;

            for (int i = 0; i < _roomListSlots.Count; i++)
            {
                Destroy(_roomListSlots[i].gameObject);
            }
            _roomListSlots.Clear();
            _roomInfosCached.Clear();

            for (int i = 0; i < roomList.Count; i++)
            {
                RoomListSlot slot = Instantiate(_roomListSlot, _roomListSlotContent);
                slot.gameObject.SetActive(true);
                slot.roomId = i;
                slot.roomName = roomList[i].Name;
                slot.roomPlayerCount = roomList[i].PlayerCount;
                slot.roomMaxPlayers = roomList[i].MaxPlayers;
                slot.gameObject.SetActive((roomList[i].RemovedFromList == false) && (roomList[i].PlayerCount > 0));
                _roomListSlots.Add(slot);
                _roomInfosCached.Add(roomList[i]);

                if (roomList[i].Name.Equals(selectedRoomName))
                {
                    _roomIdSelected = i;
                    _joinRoom.interactable = true;
                }
            }
        }

        public void OnFriendListUpdate(List<FriendInfo> friendList) { }
        public void OnCreatedRoom() { }
        public void OnCreateRoomFailed(short returnCode, string message)
        {
            UI_ConfirmWindow confirmWindow = UI_Manager.instance.Resolve<UI_ConfirmWindow>();
            confirmWindow.Show(message);
        }

        public void OnJoinedRoom()
        {
            PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable
            {
                { PlayerInRoomPropertyKey.IS_READY, false },
            });

            UI_Manager.instance.Resolve<UI_Room>().Show();
        }

        public void OnJoinRoomFailed(short returnCode, string message)
        {
            UI_ConfirmWindow confirmWindow = UI_Manager.instance.Resolve<UI_ConfirmWindow>();
            confirmWindow.Show(message);
        }

        public void OnJoinRandomFailed(short returnCode, string message)
        {
            UI_ConfirmWindow confirmWindow = UI_Manager.instance.Resolve<UI_ConfirmWindow>();
            confirmWindow.Show(message);
        }

        public void OnLeftRoom()
        {
            Show();
        }
    }
}
