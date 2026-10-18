using UnityEngine;

namespace BaseCode.Utilities
{
    public class SinWaveSpawner : MonoBehaviour
    {
        public GameObject prefab;  // Assign the prefab in Inspector
        public int objectCount = 10;
        public float amplitude = 2f;
        public float frequency = 1f;
        public float rotationMultiplier = 30f;

        private void Start()
        {
            SpawnSineWave();
        }

        private void SpawnSineWave()
        {
            GameObject previousObject = null;

            for (int i = 0; i < objectCount; i++)
            {
                // Determine the X position based on the previous object's position
                float xPos = (previousObject != null) 
                    ? previousObject.transform.position.x + previousObject.GetComponent<Collider>().bounds.size.x 
                    : 0f;

                // Calculate the Y position using the sine function
                float yPos = Mathf.Sin(xPos * frequency) * amplitude;
                Vector3 position = new Vector3(xPos, yPos, 0);

                // Instantiate the new object
                GameObject obj = Instantiate(prefab, position, Quaternion.identity);

                // Rotate based on the sine wave slope
                float angle = Mathf.Cos(xPos * frequency) * rotationMultiplier;
                obj.transform.rotation = Quaternion.Euler(0, 0, angle);

                // Set the current object as the previous one for the next iteration
                previousObject = obj;
            }
        }
    }
}