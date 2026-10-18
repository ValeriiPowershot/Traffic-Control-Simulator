using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

[DisallowMultipleComponent]
public class OnClickJump : MonoBehaviour, IPointerClickHandler
{

    public void OnPointerClick(PointerEventData eventData)
    {
        //OnMouseDown();
        //Click();
        //transform.DOPunchScale(Vector3.one * 0.2f, 0.3f, 8, 0.8f);
    }

    public void Click()
    {

    }


    void OnMouseDown()
    {
        transform.DOKill();

        transform
            .DOScale(0.85f, 0.1f)
            .OnComplete(() =>
            {
                transform.DOScale(1f, 0.25f)
                    .SetEase(Ease.OutBack);
            });
    }

}
