using System;
using UnityEngine;
using UnityEngine.UIElements;

public class WaveFormer : MonoBehaviour
{
    [SerializeField] private UIDocument _hand;

    private VisualElement _handContainer;
    
    private void OnEnable()
    {
        _handContainer = _hand.rootVisualElement.Q<VisualElement>("Row");
        print(_handContainer.childCount);
    }

    private void Update()
    {
        foreach (VisualElement visualElement in _handContainer.Query<VisualElement>().ToList())
        {
            float wave = Mathf.Sin(Time.time * 2f) * 10f;

            visualElement.style.translate = new Translate(
                new Length(0, LengthUnit.Pixel), 
                new Length(wave, LengthUnit.Pixel), 
                0
            );
        }
    }
}
