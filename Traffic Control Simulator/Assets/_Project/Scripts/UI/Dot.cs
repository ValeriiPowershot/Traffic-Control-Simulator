using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Dot : MonoBehaviour
{
    private Image _image;
    private Color32  _activeColor = new(157, 157, 157, 255);
    private Color32  _defaultColor = new(209, 209, 209, 255);

    private void Awake()
    {
        _image = GetComponent<Image>();
    }

    public void Activate()
    {
        _image.color = _activeColor;
        _image.transform.DOScale(1.3f, 0.8f).SetEase(Ease.OutQuint);
    }

    public void Deactivate()
    {
        _image.color = _defaultColor;
        _image.transform.DOScale(1f, 0.8f).SetEase(Ease.OutSine);
    }
}
