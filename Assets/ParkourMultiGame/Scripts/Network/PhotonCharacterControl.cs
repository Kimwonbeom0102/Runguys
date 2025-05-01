using Photon.Pun;       // [ADDED] Photon
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

/// <summary>
/// Photon 환경에서 동작하는 캐릭터 컨트롤러 예시.
/// - 로컬 플레이어만 입력 + 카메라를 활성화한다.
/// - OwnershipTransfer=Fixed 시, 각 클라이언트가 직접 Instantiate 해야 자기 캐릭터가 IsMine.
/// </summary>
namespace Supercyan.FreeSample
{
    // [CHANGED] MonoBehaviourPun으로 교체 (PhotonView, photonView.IsMine 사용 위해)
    public class PhotonCharacterControl : MonoBehaviour, IPunObservable
    {
        // 플레이어 상태
        private enum State
        {
            Idle,
            Move,
            Jump,
            Boost,
            Climb
        }

        [Header("Player State")]
        [SerializeField] private State _playerState;

        // --- 애니메이터, 리지드바디 등 ---
        [Header("Components")]
        private Rigidbody _rigidbody;
        private Animator _animator;
        private Vector3 _moveDirection;

        // --- 이동 ---
        [Header("Movement Settings")]
        [SerializeField] private float _moveSpeed = 15;
        [SerializeField] private float _boostTime = 5f;
        private bool _isBoosting;

        // --- 점프 ---
        [Header("Jump Settings")]
        [SerializeField] private float _jumpForce = 10f;
        private bool _isJumping;
        private bool _isGrounded;
        private bool _previousGroundedState;
        [SerializeField] private LayerMask _groundLayer; // 점프 가능한 레이어
        [SerializeField] private Transform _groundCheck; // 바닥 체크 위치
        private float _groundCheckRadius = 0.2f; // 바닥 체크 반경

        // --- 사다리 타기 관련 설정 ---
        [Header("Climbing Settings")]
        private float _climbSpeed = 5f;
        private bool _isClimbing;
        private Transform _climbableTop; // 사다리 위쪽 도착 지점
        private Transform _climbableBottom; // 사다리 아래쪽 도착 지점

        // --- 카메라 관련 변수 ---
        [Header("Camera Settings")]
        [SerializeField] private Camera _characterCamera = null;
        private Vector3 _cameraOffset = new Vector3(0, 5, -5); // 카메라와의 상대 위치
        [SerializeField] private float mouseSensitivity = 150f;  // 마우스 감도
        private float _cameraYaw = 0f;  // 카메라의 좌우 회전 각도
        private float _cameraPitch = 0f;  // 카메라의 상하 회전 각도
        private float _cameraSmoothSpeed = 0.1f;  // 카메라 부드러운 전환 속도
        private float _currentYaw = 0f; // 카메라의 좌우 회전 각도 (Y축)

        PhotonView photonView;

        private bool _isCursorLocked;

        // [ADDED] 오디오 리스너도 필요하다면
        [SerializeField] private AudioListener audioListener = null;


        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _animator = GetComponent<Animator>();
            photonView = GetComponent<PhotonView>();
            _playerState = State.Idle;
            _isCursorLocked = false;

            ToggleCursorLock();

            // [ADDED] 카메라/오디오가 없다면 시도해보기 (선택)
            // if (!characterCamera) characterCamera = GetComponentInChildren<Camera>();
            // if (!audioListener)   audioListener   = GetComponentInChildren<AudioListener>();

            // [ADDED] 내 캐릭터가 아니라면 카메라 비활성화
            if (!photonView.IsMine)
            {
                if (_characterCamera)
                {
                    _characterCamera.enabled = false;
                }
                if (audioListener)
                {
                    audioListener.enabled = false;
                }
            }
        }

        private void Start()
        {
            // 카메라 초기 위치를 플레이어의 뒤쪽에 배치
            Vector3 initialPosition = transform.position + Quaternion.Euler(0f, transform.eulerAngles.y, 0f) * _cameraOffset;
            _characterCamera.transform.position = initialPosition;

            // 초기 카메라는 플레이어를 바라보도록 설정
            _characterCamera.transform.LookAt(transform.position + Vector3.up * 1.5f); // 플레이어의 약간 위쪽을 바라보게 설정

            // 초기 카메라 회전 값
            _cameraYaw = transform.eulerAngles.y;
            _cameraPitch = 0f;  // 초기 상태에서는 상하 회전 없이 정면을 보게 설정
        }

