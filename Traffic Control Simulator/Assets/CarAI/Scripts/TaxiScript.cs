using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaxiScript : MonoBehaviour
{
    public Transform startCheckpoint;
    public Transform endCheckpoint;
    public List<Transform> bestRoute;
    [Tooltip("The number of coroutines that are running from computing the path. If it's 0 then it means that the script is not computing. This can help you check when the script finished calculating.")]
    public int coroutines = 0;
    [Tooltip("The number of checkpoints that will be proccesed per coroutine. Increasing this number can make the game run slower depending on your pc specifications, but it will calculate the best route faster.")]
    public int iterations = 100;
    private CarAIController controller;
    
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CarAIController>();

        if(!controller.taxiMode)
        {
            Debug.LogWarning("Make sure to turn on the taxi mode form CarAIController in order for the car to follow the path.");
        }

        if(!controller.isCarControlledByAI)
        {
            Debug.LogWarning("Make sure to turn on the isCarControlledByAI form CarAIController in order for the car to follow the path.");
        }

        if(!controller.CheckPointSearch)
        {
            Debug.LogWarning("Make sure to turn on the CheckPointSearch form CarAIController in order for the car to follow the path.");
        }

        if(!controller)
        {
            Debug.LogError("The game object " + gameObject.name + " does not contain the CarAIController script.");
        }
    }

    public void ComputeRoute()
    {

        List<Transform> route = new List<Transform>() {startCheckpoint};

        StartCoroutine(Run(route));
        coroutines++;
    }

    private bool objectExistsInList(List<Transform> list, Transform obj)
    {
        for(int i = 0; i < list.Count; i++)
        {
            if(list[i] == obj)
            {
                return true;
            }
        } 
        return false;
    }

    IEnumerator Run(List<Transform> route)
    {
        Transform current = route[route.Count - 1];
        CheckpointScript script = current.gameObject.GetComponent<CheckpointScript>();

        bool run = true;

        if(script.nextCheckpoints.Count > 1)
        {
            run = false;
        }

        int iterations = 0;

        while(run)
        {
            iterations++;

            if(iterations % 100 == 0)
            {
                yield return 0;
            }

            current = route[route.Count - 1];
            script = current.gameObject.GetComponent<CheckpointScript>();
            
            if(script.nextCheckpoints.Count != 0)
            {
                current = script.nextCheckpoints[0];
                script = current.gameObject.GetComponent<CheckpointScript>();
            }

            if(current == endCheckpoint)
            {
                route.Add(current);

                if(bestRoute.Count == 0 || route.Count < bestRoute.Count)
                {
                    bestRoute = new List<Transform>(route);
                }

                coroutines--;
                yield break;
            }

            if(script.nextCheckpoints.Count == 0)
            {
                coroutines--;
                yield break;
            }

            if(script.nextCheckpoints.Count > 1)
            {
                break;
            }

            route.Add(current);
        }

        List<Transform> next = route[route.Count - 1].GetComponent<CheckpointScript>().nextCheckpoints;

        for(int i = 0; i < next.Count; i++)
        {
            List<Transform> localRoute = new List<Transform>(route);
            if(!objectExistsInList(localRoute, next[i]))
            {
                localRoute.Add(next[i]);
                StartCoroutine(Run(localRoute));
                coroutines++;
            }
        }

        coroutines--;
    }
}
