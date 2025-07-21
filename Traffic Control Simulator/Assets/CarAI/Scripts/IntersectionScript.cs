using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntersectionScript : MonoBehaviour
{
    public List<GameObject> stops = new List<GameObject>();

    [Tooltip("The time a stop is green in seconds.")]
    public float wait = 5f;

    private int index = 0;

    private bool next = true;

    private List<StopScript> scripts = new List<StopScript>();

    void Awake()
    {
        for(int i = 0; i < stops.Count; i++)
        {
            StopScript script = stops[i].GetComponent<StopScript>();
            script.stop = true;
            scripts.Add(script);
            stops[i].GetComponent<MeshRenderer>().material.color = Color.red;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(next)
        {
            index++;
            if(index >= stops.Count)
            {
                index = 0;
            }
            StartCoroutine(Cycle());
        }
    }

    IEnumerator Cycle()
    {
        next = false;

        stops[index].GetComponent<MeshRenderer>().material.color = Color.green;
        scripts[index].stop = false;

        yield return new WaitForSeconds(wait);

        while(scripts[index].priority > 0)
        {
            yield return new WaitForSeconds(0.1f);
        }

        stops[index].GetComponent<MeshRenderer>().material.color = Color.red;
        scripts[index].stop = true;

        next = true;
    }

    
}
