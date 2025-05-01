using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{
    public Slider bgmSlider;
    public Slider sfxSlider;

    private void Start()
    {
        // �����̴� �⺻�� 75% ����
        bgmSlider.value = 0.75f;
        sfxSlider.value = 0.75f;

        // ���� SoundManager�� ���� ����
        SoundManager.Instance.SetBGMVolume(bgmSlider.value);
        SoundManager.Instance.SetSFXVolume(sfxSlider.value);

        // �����̴� �� ���� �� ���� ����
        bgmSlider.onValueChanged.AddListener((value) => SoundManager.Instance.SetBGMVolume(value));
        sfxSlider.onValueChanged.AddListener((value) => SoundManager.Instance.SetSFXVolume(value));
    }
}
