using Practices.UGUI_Management.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Practices.UGUI_Management.UI
{
    public class UI_ConfirmWindow : UI_Popup
    {
        [Resolve] TMP_Text _message;
        [Resolve] Button _confirm;

        public void Show(string message, UnityAction onConfirmed = null)
        {
            // [ADDED] 우선, 이미 파괴되었는지(=this == null) 혹은 Canvas가 살아있는지 검사
            if (this == null)
            {
                Debug.LogWarning("[UI_ConfirmWindow] Attempting to Show(), but this is null/destroyed.");
                return;
            }

            // base.Show() 내부에서 _canvas.enabled = true; 등을 호출
            // base.Show() 호출 전, canvas가 존재하는지 체크( UI_Base 수정이 필요할 수도 )
            // 아래처럼 UI_Popup, UI_Base 쪽에서 null-check 할 수도 있습니다.

            base.Show(); // <-- 만약 여기서 MissingReference가 뜬다면, UI_Base나 UI_Popup에서 canvas null 체크 필요.

            if (this == null)
            {
                Debug.LogWarning("[UI_ConfirmWindow] After base.Show(), this is destroyed?");
                return;
            }

            if (_message == null || _confirm == null)
            {
                Debug.LogWarning("[UI_ConfirmWindow] _message or _confirm is null. Possibly destroyed?");
                return;
            }

            _message.text = message;
            _confirm.onClick.RemoveAllListeners();
            _confirm.onClick.AddListener(Hide);

            if (onConfirmed != null)
                _confirm.onClick.AddListener(onConfirmed);
        }
    }
}




/* //240116-V1
using Practices.UGUI_Management.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Practices.UGUI_Management.UI
{
    public class UI_ConfirmWindow : UI_Popup
    {
        [Resolve] TMP_Text _message;
        [Resolve] Button _confirm;


        public void Show(string message, UnityAction onConfirmed = null)
        {
            base.Show();

            _message.text = message;
            _confirm.onClick.RemoveAllListeners();
            _confirm.onClick.AddListener(Hide);

            if (onConfirmed != null) 
                _confirm.onClick.AddListener(onConfirmed);
        }
    }
}*/