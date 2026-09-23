using System;
using UnityEngine;
using UtilityToolkit.CollectionExtensions;

namespace Game.Presentation.Audio
{
    [CreateAssetMenu(fileName = "Audio Container", menuName = "Audio/Audio Container")]
    public class AudioContainer : ScriptableObject
    {
        [SerializeField] private AudioClip[] _drawCard;
        [SerializeField] private AudioClip[] _onClick;
        [SerializeField] private AudioClip[] _pop;
        [SerializeField] private AudioClip[] _putDown;
        [SerializeField] private AudioClip[] _toHand;
        
        public AudioClip Get(Sound sound)
        {
            AudioClip[] clips = sound switch
            {
                Sound.DrawCard => _drawCard,
                Sound.Click => _onClick,
                Sound.Pop => _pop,
                Sound.PutDown => _putDown,
                Sound.ToHand => _toHand,
                _ => throw new ArgumentOutOfRangeException(nameof(sound), sound, null)
            };

            return clips.RandomElement();
        }
    }

    public enum Sound
    {
        DrawCard,
        Click,
        Pop,
        PutDown,
        ToHand
    }
}