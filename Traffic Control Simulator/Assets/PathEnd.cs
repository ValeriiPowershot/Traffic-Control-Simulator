using System;
using System.Collections;
using System.Collections.Generic;
using BaseCode;
using TMPro;
using UnityEngine;

[DisallowMultipleComponent]
public class PathEnd : MonoBehaviour
{
    [SerializeField] private ScoreText _scoreText;
    
    private void OnTriggerEnter(Collider other)
    {
        CarScoreValue carScoreValue = other.transform.GetComponentInParent<CarScoreValue>();
        
        ScoreData.Instance.Score += carScoreValue.CarScore;
        
        _scoreText.UpdateScoreText(ScoreData.Instance.Score);
        
        Destroy(other.transform.parent.gameObject);
    }
}