        private void Update()
        {
            // [ADDED] 로컬 캐릭터만 입력 처리
            if (!photonView.IsMine)
            {
                return;
            }

            if (_playerState == State.Climb)
            {
                HandleClimbing();
                FinishClimbing();
            }
            else
            {
                Move();
            }

            _isGrounded = Physics.CheckSphere(_groundCheck.position, _groundCheckRadius, _groundLayer);

            if (_isGrounded != _previousGroundedState)
            {
                photonView.RPC("SyncGroundState", RpcTarget.All, _isGrounded);
                _previousGroundedState = _isGrounded;
            }

            HandleStateTransitions();
            UpdateAnimation();
            UpdateCameraPosition();
        }

        [PunRPC]
        void SyncGroundState(bool grounded)
        {
            _isGrounded = grounded;
        }

        private void HandleStateTransitions()
        {
            // 점프 상태에서 착지했을 때 Move 상태로 전환
            if (_playerState == State.Jump && _isGrounded)
            {
                _playerState = State.Move;
                _isJumping = false;
            }

            // 공중에서의 상태 관리 (예: 낙하 상태 추가 가능)
            if (!_isGrounded && _playerState != State.Jump && _playerState != State.Climb)
            {
                _playerState = State.Jump;
            }
        }

        private void UpdateAnimation()
        {
            _animator.SetBool("isMoving", _playerState == State.Move);
            _animator.SetBool("isBoosting", _playerState == State.Boost);
            _animator.SetBool("isJumping", _playerState == State.Jump);
            _animator.SetBool("isClimbing", _playerState == State.Climb);
        }

        private void UpdateCameraPosition()
        {
            // 마우스 입력 받기 (좌우 회전: Mouse X, 상하 회전: Mouse Y)
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            // 상하 회전 각도 조정 (카메라 Pitch)
            _cameraPitch -= mouseY;
            _cameraPitch = Mathf.Clamp(_cameraPitch, -45f, 75f);

            // 좌우 회전 각도 조정 (카메라 Yaw)
            _cameraYaw += mouseX;

            // 카메라 위치 계산
            Quaternion rotation = Quaternion.Euler(_cameraPitch, _cameraYaw, 0f);
            Vector3 offset = rotation * _cameraOffset;

            // 캐릭터를 기준으로 카메라 배치
            Vector3 targetPosition = transform.position + offset;

            // 카메라 위치 바로 적용 (딜레이 제거)
            _characterCamera.transform.position = targetPosition;

            // 카메라가 항상 캐릭터의 중심을 바라보도록 보정
            Vector3 lookAtTarget = transform.position + Vector3.up * 1.5f; // 캐릭터의 머리 위를 기준으로
            _characterCamera.transform.LookAt(lookAtTarget);
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            // [ADDED] 로컬 캐릭터만 입력 처리
            if (!photonView.IsMine)
            {
                return;
            }

            Vector2 input = context.ReadValue<Vector2>();  // 이동 입력 값 받기

            if (_playerState == State.Climb)
            {
                _moveDirection = new Vector3(0, input.y, 0);
            }
            else
            {
                Vector3 forward = _characterCamera.transform.forward;  // 카메라의 앞 방향
                Vector3 right = _characterCamera.transform.right;  // 카메라의 오른쪽 방향

                forward.y = 0;  // y축 회전은 제외
                right.y = 0;    // y축 회전은 제외

                forward.Normalize();
                right.Normalize();

                _moveDirection = (forward * input.y + right * input.x).normalized;  // 입력 값에 따라 이동 방향 설정
            }
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            // [ADDED] 로컬 캐릭터만 입력 처리
            if (!photonView.IsMine)
            {
                return;
            }

            if (_playerState == State.Climb)
            {
                StopClimbing();
                Debug.Log("점프");
            }
            else if (_isGrounded && !_isJumping)  //  땅에 닿고, 점프중이 아닐때
            {
                _playerState = State.Jump;
                _isJumping = true;
                _isGrounded = false;
                _rigidbody.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
            }
        }

        public void OnClimb(InputAction.CallbackContext context)
        {
            // [ADDED] 로컬 캐릭터만 입력 처리
            if (!photonView.IsMine)
            {
                return;
            }

            if (context.phase == InputActionPhase.Performed)
            {
                if (!_isClimbing)
                {
                    TryClimbObject();
                }
                else
                {
                    StopClimbing();
                    Debug.Log("내리기");
                }
            }
        }

