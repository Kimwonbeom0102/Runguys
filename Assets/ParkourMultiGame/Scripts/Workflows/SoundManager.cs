using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;  // 싱글톤 패턴 (씬이 바뀌어도 유지됨)

    public AudioSource BGMSource;  // 배경음악용 AudioSource
    public AudioSource SFXSource;  // 효과음용 AudioSource
    public Dictionary<string, AudioClip> SFXClips = new Dictionary<string, AudioClip>(); // 효과음 저장
    // 다른 스크립트에서 SoundManager.Instance.PlaySFX("효과음이름"); 으로 호출하면 됨

    public AudioClip[] bgmClips; // 씬별 BGM 리스트
    public AudioClip[] sfxClips; // 모든 효과음 리스트

    private void Awake()
    {
        Debug.Log("[SoundManager] Awake 실행됨");

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("[SoundManager] 싱글톤 생성 및 유지됨");
        }
        else
        {
            Debug.Log("[SoundManager] 기존 싱글톤이 존재하여 삭제됨");
            Destroy(gameObject);
            return;
        }

        foreach (var clip in sfxClips)
        {
            SFXClips[clip.name] = clip;
        }
    }


    private void Start()
    {
        Debug.Log("[SoundManager] Start 실행됨! 씬 로딩 이벤트 연결");

        SceneManager.sceneLoaded += OnSceneLoaded;

        // 강제 호출해서 `OnSceneLoaded()`가 실행되는지 확인
        Debug.Log("[SoundManager] 현재 씬에서 OnSceneLoaded 강제 실행");
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"[SoundManager] 씬 로드됨: {scene.name}");

        switch (scene.name)
        {
            case "Login":
                Debug.Log("[SoundManager] Login 씬 BGM 실행 시도");
                PlayBGM(0);
                break;
            case "Lobby":
                Debug.Log("[SoundManager] Lobby 씬 BGM 실행 시도");
                PlayBGM(0);
                break;
            case "GamePlay":
                Debug.Log("[SoundManager] GamePlay 씬 BGM 실행 시도");
                PlayBGM(0);
                break;
            case "DoorDash":
                Debug.Log("[SoundManager] DoorDash 씬 BGM 실행 시도");
                PlayBGM(1);
                break;
            case "JumpClub":
                Debug.Log("[SoundManager] JumpClub 씬 BGM 실행 시도");
                PlayBGM(2);
                break;
            case "ThinIce":
                Debug.Log("[SoundManager] ThinIce 씬 BGM 실행 시도");
                PlayBGM(3);
                break;
            case "Tiptoe":
                Debug.Log("[SoundManager] Tiptoe 씬 BGM 실행 시도");
                PlayBGM(4);
                break;
            case "WallParty":
                Debug.Log("[SoundManager] WallParty 씬 BGM 실행 시도");
                PlayBGM(5);
                break;

            default:
                Debug.LogWarning("[SoundManager] 해당 씬에 맞는 BGM이 없습니다!");
                break;
        }
    }


    public void PlayBGM(int index)
    {
        Debug.Log($"[SoundManager] PlayBGM 호출됨. 인덱스: {index}");

        if (BGMSource == null)
        {
            Debug.LogError("[SoundManager] BGMSource가 없습니다! AudioSource 확인 필요");
            return;
        }

        if (index < 0 || index >= bgmClips.Length)
        {
            Debug.LogError($"[SoundManager] PlayBGM 오류: 유효하지 않은 인덱스 {index}");
            return;
        }

        if (bgmClips[index] == null)
        {
            Debug.LogError($"[SoundManager] BGM 클립이 없습니다! index: {index}");
            return;
        }

        BGMSource.clip = bgmClips[index];
        BGMSource.loop = true;
        BGMSource.Play();

        Debug.Log($"[SoundManager] BGM 재생 시작: {bgmClips[index].name}");
    }


    public void PlaySFX(string sfxName)
    {
        if (SFXClips.ContainsKey(sfxName))
        {
            SFXSource.PlayOneShot(SFXClips[sfxName]);
        }
        else
        {
            Debug.LogWarning("SFX not found: " + sfxName);
        }
    }

    public void SetBGMVolume(float volume)
    {
        BGMSource.volume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        SFXSource.volume = volume;
    }
}


