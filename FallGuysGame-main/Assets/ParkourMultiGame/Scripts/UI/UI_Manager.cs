/*using System;
using System.Collections.Generic;
using UnityEngine;
using Practices.UGUI_Management.Singletons; // [ADDED] 네임스페이스 추가

namespace Practices.UGUI_Management.UI
{
    public class UI_Manager : Singleton<UI_Manager>
    {
        // [CHANGE] 생성자 대신 Awake에서 초기화하도록 수정
        const int EXPECTED_MAX_UI_COUNT_IN_SCENE = 30;
        const int EXPECTED_MAX_POPUP_COUNT_IN_SCENE = 10;

        private Dictionary<Type, UI_Base> _uis;
        private UI_Screen _screen;
        private List<UI_Popup> _popupStack;

        protected override void Awake()
        {
            base.Awake();
            _uis = new Dictionary<Type, UI_Base>(EXPECTED_MAX_UI_COUNT_IN_SCENE);
            _popupStack = new List<UI_Popup>(EXPECTED_MAX_POPUP_COUNT_IN_SCENE);
        }

        public IEnumerable<UI_Popup> popups => _popupStack;

        public void Register(UI_Base ui)
        {
            if (_uis.ContainsKey(ui.GetType()))
            {
                Debug.LogWarning($"UI {ui.GetType()} is already registered. Skipping re-register.");
                return;
            }
            _uis.Add(ui.GetType(), ui);
            Debug.Log($"Registered UI {ui.GetType()}");

            if (ui is UI_Popup popup)
            {
                ui.onShow += () => Push(popup);
                ui.onHide += () => Pop(popup);
            }
        }

        public void Unregister(UI_Base ui)
        {
            if (_uis.Remove(ui.GetType()))
            {
                Debug.Log($"Unregistered UI {ui.GetType()}");
            }
            else
            {
                Debug.LogError($"Failed to unregister ui {ui.GetType()}. Not exist?");
            }
        }

        public T Resolve<T>() where T : UI_Base
        {
            if (_uis.TryGetValue(typeof(T), out UI_Base result))
            {
                if (result == null || result.gameObject == null)
                {
                    _uis.Remove(typeof(T));
                    return InstantiateUI<T>();
                }
                return (T)result;
            }
            else
            {
                return InstantiateUI<T>();
            }
        }

        private T InstantiateUI<T>() where T : UI_Base
        {
            string path = $"UI/Canvas - {typeof(T).Name.Substring(3)}"; // 예: UI_Lobby -> "UI/Canvas - Lobby"
            UI_Base prefab = Resources.Load<UI_Base>(path);
            if (prefab == null)
                throw new Exception($"Failed to resolve ui {typeof(T)}. Not exist in Resources: {path}");
            T newUI = (T)GameObject.Instantiate(prefab);
            return newUI;
        }

        public void SetScreen(UI_Screen screen)
        {
            if (_screen != null)
            {
                _screen.inputActionsEnabled = false;
                _screen.Hide();
            }
            _screen = screen;
            _screen.sortingOrder = 0;
            _screen.inputActionsEnabled = true;
        }

        public void Push(UI_Popup popup)
        {
            int popupIndex = _popupStack.FindLastIndex(ui => ui == popup);
            if (popupIndex >= 0)
            {
                _popupStack.RemoveAt(popupIndex);
            }
            int sortingOrder = 1;
            if (_popupStack.Count > 0)
            {
                UI_Popup prevPopup = _popupStack[_popupStack.Count - 1];
                prevPopup.inputActionsEnabled = false;
                sortingOrder = prevPopup.sortingOrder + 1;
            }
            popup.sortingOrder = sortingOrder;
            popup.inputActionsEnabled = true;
            _popupStack.Add(popup);
            Debug.Log($"Pushed {popup.name}");
        }

        public void Pop(UI_Popup popup)
        {
            int popupIndex = _popupStack.FindLastIndex(ui => ui == popup);
            if (popupIndex < 0)
                return;
            if (popupIndex == _popupStack.Count - 1)
            {
                _popupStack[popupIndex].inputActionsEnabled = false;
                if (popupIndex > 0)
                    _popupStack[popupIndex - 1].inputActionsEnabled = true;
            }
            _popupStack.RemoveAt(popupIndex);
            Debug.Log($"Popped {popup.name}");
        }
    }
}*/



