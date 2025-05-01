using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{
    public Slider bgmSlider;
    public Slider sfxSlider;

    private void Start()
    {
        // 슬라이더 기본값 75% 설정
        bgmSlider.value = 0.75f;
        sfxSlider.value = 0.75f;

        // 현재 SoundManager의 볼륨 설정
        SoundManager.Instance.SetBGMVolume(bgmSlider.value);
        SoundManager.Instance.SetSFXVolume(sfxSlider.value);

        // 슬라이더 값 변경 시 볼륨 조절
        bgmSlider.onValueChanged.AddListener((value) => SoundManager.Instance.SetBGMVolume(value));
        sfxSlider.onValueChanged.AddListener((value) => SoundManager.Instance.SetSFXVolume(value));
    }
}
