// --- UI_Base.cs ---
using Practices.UGUI_Management.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Practices.UGUI_Management.UI
{
    [RequireComponent(typeof(Canvas))]
    public abstract class UI_Base : ComponentResolvingBehaviour
    {
        public int sortingOrder
        {
            get => _canvas ? _canvas.sortingOrder : 0;  // [ADDED] null 체크
            set { if (_canvas) _canvas.sortingOrder = value; }
        }

        public bool inputActionsEnabled
        {
            get => playerInputActions.asset.enabled;
            set
            {
                if (value)
                    playerInputActions.Enable();
                else
                    playerInputActions.Disable();
            }
        }

        protected UI_Manager manager;
        protected PlayerInputActions playerInputActions;
        Canvas _canvas;
        GraphicRaycaster _graphicRaycaster;
        EventSystem _eventSystem;
        PointerEventData _pointerEventData;
        List<RaycastResult> _raycastResultBuffer;

        public event Action onShow;
        public event Action onHide;

        protected override void Awake()
        {
            base.Awake();

            _canvas = GetComponent<Canvas>();
            _graphicRaycaster = GetComponent<GraphicRaycaster>();
            _eventSystem = EventSystem.current;
            _pointerEventData = new PointerEventData(_eventSystem);
            _raycastResultBuffer = new List<RaycastResult>(1);

            playerInputActions = new PlayerInputActions();  // [ADDED] InputActions 인스턴스 생성
            manager = UI_Manager.instance;
            manager.Register(this);
        }

        protected virtual void Start() { }

        // [ADDED] Show()에서 null 체크 후 캔버스 활성화
        public virtual void Show()
        {
            if (!this || !_canvas) // 이미 Destroy되었거나, Canvas가 null
            {
                Debug.LogWarning($"[UI_Base] Attempted Show(), but is destroyed or _canvas is null. name={name}");
                return;
            }

            _canvas.enabled = true;
            onShow?.Invoke();
        }

        // [ADDED] Hide()에서 null 체크 후 캔버스 비활성화
        public virtual void Hide()
        {
            if (!this || !_canvas) // 이미 Destroy되었거나 Null
            {
                Debug.LogWarning($"[UI_Base] Canvas is null or destroyed! Cannot Hide {name}");
                return;
            }

            _canvas.enabled = false;
            onHide?.Invoke();
        }

        // [CHANGE/ADDED] OnDestroy() 수정: playerInputActions를 Disable하고 Dispose하여 메모리 누수 방지
        protected virtual void OnDestroy()
        {
            if (playerInputActions != null)
            {
                playerInputActions.Disable(); // [ADDED]
                playerInputActions.Dispose();  // [ADDED]
            }
            onHide?.Invoke();
            manager.Unregister(this);
        }

        public bool TryGraphicRaycast<T>(Vector2 pointerPosition, out T result)
            where T : Component
        {
            if (!_graphicRaycaster)
            {
                result = default;
                return false;
            }

            _pointerEventData.position = pointerPosition;
            _raycastResultBuffer.Clear();
            _graphicRaycaster.Raycast(_pointerEventData, _raycastResultBuffer);

            if (_raycastResultBuffer.Count > 0)
            {
                if (_raycastResultBuffer[0].gameObject.TryGetComponent(out result))
                    return true;
            }

            result = default;
            return false;
        }
    }
}



/*
using Practices.UGUI_Management.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Practices.UGUI_Management.UI
{
    [RequireComponent(typeof(Canvas))]
    public abstract class UI_Base : ComponentResolvingBehaviour
    {
        public int sortingOrder
        {
            get => _canvas ? _canvas.sortingOrder : 0;  // [ADDED] null 체크
            set { if (_canvas) _canvas.sortingOrder = value; }
        }

        public bool inputActionsEnabled
        {
            get => playerInputActions.asset.enabled;
            set
            {
                if (value)
                    playerInputActions.Enable();
                else
                    playerInputActions.Disable();
            }
        }

        protected UI_Manager manager;
        protected PlayerInputActions playerInputActions;
        Canvas _canvas;
        GraphicRaycaster _graphicRaycaster;
        EventSystem _eventSystem;
        PointerEventData _pointerEventData;
        List<RaycastResult> _raycastResultBuffer;

        public event Action onShow;
        public event Action onHide;

        protected override void Awake()
        {
            base.Awake();

            _canvas = GetComponent<Canvas>();
            _graphicRaycaster = GetComponent<GraphicRaycaster>();
            _eventSystem = EventSystem.current;
            _pointerEventData = new PointerEventData(_eventSystem);
            _raycastResultBuffer = new List<RaycastResult>(1);

            playerInputActions = new PlayerInputActions();
            manager = UI_Manager.instance;
            manager.Register(this);
        }

        protected virtual void Start() { }

        // [ADDED] null 체크
        public virtual void Show()
        {
            if (!this || !_canvas) // 이미 Destroy되었거나, Canvas가 null
            {
                Debug.LogWarning($"[UI_Base] Attempted Show(), but is destroyed or _canvas is null. name={name}");
                return;
            }

            _canvas.enabled = true;
            onShow?.Invoke();
        }

        // [ADDED] null 체크
        public virtual void Hide()
        {
            if (!this || !_canvas) // 이미 Destroy되었거나 Null
            {
                Debug.LogWarning($"[UI_Base] Canvas is null or destroyed! Cannot Hide {name}");
                return;
            }

            _canvas.enabled = false;
            onHide?.Invoke();
        }

        private void OnDestroy()
        {
            onHide?.Invoke();
            manager.Unregister(this);
        }

        public bool TryGraphicRaycast<T>(Vector2 pointerPosition, out T result)
            where T : Component
        {
            if (!_graphicRaycaster)
            {
                result = default;
                return false;
            }

            _pointerEventData.position = pointerPosition;
            _raycastResultBuffer.Clear();
            _graphicRaycaster.Raycast(_pointerEventData, _raycastResultBuffer);

            if (_raycastResultBuffer.Count > 0)
            {
                if (_raycastResultBuffer[0].gameObject.TryGetComponent(out result))
                    return true;
            }

            result = default;
            return false;
        }
    }
}




*/