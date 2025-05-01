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
            // [ADDED] �켱, �̹� �ı��Ǿ�����(=this == null) Ȥ�� Canvas�� ����ִ��� �˻�
            if (this == null)
            {
                Debug.LogWarning("[UI_ConfirmWindow] Attempting to Show(), but this is null/destroyed.");
                return;
            }

            // base.Show() ���ο��� _canvas.enabled = true; ���� ȣ��
            // base.Show() ȣ�� ��, canvas�� �����ϴ��� üũ( UI_Base ������ �ʿ��� ���� )
            // �Ʒ�ó�� UI_Popup, UI_Base �ʿ��� null-check �� ���� �ֽ��ϴ�.

            base.Show(); // <-- ���� ���⼭ MissingReference�� ��ٸ�, UI_Base�� UI_Popup���� canvas null üũ �ʿ�.

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