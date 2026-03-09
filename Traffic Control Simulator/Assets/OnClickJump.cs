using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

[DisallowMultipleComponent]
public class OnClickJump : MonoBehaviour, IPointerClickHandler
{

    public void OnPointerClick(PointerEventData eventData)
    {
        //OnMouseDown();
        transform.DOPunchRotation(new Vector3(0,0,25), 0.4f, 8, 1);
        //Click();
        //transform.DOPunchScale(Vector3.one * 0.2f, 0.3f, 8, 0.8f);
    }

    public void Click()
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


    // void OnMouseDown()
    // {
    //     Sequence seq = DOTween.Sequence();
    //
    //     seq.Append(transform.DOScale(1.2f, 0.15f));
    //     seq.Join(transform.DORotate(new Vector3(0,0,20), 0.15f));
    //
    //     seq.Append(transform.DOScale(1f, 0.2f));
    //     seq.Join(transform.DORotate(Vector3.zero, 0.2f));
    // }

}
