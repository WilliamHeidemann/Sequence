using System;
using UnityEngine;
using UnityEngine.UIElements;

public class AutoRotate : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private PanelRenderer _panelRenderer;

    private VisualElement _background;
    private VisualElement _circle;
    private float _currentAngle = 0f;

    private void OnEnable()
    {
        _panelRenderer.RegisterUIReloadCallback(Setup);
    }

    private void Setup(PanelRenderer panelRenderer, VisualElement rootElement, int version)
    {
        _background = rootElement.Q<VisualElement>("background");
        _circle = rootElement.Q<VisualElement>("circle");

        if (_background == null) Debug.LogError("AutoRotate requires background element");
        if (_circle == null) Debug.LogError("AutoRotate requires circle element");
    }

    void Update()
    {
        _currentAngle += _speed * Time.deltaTime;
        _currentAngle %= 360f;

        _background.style.rotate = new StyleRotate(new Rotate(new Angle(_currentAngle, AngleUnit.Degree)));
        _circle.style.rotate = new StyleRotate(new Rotate(new Angle((360f - _currentAngle) / 2f, AngleUnit.Degree)));
    }
}