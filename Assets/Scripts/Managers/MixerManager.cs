using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Audio;

public class MixerManager : MonoBehaviour
{
    [SerializeField] public AudioMixer audioMixer;

    public void SoundVolume(float level)
    {
        audioMixer.SetFloat("SoundsFXVolume", Mathf.Log10(level) * 20f);
    }
    public void MusicVolume(float level)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(level) * 20f);
    }
    public void MasterVolume(float level)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(level) * 20f);
    }
}