        public void OnEscape(InputAction.CallbackContext context)
        {
            // [ADDED] 로컬 캐릭터만 입력 처리
            if (!photonView.IsMine)
            {
                return;
            }

            if (context.phase == InputActionPhase.Performed)
            {
                ToggleCursorLock();
            }
        }

        private void Move()
        {
            Vector3 velocity = _moveDirection * _moveSpeed;
            velocity.y = _rigidbody.linearVelocity.y;  // y축 속도는 그대로 유지
            _rigidbody.linearVelocity = velocity;

            // 이동 방향으로 부드럽게 회전
            if (_moveDirection.magnitude > 0)
            {
                if (_playerState != State.Jump && _playerState != State.Boost)
                {
                    _playerState = State.Move;
                }

                Quaternion targetRotation = Quaternion.LookRotation(_moveDirection);

                // X축 회전을 강제로 고정
                targetRotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
            }
            else
            {
                if (_playerState != State.Jump)
                {
                    _playerState = State.Idle;
                }
            }
        }

        private void ToggleCursorLock()
        {
            _isCursorLocked = !_isCursorLocked;

            if (_isCursorLocked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Boost"))
            {
                StartCoroutine(SpeedUp());
            }

            // 'Deadline' 태그 충돌 처리
            if (other.CompareTag("Deadline"))
            {
                RespawnManager.Instance.RespawnPlayer(gameObject);
            }

            // 'Checkpoint' 태그 충돌 처리
            if (other.CompareTag("Checkpoint"))
            {
                // 새 체크포인트를 리스폰 매니저에 전달
                RespawnManager.Instance.UpdateRespawnPoint(other.transform);
            }
        }

        // 앞에 사다리가 있다면 
        private void TryClimbObject()
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position + Vector3.up * 1.5f, transform.forward, out hit, 1.5f))
            {
                if (hit.collider.CompareTag("Ladder"))
                {
                    StartClimbing(hit.collider.transform);
                }
            }
        }

        // 사다리를 탔을 때 필요한 셋팅
        private void StartClimbing(Transform climbableTransform)
        {
            _playerState = State.Climb;
            _isClimbing = true;
            _isGrounded = false;

            _climbableTop = climbableTransform.Find("Top");   // 사다리의 상단 위치
            _climbableBottom = climbableTransform.Find("Bottom"); // 사다리의 하단 위치

            _rigidbody.isKinematic = true;
            _rigidbody.useGravity = false;
            _rigidbody.linearVelocity = Vector3.zero;

            Vector3 climbPosition = transform.position;
            climbPosition.x = climbableTransform.position.x; // 사다리의 x축 위치로 고정
            climbPosition.z = climbableTransform.position.z; // 사다리의 z축 위치로 고정

            transform.position = climbPosition;


            // 캐릭터가 사다리를 정면으로 바라보도록 회전 조정
            Vector3 ladderForward = climbableTransform.forward; // 사다리의 앞 방향
            ladderForward.y = 0; // 수평 방향만 고려
            Quaternion targetRotation = Quaternion.LookRotation(ladderForward);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 1f);
        }

        // 사다리타기 종료
        private void StopClimbing()
        {
            _playerState = State.Idle;
            _isClimbing = false;

            // Rigidbody 물리 상태 초기화
            _rigidbody.isKinematic = false;
            _rigidbody.useGravity = true;
            _rigidbody.linearVelocity = Vector3.zero;

            // 사다리 끝에서 캐릭터를 앞으로 밀어내지 않도록 수정
            if (transform.position.y <= _climbableBottom.position.y || transform.position.y >= _climbableTop.position.y)
            {
                Vector3 forwardDirection = transform.forward;
                forwardDirection.y = 0; // 수평 방향만 사용
                forwardDirection.Normalize();
                transform.position += forwardDirection * 0.2f;
            }

            // 이동 벡터를 캐릭터의 정면 방향으로 초기화
            _moveDirection = transform.forward;

            // 부드러운 회전을 적용하여 Y축만 유지
            Quaternion targetRotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.2f);

            // 애니메이션 상태 초기화
            _animator.SetBool("isLadderMove", false);
            _animator.SetBool("LadderUp", false);
            _animator.SetBool("LadderDown", false);
        }

        // 사다리의 끝에 도달했는지 체크
        private void FinishClimbing()
        {
            if (_playerState == State.Climb)
            {
                if (transform.position.y >= _climbableTop.position.y) // 플레이어가 사다리의 Top보다 올라갈 경우
                {
                    StopClimbing(); // 상단 도달
                }
                else if (transform.position.y <= _climbableBottom.position.y) // 플레이어가 사다리의 Bottom보다 내려갈 경우
                {
                    StopClimbing(); // 하단 도달

                    Vector3 groundedPosition = transform.position;
                    groundedPosition.y = _climbableBottom.position.y; // Bottom 높이로 캐릭터 위치 보정
                    transform.position = groundedPosition;
                }
            }
        }

        // 사다리를 탔을 때 이동하기 위한 핸들
        private void HandleClimbing()
        {
            if (_moveDirection.y != 0)
            {
                Vector3 newPosition = _rigidbody.position + _moveDirection * _climbSpeed * Time.fixedDeltaTime;
                _rigidbody.MovePosition(newPosition);

                if (_moveDirection.y > 0) // 위로 가고 있다면
                {
                    if (transform.position.y < _climbableTop.position.y)
                    {
                        _animator.SetBool("isLadderMove", true);
                        _animator.SetBool("LadderUp", true);
                        _animator.SetBool("LadderDown", false);
                    }
                }
                else if (_moveDirection.y < 0) // 아래로 가고 있다면
                {
                    if (transform.position.y > _climbableBottom.position.y)
                    {
                        _animator.SetBool("isLadderMove", true);
                        _animator.SetBool("LadderUp", false);
                        _animator.SetBool("LadderDown", true);
                    }
                }
            }
            else
            {
                // 입력이 없을 때 애니메이션 초기화
                _animator.SetBool("isLadderMove", false);
                _animator.SetBool("LadderUp", false);
                _animator.SetBool("LadderDown", false);
                _rigidbody.linearVelocity = Vector3.zero; // 움직임 정지
            }
        }

        // 부스트 상태일 때 일정시간 이동속도 증가
        IEnumerator SpeedUp()
        {
            Debug.Log("부스트모드 발동");

            if (!_isBoosting)
            {
                _isBoosting = true;
                _playerState = State.Boost;
                _moveSpeed = 15;

                Debug.Log($"현재 속도: {_moveSpeed}, 부스트 상태: {_isBoosting}");

                yield return new WaitForSeconds(_boostTime);

                _moveSpeed = 13;
                _isBoosting = false;
                _playerState = State.Move;  // 원래 상태로 복귀

                Debug.Log($"부스트 종료, 현재 속도: {_moveSpeed}, 부스트 상태: {_isBoosting}");
            }
        }

        //IEnumerator SpeedUp()
        //{
        //    Debug.Log("부스트모드 발동");
        //    if (!_isBoosting)
        //    {
        //        _playerState = State.Boost;
        //        _isBoosting = true;
        //        _moveSpeed = 15;
        //        yield return new WaitForSeconds(_boostTime);
        //        _isBoosting = false;
        //        _moveSpeed =3;
        //    }
        //}

        // 플레이어 동기화
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                if (photonView.IsMine)
                {
                    stream.SendNext(_playerState);

                    stream.SendNext(_rigidbody.isKinematic);
                    stream.SendNext(_rigidbody.useGravity);

                    stream.SendNext(_moveSpeed);
                    stream.SendNext(_jumpForce);
                    stream.SendNext(_climbSpeed);

                    stream.SendNext(_isJumping);
                    stream.SendNext(_isBoosting);
                    stream.SendNext(_isClimbing);
                }
            }
            else
            {
                if (!photonView.IsMine)
                {
                    _playerState = (State)stream.ReceiveNext();

                    _rigidbody.isKinematic = (bool)stream.ReceiveNext();
                    _rigidbody.useGravity = (bool)stream.ReceiveNext();

                    _moveSpeed = (float)stream.ReceiveNext();
                    _jumpForce = (float)stream.ReceiveNext();
                    _climbSpeed = (float)stream.ReceiveNext();

                    _isJumping = (bool)stream.ReceiveNext();
                    _isBoosting = (bool)stream.ReceiveNext();
                    _isClimbing = (bool)stream.ReceiveNext();
                }
            }
        }
    }
}