/*// 0115V2[CHANGED/ADDED code with comments]

using Photon;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem; // [ADDED] New Input System

namespace Practices.PhotonPunClient.Network
{
    public class ClientCharacterController : MonoBehaviour, IPunInstantiateMagicCallback
    {
        // 멀티톤
        public static Dictionary<int, ClientCharacterController> controllers
            = new Dictionary<int, ClientCharacterController>();

        public int ownerActorNr => _photonView.OwnerActorNr;
        public int photonViewId => _photonView.ViewID;
        public bool isInitialized { get; private set; }
        public Pickable pickable { get; set; }

        PhotonView _photonView;
        NavMeshAgent _agent;

        // [CHANGED] New Input System: PlayerInput
        private PlayerInput _playerInput;
        private InputAction _inputActions; // [ADDED] 기존 InputActions
        private InputAction _leftClickAction;

        [SerializeField] LayerMask _groundMask;
        [SerializeField] LayerMask _pickable;
        [SerializeField] LayerMask _kickable;
        [SerializeField] Transform _rightHand;
        [SerializeField] Transform _leftHand;

        // [ADDED] 파쿠르/폴가이즈 스타일 속성
        [Header("Parkour Settings")]
        [SerializeField] float walkSpeed = 3.5f;
        [SerializeField] float sprintSpeed = 6f;
        [SerializeField] float jumpForce = 7f;
        [SerializeField] float rollDistance = 3f;
        // 상황에 따라 슬라이딩, 벽잡기 등 추가

        bool isSprinting = false;

        private void Awake()
        {
            _photonView = GetComponent<PhotonView>();
            _agent = GetComponent<NavMeshAgent>();

            // [ADDED] PlayerInput 컴포넌트 찾기
            _playerInput = GetComponent<PlayerInput>();
            if (_playerInput != null)
            {
                // 예: InputActions에 "LeftClick" 액션이 존재해야 함
                _leftClickAction = _playerInput.actions["LeftClick"];
            }
            else
            {
                Debug.LogError("PlayerInput component not found!");
            }
        }

        private void OnEnable()
        {
            if (_leftClickAction != null)
            {
                _leftClickAction.performed += OnLeftClickHandler;
                _leftClickAction.Enable();
            }
            // [ADDED] Enable our custom input actions
            SetupParkourActions(true);
        }

        private void OnDisable()
        {
            if (_leftClickAction != null)
            {
                _leftClickAction.performed -= OnLeftClickHandler;
                _leftClickAction.Disable();
            }
            // [ADDED] Disable our custom input actions
            SetupParkourActions(false);
        }

        private void OnDestroy()
        {
            if (_leftClickAction != null)
            {
                _leftClickAction.performed -= OnLeftClickHandler;
            }
        }

        // [ADDED] 파쿠르 입력 설정 (WASD, Shift, Space, Ctrl 등)
        void SetupParkourActions(bool enable)
        {
            if (_inputActions == null) return; // PhotonView 소유자가 아니면 null 일수 있음

            if (enable)
            {
                // 이동(WASD), 스프린트(Shift), 점프(Space), 구르기(Ctrl) 등
                _inputActions.Player.Move.Enable();
                _inputActions.Player.Sprint.performed += OnSprintPerformed;
                _inputActions.Player.Sprint.canceled += OnSprintCanceled;

                _inputActions.Player.Jump.performed += OnJump;
                _inputActions.Player.Roll.performed += OnRoll;

                // 균형 잡기(Q/E), 상호작용(F), 던지기(R) 등
                _inputActions.Player.LeanLeft.performed += OnLeanLeft;
                _inputActions.Player.LeanRight.performed += OnLeanRight;
                _inputActions.Player.Interact.performed += OnInteract;
                _inputActions.Player.ThrowItem.performed += OnThrowItem;

                _inputActions.Player.Move.Enable();
                _inputActions.Player.Sprint.Enable();
                _inputActions.Player.Jump.Enable();
                _inputActions.Player.Roll.Enable();
                _inputActions.Player.LeanLeft.Enable();
                _inputActions.Player.LeanRight.Enable();
                _inputActions.Player.Interact.Enable();
                _inputActions.Player.ThrowItem.Enable();
            }
            else
            {
                _inputActions.Player.Move.Disable();
                _inputActions.Player.Sprint.performed -= OnSprintPerformed;
                _inputActions.Player.Sprint.canceled -= OnSprintCanceled;

                _inputActions.Player.Jump.performed -= OnJump;
                _inputActions.Player.Roll.performed -= OnRoll;

                _inputActions.Player.LeanLeft.performed -= OnLeanLeft;
                _inputActions.Player.LeanRight.performed -= OnLeanRight;
                _inputActions.Player.Interact.performed -= OnInteract;
                _inputActions.Player.ThrowItem.performed -= OnThrowItem;

                _inputActions.Player.Sprint.Disable();
                _inputActions.Player.Jump.Disable();
                _inputActions.Player.Roll.Disable();
                _inputActions.Player.LeanLeft.Disable();
                _inputActions.Player.LeanRight.Disable();
                _inputActions.Player.Interact.Disable();
                _inputActions.Player.ThrowItem.Disable();
            }
        }

        public void OnLeftClickHandler(InputAction.CallbackContext context)
        {
            if (this == null) return;
            Debug.Log("LeftClick performed via PlayerInput");

            // [ADDED] 여기서도 '잡기' 또는 '밀기'로 사용할 수 있음
            // 만약 pickable과 로직이 충돌하면, OnLeftClick(InputAction.CallbackContext) 기존 코드를 참조
        }

        public Transform GetEmptyHand()
        {
            return _rightHand; // 일단 오른손 사용
        }

        public void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable
            {
                { PlayerInGamePlayPropertyKey.IS_CHARACTER_SPAWNED, true }
            });

            isInitialized = true;

            if (_photonView.IsMine)
            {
                _agent.enabled = true;

                // [CHANGED] 기존 InputActions 직접 Enable
                _inputActions = new InputActions();
                _inputActions.Player.Fire.performed += OnLeftClick;
                _inputActions.Player.MouseRight.performed += OnRightClick;
                _inputActions.Enable();
            }
            else
            {
                _agent.enabled = false;
            }

            controllers.Add(_photonView.OwnerActorNr, this);
            Debug.Log($"[ClientCharacterController] Instantiated. Actor={_photonView.OwnerActorNr}");
        }

        // [CHANGED] 기존 Mouse LeftClick - "Fire" -> OnLeftClick
        void OnLeftClick(InputAction.CallbackContext context)
        {
            if (pickable)
            {
                pickable.Drop();
                return;
            }
            else
            {
                Collider[] cols = Physics.OverlapSphere(transform.position, 1f, _pickable);
                if (cols.Length > 0)
                {
                    cols[0].GetComponent<Pickable>().PickUp();
                    return;
                }
            }

            if (Physics.SphereCast(transform.position, 1f, transform.forward, out RaycastHit hit, 1f, _kickable))
            {
                Kickable kickable = hit.collider.GetComponent<Kickable>();
                kickable.Kick((hit.point - transform.position) * 3f);
            }
        }

        // [CHANGED] 기존 Mouse RightClick - "MouseRight" -> OnRightClick
        void OnRightClick(InputAction.CallbackContext context)
        {
            if (!_photonView.IsMine) return;
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            Debug.DrawRay(ray.origin, ray.direction);

            if (Physics.Raycast(ray, out RaycastHit hit, float.PositiveInfinity, _groundMask))
            {
                _agent.SetDestination(hit.point);
            }
        }

        // [ADDED] 파쿠르 함수들
        void OnSprintPerformed(InputAction.CallbackContext ctx)
        {
            isSprinting = true;
            _agent.speed = sprintSpeed;
        }
        void OnSprintCanceled(InputAction.CallbackContext ctx)
        {
            isSprinting = false;
            _agent.speed = walkSpeed;
        }

        void OnJump(InputAction.CallbackContext ctx)
        {
            // 점프 (NavMeshAgent 쓰면 물리 점프 구현이 좀 까다롭지만 예시)
            Debug.Log("Jump pressed!");
            // NavMeshAgent로는 점프 적용이 어려우니, 임시로...
            // TODO: 직접 Rigidbody.AddForce or CharacterController
        }

        void OnRoll(InputAction.CallbackContext ctx)
        {
            // 구르기 (Ctrl)
            Debug.Log("Roll performed!");
            // TODO: 애니메이션, 이동 처리
        }

        void OnLeanLeft(InputAction.CallbackContext ctx)
        {
            Debug.Log("Lean Left (Q)");
            // 좁은 다리 등에서 균형 기울이기
        }

        void OnLeanRight(InputAction.CallbackContext ctx)
        {
            Debug.Log("Lean Right (E)");
        }

        void OnInteract(InputAction.CallbackContext ctx)
        {
            Debug.Log("Interact(F) pressed");
            // 아이템 줍기, 문 열기 등
        }

        void OnThrowItem(InputAction.CallbackContext ctx)
        {
            Debug.Log("Throw(R) pressed");
            // 잡은 아이템 던지기
        }
    }
}*/





