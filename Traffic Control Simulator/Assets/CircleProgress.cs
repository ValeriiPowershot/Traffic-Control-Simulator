using UnityEngine;

public class CircleProgress : MonoBehaviour
{
    [SerializeField] private GameObject[] circles;

    private int currentValue = 0;

    private void Start()
    {
        UpdateCircles();
    }

    public void Plus()
    {
        if (currentValue < circles.Length)
        {
            currentValue++;
            UpdateCircles();
        }
    }

    public void Minus()
    {
        if (currentValue > 0)
        {
            currentValue--;
            UpdateCircles();
        }
    }

    private void UpdateCircles()
    {
        for (int i = 0; i < circles.Length; i++)
        {
            circles[i].SetActive(i < currentValue);
        }
    }
}