using Practices.UGUI_Management.Singletons;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Practices.UGUI_Management.UI
{
    public class UI_Manager : Singleton<UI_Manager>
    {
        public UI_Manager()
        {
            _uis = new Dictionary<Type, UI_Base>(EXPECTED_MAX_UI_COUNT_IN_SCENE);
            _popupStack = new List<UI_Popup>(EXPECTED_MAX_POPUP_COUNT_IN_SCENE);
        }

        public IEnumerable<UI_Popup> popups => _popupStack;

        const int EXPECTED_MAX_UI_COUNT_IN_SCENE = 30;
        const int EXPECTED_MAX_POPUP_COUNT_IN_SCENE = 10;

        Dictionary<Type, UI_Base> _uis;
        UI_Screen _screen;
        List<UI_Popup> _popupStack;

        // [CHANGED] 중복 등록 방지 로직
        public void Register(UI_Base ui)
        {
            if (_uis.ContainsKey(ui.GetType()))
            {
                Debug.LogWarning($"UI {ui.GetType()} is already registered. Skipping re-register.");
                return;
            }

            if (_uis.TryAdd(ui.GetType(), ui))
            {
                Debug.Log($"Registered UI {ui.GetType()}");

                if (ui is UI_Popup popup)
                {
                    ui.onShow += () => Push(popup);
                    ui.onHide += () => Pop(popup);
                }
            }
            else
            {
                Debug.LogError($"Failed to register ui {ui.GetType()}. Already exist?");
            }
        }

        // [CHANGED] 중복 등록 방지 로직
        public void Unregister(UI_Base ui)
        {
            if (_uis.Remove(ui.GetType()))
            {
                Debug.Log($"Unregistered UI {ui.GetType()}");
            }
            else
            {
                Debug.LogError($"Failed to unregister ui {ui.GetType()}. Not exist?");
            }
        }

        // [CHANGED] 
        public T Resolve<T>() where T : UI_Base
        {
            if (_uis.TryGetValue(typeof(T), out UI_Base result))
            {
                // [ADDED] 파괴되었는지(=null) 체크
                if (result == null || result.gameObject == null)
                {
                    _uis.Remove(typeof(T));
                    return InstantiateUI<T>();
                }

                return (T)result;
            }
            else
            {
                return InstantiateUI<T>();
            }
        }

        // [ADDED] UI Prefab 로드 + Instantiate를 한 곳에서 처리
        private T InstantiateUI<T>() where T : UI_Base
        {
            string path = $"UI/Canvas - {typeof(T).Name.Substring(3)}";
            // 예: T가 UI_Lobby 라면  => UI/Canvas - Lobby
            // 실제 Resources 폴더 구조에 맞게 수정 필요

            UI_Base prefab = Resources.Load<UI_Base>(path);
            if (prefab == null)
                throw new Exception($"Failed to resolve ui {typeof(T)}. Not exist in Resources: {path}");

            T newUI = (T)GameObject.Instantiate(prefab);
            return newUI;
        }

        public void SetScreen(UI_Screen screen)
        {
            if (_screen != null)
            {
                _screen.inputActionsEnabled = false;
                _screen.Hide();
            }

            _screen = screen;
            _screen.sortingOrder = 0;
            _screen.inputActionsEnabled = true;
        }

        public void Push(UI_Popup popup)
        {
            int popupIndex = _popupStack.FindLastIndex(ui => ui == popup);
            if (popupIndex >= 0)
            {
                _popupStack.RemoveAt(popupIndex);
            }

            int sortingOrder = 1;
            if (_popupStack.Count > 0)
            {
                UI_Popup prevPopup = _popupStack[^1];
                prevPopup.inputActionsEnabled = false;
                sortingOrder = prevPopup.sortingOrder + 1;
            }

            popup.sortingOrder = sortingOrder;
            popup.inputActionsEnabled = true;
            _popupStack.Add(popup);
            Debug.Log($"Pushed {popup.name}");
        }

        public void Pop(UI_Popup popup)
        {
            int popupIndex = _popupStack.FindLastIndex(ui => ui == popup);
            if (popupIndex < 0)
                return; //new Exception($"Failed to remove popup. {popup.name}");

            if (popupIndex == _popupStack.Count - 1)
            {
                _popupStack[popupIndex].inputActionsEnabled = false;
                if (popupIndex > 0)
                    _popupStack[popupIndex - 1].inputActionsEnabled = true;
            }

            _popupStack.RemoveAt(popupIndex);
            Debug.Log($"Popped {popup.name}");
        }
    }
}


