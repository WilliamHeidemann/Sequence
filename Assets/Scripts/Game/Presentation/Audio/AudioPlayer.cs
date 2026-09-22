using Game.Domain;
using UnityEngine;

namespace Game.Presentation
{
    public class AudioPlayer : MonoBehaviour
    {
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioContainer _audioContainer;

        public void Play(Sound sound)
        {
            AudioClip audioClip = _audioContainer.Get(sound);
            _audioSource.pitch = Random.Range(0.9f, 1.1f);
            _audioSource.PlayOneShot(audioClip);
        }

        public async Awaitable Play(Sound sound, float delay)
        {
            await Awaitable.WaitForSecondsAsync(delay);
            Play(sound);
        }
    }
}