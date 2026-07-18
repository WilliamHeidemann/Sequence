using LitMotion;
using LitMotion.Extensions;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class DiscardPile : MonoBehaviour
{
    [SerializeField] private UIDocument _discardPile;
    [SerializeField] private float _discardDuration = 0.5f;
    
    private void OnEnable()
    {
        _discardPile.rootVisualElement.Q<VisualElement>("Card").style.display = DisplayStyle.None;
    }

    public void Discard(Transform card)
    {
        var targetPosition = _discardPile.transform.position + new Vector3(0f, Random.Range(-0.05f, 0.05f), 0f);
        
        LMotion.Create(card.position, targetPosition, _discardDuration)
            .WithEase(Ease.InOutCubic)
            .BindToPosition(card);
        
        var targetRotation = Quaternion.Euler(0, 0, Random.Range(-10f, 10f)) * _discardPile.transform.rotation;
        
        LMotion.Create(card.rotation, targetRotation, _discardDuration)
            .WithEase(Ease.InOutCubic)
            .BindToRotation(card);
    }
}
