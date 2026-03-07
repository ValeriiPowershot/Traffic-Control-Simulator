using UnityEngine;

[DisallowMultipleComponent]
public class Dots : MonoBehaviour
{
    [SerializeField] private CarouselUI _carouselUI;
    [SerializeField] private GameObject _dotPrefab;
    [SerializeField] private Transform _dotsHolder;

    private Dot[] _dots;

    private void Start() =>
        CreateDots();

    private void CreateDots()
    {
        _dots = new Dot[_carouselUI.GetImagesCount()];

        for (int i = 0; i < _carouselUI.GetImagesCount(); i++)
        {
            GameObject dot = Instantiate(_dotPrefab, _dotsHolder);
            _dots[i] = dot.GetComponent<Dot>();
        }

        DeativateAllDots();

        _dots[0].Activate();
    }

    public void ActivateDot(int index)
    {
        DeativateAllDots();
        _dots[index].Activate();
    }

    private void DeativateAllDots()
    {
        foreach (Dot dot in _dots)
            dot.Deactivate();
    }
}
