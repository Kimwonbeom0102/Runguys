using UnityEngine;
using Practices.UGUI_Management.Utilities;
using TMPro;
using UnityEngine.UI;

namespace Practices.PhotonPunClient.UI
{
    public class RoomPlayerInfoSlot : ComponentResolvingBehaviour
    {
        public int actorNumber { get; set; }

        public bool isReady
        {
            get => _isReadyValue;
            set
            {
                _isReadyValue = value;
                _isReady.gameObject.SetActive(value);
            }
        }

        public string playerName
        {
            get => _playerNameValue;
            set
            {
                _playerNameValue = value;
                _playerName.text = value;
            }
        }

        public bool isMasterClient
        {
            get => _isMasterClientValue;
            set
            {
                _isMasterClientValue = value;
                _isMasterClient.gameObject.SetActive(value);
            }
        }

        //[ADDED] Show selected character icon
        public void SetCharacterImage(string characterName)
        {
            if (_characterImage == null) return;
            // Example: "CharacterImages/Archer"
            Sprite sp = Resources.Load<Sprite>($"CharacterImages/{characterName}");
            if (sp != null)
            {
                _characterImage.sprite = sp;
            }
        }

        bool _isReadyValue;
        string _playerNameValue;
        bool _isMasterClientValue;

        [Resolve] TMP_Text _isReady;
        [Resolve] TMP_Text _playerName;
        [Resolve] Image _isMasterClient;

        [Resolve] Image _characterImage; // [ADDED] "Image - CharacterImage" in child
    }
}


/*using UnityEngine;
using Practices.UGUI_Management.Utilities;
using TMPro;
using UnityEngine.UI;

namespace Practices.PhotonPunClient.UI
{
    public class RoomPlayerInfoSlot : ComponentResolvingBehaviour
    {
        public int actorNumber { get; set; }

        public bool isReady
        {
            get => _isReadyValue;
            set
            {
                _isReadyValue = value;
                _isReady.gameObject.SetActive(value);
            }
        }

        public string playerName
        {
            get => _playerNameValue;
            set
            {
                _playerName.text = value;
            }
        }

        public bool isMasterClient
        {
            get => _isMasterClientValue;
            set
            {
                _isMasterClientValue = value;
                _isMasterClient.gameObject.SetActive(value);
            }
        }


        bool _isReadyValue;
        string _playerNameValue;
        bool _isMasterClientValue;
        [Resolve] TMP_Text _isReady;
        [Resolve] TMP_Text _playerName;
        [Resolve] Image _isMasterClient;
    }
}*/