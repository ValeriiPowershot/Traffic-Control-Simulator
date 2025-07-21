using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class carspawnerscript : MonoBehaviour
{

    [Tooltip("A list of car models that will be spawned randomly. This script will simply copy these cars so they must have the CarAIController script setup and isCarControlledByAI=false.")]
    public List<GameObject> cars = new List<GameObject>();
    [Tooltip("The number of cars that will be spawned.")]
    public int numberOfCarsToSpawn = 1;
    [Tooltip("If false the spawner won't spawn cars.")]
    public bool canSpawn = true;
    [Tooltip("The first checkpoint that the car(s) will be redirected to.")]
    public Transform startingCheckpoint;
    [Tooltip("Time interval between cars in seconds.")]
    public float timeIntervalBetweenCarsInSeconds = 0f;
    [Header("This will randomly assign the distance that the cars will keep \n from other objects or cars from min to max.")]
    public float distanceKeptMin = 2f;
    public float distanceKeptMax = 2f;
    [Header("This will randomly assign the driving recklessness threshold \n from min to max.")]
    public int recklessnessMin = 0;
    public int recklessnessMax = 0;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnCycle());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator SpawnCycle()
    {
        int index = 0;
        while(index < numberOfCarsToSpawn)
        {
            if(canSpawn)
            {
                GameObject model = cars[Random.Range(0, cars.Count)];
                GameObject newCar = Instantiate(model);
                newCar.transform.position = transform.position;
                newCar.transform.rotation = transform.rotation;

                CarAIController controller = newCar.GetComponent<CarAIController>();

                controller.CheckPointSearch = true;
                controller.isCarControlledByAI = true;
                controller.distanceFromObjects = Random.Range(distanceKeptMin, distanceKeptMax);
                controller.recklessnessThreshold = Random.Range(recklessnessMin, recklessnessMax);
                controller.nextCheckpoint = startingCheckpoint;

                index++;

                yield return new WaitForSeconds(timeIntervalBetweenCarsInSeconds);
            }
            else
            {
                yield return new WaitForSeconds(1f);
            }
        }

        yield return 0;
    }
    
    private void OnTriggerStay(Collider other)
    {
        if(other.GetComponent<CarAIController>())
        {
            canSpawn = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.GetComponent<CarAIController>())
        {
            canSpawn = true;
        }
    }
}