/*// 0116-V2 [CHANGED/ADDED code with comments]

using Photon;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;  // [ADDED] New Input System

namespace Practices.PhotonPunClient.Network
{
    public class ClientCharacterController : MonoBehaviour, IPunInstantiateMagicCallback
    {
        // 멀티톤
        public static Dictionary<int, ClientCharacterController> controllers
            = new Dictionary<int, ClientCharacterController>();

        public int ownerActorNr => _photonView.OwnerActorNr;
        public int photonViewId => _photonView.ViewID;
        public bool isInitialized { get; private set; }
        
        //public Pickable pickable { get; set; }

        PhotonView _photonView;
        NavMeshAgent _agent;

        // [CHANGED] New Input System: PlayerInput
        private PlayerInput _playerInput;

        // [CHANGED] 자동 생성된 클래스 이름이 "PlayerInputActions"라고 가정
        private PlayerInputActions _inputActions;

        // [ADDED] WASD 움직임을 위한 액션
        private InputAction _moveAction; // Move (Vector2)

        [SerializeField] LayerMask _groundMask;
        [SerializeField] LayerMask _pickable;
        [SerializeField] LayerMask _kickable;
        [SerializeField] Transform _rightHand;
        [SerializeField] Transform _leftHand;

        // [ADDED] 이동속도
        [SerializeField] float moveSpeed = 3.5f;

        private void Awake()
        {
            _photonView = GetComponent<PhotonView>();
            _agent = GetComponent<NavMeshAgent>();

            // [CHANGED] PlayerInput 컴포넌트 찾기
            _playerInput = GetComponent<PlayerInput>();
            if (_playerInput != null)
            {
                // .inputactions 파일에서 "Move" 액션이 존재한다고 가정
                _moveAction = _playerInput.actions["Move"];
                // (만약 "Move" 대신 다른 이름이면 코드 수정)
            }
            else
            {
                Debug.LogError("PlayerInput component not found!");
            }
        }

        private void OnEnable()
        {
            if (_moveAction != null)
            {
                // [ADDED] Move 액션의 performed/canceled 이벤트 등록
                _moveAction.performed += OnMovePerformed;
                _moveAction.canceled += OnMoveCanceled;
                _moveAction.Enable();
            }
        }

        private void OnDisable()
        {
            if (_moveAction != null)
            {
                _moveAction.performed -= OnMovePerformed;
                _moveAction.canceled -= OnMoveCanceled;
                _moveAction.Disable();
            }
        }

        private void OnDestroy()
        {
            // 이벤트 해제
            if (_moveAction != null)
            {
                _moveAction.performed -= OnMovePerformed;
                _moveAction.canceled -= OnMoveCanceled;
            }
        }

        // [ADDED] Move 액션 처리
        private void OnMovePerformed(InputAction.CallbackContext context)
        {
            if (!_photonView.IsMine) return; // 내 캐릭터만 제어

            // Vector2: (x = A/D, y = W/S)
            Vector2 input = context.ReadValue<Vector2>();
            // NavMeshAgent 이용: 현재 위치 + (방향 * 속도)
            Vector3 moveDir = new Vector3(input.x, 0f, input.y);

            // [CHANGED] NavMeshAgent 로 즉시 SetDestination
            // 단, 매 프레임마다 반복 호출은 과도할 수 있음 → 예시로 작성
            Vector3 targetPos = transform.position + moveDir * moveSpeed * Time.deltaTime;
            _agent.SetDestination(targetPos);
        }

        private void OnMoveCanceled(InputAction.CallbackContext context)
        {
            if (!_photonView.IsMine) return;

            // [ADDED] WASD 키에서 손 뗐을 때, 
            // 일단 현재 위치에 멈추도록 agent Destination을 자기 위치로 설정
            _agent.SetDestination(transform.position);
        }

        public Transform GetEmptyHand()
        {
            return _rightHand; // 일단 오른손 사용
        }

        public void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable
            {
                { PlayerInGamePlayPropertyKey.IS_CHARACTER_SPAWNED, true }
            });

            isInitialized = true;

            if (_photonView.IsMine)
            {
                _agent.enabled = true;

                // [CHANGED] PlayerInputActions 인스턴스 생성
                _inputActions = new PlayerInputActions();
                // [CHANGED] 원래 Fire/MouseRight 등 있었지만, 여기서는 WASD만 사용
                // _inputActions.Player.Fire.performed += ... 제거함

                _inputActions.Enable();
            }
            else
            {
                _agent.enabled = false;
            }

            controllers.Add(_photonView.OwnerActorNr, this);
            Debug.Log($"[ClientCharacterController] Instantiated. Actor={_photonView.OwnerActorNr}");
        }
    }
}
*/

