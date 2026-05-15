using UnityEngine;

[DisallowMultipleComponent]
public class Dots : MonoBehaviour
{
    [SerializeField] private LevelChanger _levelChanger;
    [SerializeField] private GameObject _dotPrefab;
    [SerializeField] private Transform _dotsHolder;

    private Dot[] _dots;
    private int _currentIndex;

    private void Start()
    {
        CreateDots();
    }

    private void CreateDots()
    {
        int levelCount = _levelChanger.LevelCount();

        _dots = new Dot[levelCount];

        for (int i = 0; i < levelCount; i++)
        {
            GameObject dot = Instantiate(_dotPrefab, _dotsHolder);
            _dots[i] = dot.GetComponent<Dot>();
        }

        ActivateDot(0);
    }

    public void ActivateNextDot()
    {
        _currentIndex++;

        if (_currentIndex >= _dots.Length)
            _currentIndex = 0;

        ActivateDot(_currentIndex);
    }

    public void ActivatePreviousDot()
    {
        _currentIndex--;

        if (_currentIndex < 0)
            _currentIndex = _dots.Length - 1;

        ActivateDot(_currentIndex);
    }

    public void ActivateDot(int index)
    {
        if (_dots == null || _dots.Length == 0)
            return;

        if (index < 0 || index >= _dots.Length)
            return;

        _currentIndex = index;

        DeactivateAllDots();

        _dots[index].Activate();
    }

    private void DeactivateAllDots()
    {
        foreach (Dot dot in _dots)
        {
            dot.Deactivate();
        }
    }
}