//private void Start()
//{
//    // 기존 설정된 값 불러오기 (0~1 사이 값)
//    bgmSlider.value = PlayerPrefs.GetFloat("BGMVolume", 1f);
//    sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);

//    bgmSlider.onValueChanged.AddListener(SetBGMVolume);
//    sfxSlider.onValueChanged.AddListener(SetSFXVolume);
//}

//public void SetBGMVolume(float volume)  // 볼륨 조절기능
//{
//    audioMixer.SetFloat("BGM", Mathf.Log10(volume) * 20);
//    PlayerPrefs.SetFloat("BGMVolume", volume);
//}

//public void SetSFXVolume(float volume) // 볼륨 조절
//{
//    audioMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
//    PlayerPrefs.SetFloat("SFXVolume", volume);
//}




//using UnityEngine;


//public class SoundManager : MonoBehaviour
//{
//    // 캐릭터의 오디오 소스를 저장
//    private AudioSource audioSource;

//    [Header("사운드 클립설정")]
//    [SerializeField] private AudioClip _footstepSound;
//    [SerializeField] private AudioClip _jumpSound;
//    [SerializeField] private AudioClip _landSound;
//    [SerializeField] private AudioClip _crackSound;
//    [SerializeField] private AudioClip _countDownSound;
//    [SerializeField] private AudioClip _countDown;
//    [SerializeField] private AudioClip _ladderCatch;
//    [SerializeField] private AudioClip _bgm;



//    private bool isGrounded = true;  //  캐릭터가 땅에 있는지 체크 (기본지속이기때문에 true)

//    public void Awake()
//    {
//        audioSource = GetComponent<AudioSource>();
//        if(audioSource == null)
//        {
//            Debug.LogError("AudioSource 컴포넌트가 비어있거나 추가되지 않았습니다.");
//        }
//    }

//    public void PlayCrakSound()
//    {
//        if(_crackSound != null)
//        {
//            audioSource.PlayOneShot(_crackSound); // 크랙사운드 한 번 재생
//        }
//        else
//        {
//            Debug.LogWarning("크랙 사운드 클립을 설정해주세요");
//        }
//    }

//    public void PlayCountDownSound() 
//    {
//        if (_countDownSound != null)
//        {
//            audioSource.PlayOneShot(_countDownSound); // 카운트다운 사운드 한 번 재생
//        }
//        else
//        {
//            Debug.LogWarning("카운트다운 사운드 클립을 설정해주세요");
//        }
//    }

//    public void PlayCountDownEffectSound()
//    {
//        if (_countDown != null)
//        {
//            audioSource.PlayOneShot(_countDown); // 카운트다운 효과음 재생
//        }
//        else
//        {
//            Debug.LogWarning("카운트다운 효과음을 설정해주세요");
//        }
//    }

//    public void PlayLadderSound()
//    {
//        if(_ladderCatch != null)
//        {
//            audioSource.PlayOneShot(_ladderCatch); // 사다리 잡는 소리 한 번 재생
//        }
//        else
//        {
//            Debug.LogWarning("사다리 잡는 사운드 클립을 설정해주세요");
//        }
//    }


//    public void PlayFootStepSound()
//    {
//        if(_footstepSound != null)
//        {
//            audioSource.PlayOneShot(_footstepSound); // 발소리를 한 번 재생
//        }
//        else
//        {
//            Debug.LogWarning("발소리 사운드 클립을 설정해주세요");
//        }

//    }

//    public void PlayfJumpSound()
//    {
//        if(_jumpSound != null)
//        {
//            audioSource.PlayOneShot(_jumpSound); // 점프소리를 한 번 재생
//        }
//        else
//        {
//            Debug.LogWarning("점프 사운드 클립을 설정해주세요");
//        }

//    }
//    public void PlayLandSound()
//    {
//        if (_landSound != null)
//        {
//            audioSource.PlayOneShot(_landSound); // 착지소리를 한 번 재생
//        }
//        else
//        {
//            Debug.LogWarning("착지 사운드 클립을 설정해주세요");
//        }

//    }

//    public void SetGroundedState(bool grounded)
//    {
//        isGrounded = grounded;  
//    }


//}

