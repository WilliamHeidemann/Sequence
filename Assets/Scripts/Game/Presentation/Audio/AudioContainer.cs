using System;
using UnityEngine;
using UtilityToolkit.CollectionExtensions;

namespace Game.Presentation.Audio
{
    [Serializable]
    public class SoundConfig
    {
        [SerializeField] private AudioClip[] _clips = Array.Empty<AudioClip>();
        [SerializeField] private bool _randomizePitch = true;
        [SerializeField] private Vector2 _pitchRange = new(0.9f, 1.1f);

        public (AudioClip clip, float pitch) Resolve()
        {
            if (_clips == null || _clips.Length == 0)
            {
                return (null, 1f);
            }

            AudioClip clip = _clips.RandomElement();
            float pitch = _randomizePitch 
                ? UnityEngine.Random.Range(_pitchRange.x, _pitchRange.y) 
                : 1f;

            return (clip, pitch);
        }
    }

    [CreateAssetMenu(fileName = "Audio Container", menuName = "Audio/Audio Container")]
    public class AudioContainer : ScriptableObject
    {
        [SerializeField] private SoundConfig _drawCard;
        [SerializeField] private SoundConfig _onClick;
        [SerializeField] private SoundConfig _pop;
        [SerializeField] private SoundConfig _putDown;
        [SerializeField] private SoundConfig _toHand;
        [SerializeField] private SoundConfig _yes;
        [SerializeField] private SoundConfig _no;

        public (AudioClip clip, float pitch) Get(Sound sound)
        {
            SoundConfig config = sound switch
            {
                Sound.DrawCard => _drawCard,
                Sound.Click => _onClick,
                Sound.Pop => _pop,
                Sound.PutDown => _putDown,
                Sound.ToHand => _toHand,
                Sound.Yes => _yes,
                Sound.No => _no,
                _ => throw new ArgumentOutOfRangeException(nameof(sound), sound, null)
            };

            return config.Resolve();
        }
    }
}