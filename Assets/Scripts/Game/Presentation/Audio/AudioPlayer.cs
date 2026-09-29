using UnityEngine;

namespace Game.Presentation.Audio
{
    public class AudioPlayer : MonoBehaviour
    {
        [SerializeField] private AudioContainer _audioContainer;
        [SerializeField] private AudioSource _audioSourceTemplate; // Optional reference, or attach an AudioSource to this GameObject

        private void Awake()
        {
            if (_audioSourceTemplate == null)
            {
                _audioSourceTemplate = GetComponent<AudioSource>();
            }
        }

        public void Play(Sound sound)
        {
            var (audioClip, pitch) = _audioContainer.Get(sound);
            if (audioClip == null) return;

            // Spawn an independent one-shot GameObject to isolate the pitch
            GameObject soundObj = new GameObject($"Audio_{sound}");
            soundObj.transform.SetParent(transform);

            AudioSource source = soundObj.AddComponent<AudioSource>();
            
            // Copy base properties if a template exists
            if (_audioSourceTemplate != null)
            {
                source.outputAudioMixerGroup = _audioSourceTemplate.outputAudioMixerGroup;
                source.volume = _audioSourceTemplate.volume;
                source.spatialBlend = _audioSourceTemplate.spatialBlend;
            }

            source.clip = audioClip;
            source.pitch = pitch;
            source.Play();

            // Destroy the temporary instance after the clip finishes playing
            Destroy(soundObj, (audioClip.length / Mathf.Abs(pitch))+ 0.1f);
        }

        public async Awaitable Play(Sound sound, float delay)
        {
            await Awaitable.WaitForSecondsAsync(delay);
            Play(sound);
        }
    }
}