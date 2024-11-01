using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public float danger = 0f;
    public float baseHazardChance = 0.2f;
    public float dangerInfluenceFactor = 0.01f;
    public int maxObjects = 1;
    public float objectOffset = 2f;
    public float chanceToSpawnNothing = 0.3f;

    private Transform[] objectLocations;

    public GameObject[] hazardObjects;
    public GameObject[] platformObjects;

    private bool alreadySpawnedEnemy = false;

    // Start is called before the first frame update
    void Start()
    {
        Transform parentObjectLocations = transform.Find("ObjectLocations");
        objectLocations = new Transform[parentObjectLocations.childCount];

        for (int i = 0; i < parentObjectLocations.childCount; i++)
        {
            objectLocations[i] = parentObjectLocations.GetChild(i);
        }

        int numberOfObjects = 0;

        if (Random.value < chanceToSpawnNothing)
        {
            numberOfObjects = 0;
        }
        else
        {
            numberOfObjects = Random.Range(1, maxObjects + 1);
        }

        print("num of obj: " + numberOfObjects);
        Transform[] selectedLocations = SelectRandomItems(objectLocations, numberOfObjects);

        foreach (Transform location in selectedLocations) {
            SpawnObject(location);
        }
    }

    private void SpawnObject(Transform location)
    {
        float hazardChance = baseHazardChance + (danger * dangerInfluenceFactor);
        hazardChance = Mathf.Clamp(hazardChance, 0f, 1f);

        if (Random.value < hazardChance && !alreadySpawnedEnemy)
        {
            //spawn hazard
            print("spawning hazard");
            GameObject objectToSpawn = hazardObjects[Random.Range(0, hazardObjects.Length)];
            GameObject hazard = Instantiate(objectToSpawn, location.position, Quaternion.identity);
            if (hazard.GetComponent<BasicEnemyMovement>())
            {
                alreadySpawnedEnemy = true;
                hazard.GetComponent<BasicEnemyMovement>().speed = Random.Range(2.5f, 3.5f);
            }
        }
        else
        {
            GameObject objectToSpawn = platformObjects[Random.Range(0, platformObjects.Length)];
            Instantiate(objectToSpawn, new Vector3(location.position.x, location.position.y-objectOffset, location.position.z-1), Quaternion.identity);
        }
    }

    // Helper method to select n random items from an array
    private T[] SelectRandomItems<T>(T[] array, int n)
    {
        if (n > array.Length)
            throw new System.ArgumentException("n cannot be greater than the array length.");

        System.Random rng = new System.Random();
        HashSet<int> selectedIndices = new HashSet<int>();
        T[] result = new T[n];

        int count = 0;
        while (count < n)
        {
            int index = rng.Next(array.Length);
            if (selectedIndices.Add(index)) // Add returns false if index already exists
            {
                result[count] = array[index];
                count++;
            }
        }

        return result;
    }
}
