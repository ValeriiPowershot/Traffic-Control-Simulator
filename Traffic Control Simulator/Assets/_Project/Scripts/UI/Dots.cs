using UnityEngine;

public class Dots : MonoBehaviour
{
    [SerializeField] private LevelChanger levelChanger;
    [SerializeField] private GameObject dotPrefab;
    [SerializeField] private Transform dotsHolder;

    private Dot[] dots;

    private void Start()
    {
        CreateDots();
    }

    private void CreateDots()
    {
        int count = levelChanger.LevelCount();

        dots = new Dot[count];

        for (int i = 0; i < count; i++)
        {
            GameObject obj = Instantiate(dotPrefab, dotsHolder);
            dots[i] = obj.GetComponent<Dot>();
        }

        Refresh();
    }

    public void Refresh()
    {
        SetActive(levelChanger.CurrentIndex());
    }

    public void SetActive(int index)
    {
        if (dots == null || dots.Length == 0)
            return;

        for (int i = 0; i < dots.Length; i++)
        {
            if (i == index)
                dots[i].Activate();
            else
                dots[i].Deactivate();
        }
    }
}
