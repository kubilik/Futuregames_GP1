using UnityEngine;

public class testsound : MonoBehaviour
{
    [SerializeField] private AudioClip test;
    public void PlaySound()
    {
        SoundEffectsManager.instance.PlaySoundEffects(test, transform, 1f);
    }
}
