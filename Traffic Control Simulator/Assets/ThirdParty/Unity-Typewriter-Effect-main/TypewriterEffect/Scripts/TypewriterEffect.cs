using System;
using System.Collections;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class TypewriterEffect : MonoBehaviour
{
    [SerializeField] private TMP_Text textObject;
    [SerializeField] private float delayBeforeStart = 1f;
    [SerializeField] private float timeBtwChars = 0.1f;
    [SerializeField] private string leadingChar = "|";
    [SerializeField] private bool leadingCharBeforeDelay = false;
    
    private string _writer;

    public void StartTyping()
    {
        if (textObject != null)
        {
            _writer = textObject.text;
            textObject.text = leadingCharBeforeDelay ? leadingChar : "";

            StartCoroutine(nameof(TMPTypeWriter));
        }
        else
            textObject = GetComponent<TextMeshProUGUI>();
    }

    private IEnumerator TMPTypeWriter()
    {
        yield return new WaitForSeconds(delayBeforeStart);

        foreach (var c in _writer)
        {
            if (textObject.text.Length > 0)
                textObject.text = textObject.text[..^leadingChar.Length];
            
            textObject.text += c;
            textObject.text += leadingChar;
            RandomSound.Singleton.SetSourceClip(RandomSound.Singleton.audioClips[Random.Range(0, RandomSound.Singleton.audioClips.Length)]);
            yield return new WaitForSeconds(timeBtwChars);
        }

        if (leadingChar != "")
            textObject.text = textObject.text[..^leadingChar.Length];
    }
}
