using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Mixer")]
    [SerializeField]
    private AudioMixer _audioMixer; // Referencia al AudioMixer

    [Header("Sliders")]
    [SerializeField]
    private Slider _masterVolumeSlider;
    [SerializeField]
    private Slider _musicVolumeSlider;
    [SerializeField]
    private Slider _sfxVolumeSlider;

    private void Start()
    {
        // Inicializar los sliders con los valores actuales del AudioMixer
        float masterVolume, musicVolume, sfxVolume;
        _audioMixer.GetFloat("MasterVolume", out masterVolume);
        _audioMixer.GetFloat("MusicVolume", out musicVolume);
        _audioMixer.GetFloat("SFXVolume", out sfxVolume);

        _masterVolumeSlider.value = Mathf.Pow(10, masterVolume / 20); // Convertir de dB a escala 0-1
        _musicVolumeSlider.value = Mathf.Pow(10, musicVolume / 20);
        _sfxVolumeSlider.value = Mathf.Pow(10, sfxVolume / 20);

        // Asignar los eventos a los sliders
        _masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        _musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        _sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    public void SetMasterVolume(float value)
    {
        // Convertir de escala 0-1 a dB y asignar al AudioMixer
        _audioMixer.SetFloat("MasterVolume", Mathf.Log10(value) * 20);
    }

    public void SetMusicVolume(float value)
    {
        _audioMixer.SetFloat("MusicVolume", Mathf.Log10(value) * 20);
    }

    public void SetSFXVolume(float value)
    {
        _audioMixer.SetFloat("SFXVolume", Mathf.Log10(value) * 20);
    }

    private void OnDestroy()
    {
        // Eliminar los listeners al destruir el objeto
        _masterVolumeSlider.onValueChanged.RemoveListener(SetMasterVolume);
        _musicVolumeSlider.onValueChanged.RemoveListener(SetMusicVolume);
        _sfxVolumeSlider.onValueChanged.RemoveListener(SetSFXVolume);
    }
}
