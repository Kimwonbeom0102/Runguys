// [ADDED FILE] UI_CharacterSelect.cs
using Photon.Pun;
using Photon.Realtime;
using Practices.UGUI_Management.UI;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Practices.PhotonPunClient.UI
{
    public class UI_CharacterSelect : UI_Popup // [ADDED] UI_Popup 상속(혹은 UI_Base)
    {
        [SerializeField] private Button[] _characterButtons;
        // 인스펙터에서 8개 버튼 등록 (ArcherBtn, ClericBtn, ...)

        // [CHANGED] 
        [SerializeField] private Image[] _buttonImages; // [ADDED] 혹은 버튼 컴포넌트에서 .image를 가져올 수도 있음  // 8개의 캐릭터 버튼이 각각 어떤 Sprite를 가지고 있는지 확인
        public event Action<Sprite> onCharacterSelected; // [ADDED] 선택된 캐릭터 이미지를 외부에 알림

        private readonly string[] _characterNames =
        {
            "Arrowbot",
            "Bigbot",
            "Crabbot",
            "Greenbot",
            "Heartbot",
            "Orangebot",
            "Redbot",
            "Smallbot"
        };

        protected override void Start()
        {
            base.Start();

            for (int i = 0; i < _characterButtons.Length; i++)
            {
                int idx = i; // capture
                _characterButtons[i].onClick.AddListener(() => OnClickCharacter(idx));
            }
        }

        private void OnClickCharacter(int idx)
        {
            if (idx < 0 || idx >= _characterNames.Length) return;

            string selected = _characterNames[idx];

            // [ADDED] Photon CustomProperties 저장
            var props = new ExitGames.Client.Photon.Hashtable
            {
                { "SelectedCharacter", selected }
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);

            Debug.Log($"[UI_CharacterSelect] Selected Character = {selected}");

            // [ADDED] 해당 캐릭터 Sprite를 가져온다.
            Sprite selectedSprite = _buttonImages[idx].sprite;

            // 팝업 외부에서 이 이벤트를 구독하면, 이미지 표시 가능
            onCharacterSelected?.Invoke(selectedSprite);

            // [CHANGED] 팝업 닫기
            Hide();
        }
    }
}
