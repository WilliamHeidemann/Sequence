using UnityEngine;
using UnityEngine.UIElements;

public class DiscardPile : MonoBehaviour
{
    [SerializeField] private UIDocument _uiDocument;
    
    private void OnEnable()
    {
        _uiDocument.rootVisualElement.Q<VisualElement>("Card").style.display = DisplayStyle.None;
    }
}
