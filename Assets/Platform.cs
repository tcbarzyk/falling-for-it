using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public float danger = 0f;
    public float baseHazardChance = 0.2f;
    public float dangerInfluenceFactor = 0.01f;
    public int maxObjects = 1;

    public GameObject basicEnemy;

    private Transform[] objectLocations;

    // Start is called before the first frame update
    void Start()
    {
        objectLocations = new Transform[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            objectLocations[i] = transform.GetChild(i);
        }

        int numberOfObjects = Random.Range(0, maxObjects+1);
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

        if (Random.value < hazardChance)
        {
            //spawn hazard
            print("spawning enemy");
            Instantiate(basicEnemy, location.position, Quaternion.identity);
        }
        else
        {
            //spawn other object
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
