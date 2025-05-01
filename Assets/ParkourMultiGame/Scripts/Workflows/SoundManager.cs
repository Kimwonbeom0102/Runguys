using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;  // �̱��� ���� (���� �ٲ� ������)

    public AudioSource BGMSource;  // ������ǿ� AudioSource
    public AudioSource SFXSource;  // ȿ������ AudioSource
    public Dictionary<string, AudioClip> SFXClips = new Dictionary<string, AudioClip>(); // ȿ���� ����
    // �ٸ� ��ũ��Ʈ���� SoundManager.Instance.PlaySFX("ȿ�����̸�"); ���� ȣ���ϸ� ��

    public AudioClip[] bgmClips; // ���� BGM ����Ʈ
    public AudioClip[] sfxClips; // ��� ȿ���� ����Ʈ

    private void Awake()
    {
        Debug.Log("[SoundManager] Awake �����");

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("[SoundManager] �̱��� ���� �� ������");
        }
        else
        {
            Debug.Log("[SoundManager] ���� �̱����� �����Ͽ� ������");
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
        Debug.Log("[SoundManager] Start �����! �� �ε� �̺�Ʈ ����");

        SceneManager.sceneLoaded += OnSceneLoaded;

        // ���� ȣ���ؼ� `OnSceneLoaded()`�� ����Ǵ��� Ȯ��
        Debug.Log("[SoundManager] ���� ������ OnSceneLoaded ���� ����");
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"[SoundManager] �� �ε��: {scene.name}");

        switch (scene.name)
        {
            case "Login":
                Debug.Log("[SoundManager] Login �� BGM ���� �õ�");
                PlayBGM(0);
                break;
            case "Lobby":
                Debug.Log("[SoundManager] Lobby �� BGM ���� �õ�");
                PlayBGM(0);
                break;
            case "GamePlay":
                Debug.Log("[SoundManager] GamePlay �� BGM ���� �õ�");
                PlayBGM(0);
                break;
            case "DoorDash":
                Debug.Log("[SoundManager] DoorDash �� BGM ���� �õ�");
                PlayBGM(1);
                break;
            case "JumpClub":
                Debug.Log("[SoundManager] JumpClub �� BGM ���� �õ�");
                PlayBGM(2);
                break;
            case "ThinIce":
                Debug.Log("[SoundManager] ThinIce �� BGM ���� �õ�");
                PlayBGM(3);
                break;
            case "Tiptoe":
                Debug.Log("[SoundManager] Tiptoe �� BGM ���� �õ�");
                PlayBGM(4);
                break;
            case "WallParty":
                Debug.Log("[SoundManager] WallParty �� BGM ���� �õ�");
                PlayBGM(5);
                break;

            default:
                Debug.LogWarning("[SoundManager] �ش� ���� �´� BGM�� �����ϴ�!");
                break;
        }
    }


    public void PlayBGM(int index)
    {
        Debug.Log($"[SoundManager] PlayBGM ȣ���. �ε���: {index}");

        if (BGMSource == null)
        {
            Debug.LogError("[SoundManager] BGMSource�� �����ϴ�! AudioSource Ȯ�� �ʿ�");
            return;
        }

        if (index < 0 || index >= bgmClips.Length)
        {
            Debug.LogError($"[SoundManager] PlayBGM ����: ��ȿ���� ���� �ε��� {index}");
            return;
        }

        if (bgmClips[index] == null)
        {
            Debug.LogError($"[SoundManager] BGM Ŭ���� �����ϴ�! index: {index}");
            return;
        }

        BGMSource.clip = bgmClips[index];
        BGMSource.loop = true;
        BGMSource.Play();

        Debug.Log($"[SoundManager] BGM ��� ����: {bgmClips[index].name}");
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
//    // ���� ������ �� �ҷ����� (0~1 ���� ��)
//    bgmSlider.value = PlayerPrefs.GetFloat("BGMVolume", 1f);
//    sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);

//    bgmSlider.onValueChanged.AddListener(SetBGMVolume);
//    sfxSlider.onValueChanged.AddListener(SetSFXVolume);
//}

//public void SetBGMVolume(float volume)  // ���� �������
//{
//    audioMixer.SetFloat("BGM", Mathf.Log10(volume) * 20);
//    PlayerPrefs.SetFloat("BGMVolume", volume);
//}

//public void SetSFXVolume(float volume) // ���� ����
//{
//    audioMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
//    PlayerPrefs.SetFloat("SFXVolume", volume);
//}




//using UnityEngine;


//public class SoundManager : MonoBehaviour
//{
//    // ĳ������ ����� �ҽ��� ����
//    private AudioSource audioSource;

//    [Header("���� Ŭ������")]
//    [SerializeField] private AudioClip _footstepSound;
//    [SerializeField] private AudioClip _jumpSound;
//    [SerializeField] private AudioClip _landSound;
//    [SerializeField] private AudioClip _crackSound;
//    [SerializeField] private AudioClip _countDownSound;
//    [SerializeField] private AudioClip _countDown;
//    [SerializeField] private AudioClip _ladderCatch;
//    [SerializeField] private AudioClip _bgm;



//    private bool isGrounded = true;  //  ĳ���Ͱ� ���� �ִ��� üũ (�⺻�����̱⶧���� true)

//    public void Awake()
//    {
//        audioSource = GetComponent<AudioSource>();
//        if(audioSource == null)
//        {
//            Debug.LogError("AudioSource ������Ʈ�� ����ְų� �߰����� �ʾҽ��ϴ�.");
//        }
//    }

//    public void PlayCrakSound()
//    {
//        if(_crackSound != null)
//        {
//            audioSource.PlayOneShot(_crackSound); // ũ������ �� �� ���
//        }
//        else
//        {
//            Debug.LogWarning("ũ�� ���� Ŭ���� �������ּ���");
//        }
//    }

//    public void PlayCountDownSound() 
//    {
//        if (_countDownSound != null)
//        {
//            audioSource.PlayOneShot(_countDownSound); // ī��Ʈ�ٿ� ���� �� �� ���
//        }
//        else
//        {
//            Debug.LogWarning("ī��Ʈ�ٿ� ���� Ŭ���� �������ּ���");
//        }
//    }

//    public void PlayCountDownEffectSound()
//    {
//        if (_countDown != null)
//        {
//            audioSource.PlayOneShot(_countDown); // ī��Ʈ�ٿ� ȿ���� ���
//        }
//        else
//        {
//            Debug.LogWarning("ī��Ʈ�ٿ� ȿ������ �������ּ���");
//        }
//    }

//    public void PlayLadderSound()
//    {
//        if(_ladderCatch != null)
//        {
//            audioSource.PlayOneShot(_ladderCatch); // ��ٸ� ��� �Ҹ� �� �� ���
//        }
//        else
//        {
//            Debug.LogWarning("��ٸ� ��� ���� Ŭ���� �������ּ���");
//        }
//    }


//    public void PlayFootStepSound()
//    {
//        if(_footstepSound != null)
//        {
//            audioSource.PlayOneShot(_footstepSound); // �߼Ҹ��� �� �� ���
//        }
//        else
//        {
//            Debug.LogWarning("�߼Ҹ� ���� Ŭ���� �������ּ���");
//        }

//    }

//    public void PlayfJumpSound()
//    {
//        if(_jumpSound != null)
//        {
//            audioSource.PlayOneShot(_jumpSound); // �����Ҹ��� �� �� ���
//        }
//        else
//        {
//            Debug.LogWarning("���� ���� Ŭ���� �������ּ���");
//        }

//    }
//    public void PlayLandSound()
//    {
//        if (_landSound != null)
//        {
//            audioSource.PlayOneShot(_landSound); // �����Ҹ��� �� �� ���
//        }
//        else
//        {
//            Debug.LogWarning("���� ���� Ŭ���� �������ּ���");
//        }

//    }

//    public void SetGroundedState(bool grounded)
//    {
//        isGrounded = grounded;  
//    }


//}

