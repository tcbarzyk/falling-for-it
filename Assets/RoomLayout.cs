using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomLayout : MonoBehaviour
{
    public Vector2 roomSize;
    //danger should be from 0 to 100
    public float danger = 0f;
    public int minPlatforms = 1;
    public bool onlySmallPlatforms = false;

    private Transform[] platformLocations;

    [Header("Platform Types")]
    public GameObject smallPlatform;
    public GameObject largePlatform;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(roomSize.x, roomSize.y, 1f));
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Transform parentPlatformLocations = transform.Find("PlatformLocations");

        for (int i = 0; i < parentPlatformLocations.childCount; i++)
        {
            Gizmos.DrawWireSphere(parentPlatformLocations.GetChild(i).position, 2f);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //get all potential platform locations
        Transform parentPlatformLocations = transform.Find("PlatformLocations");
        platformLocations = new Transform[parentPlatformLocations.childCount];

        for (int i = 0; i < parentPlatformLocations.childCount; i++)
        {
            platformLocations[i] = parentPlatformLocations.GetChild(i);
        }

        SpawnPlatforms();
    }

    private void SpawnPlatforms()
    {
        int numberOfPlatforms = Random.Range(minPlatforms, platformLocations.Length);
        Transform[] selectedLocations = SelectRandomItems(platformLocations, numberOfPlatforms);

        foreach (Transform location in selectedLocations)
        {
            GameObject platformToSpawn = smallPlatform;
            if (!onlySmallPlatforms) platformToSpawn = Random.value > 0.5f ? smallPlatform : largePlatform;
            GameObject platform = Instantiate(platformToSpawn, location.position, Quaternion.identity);
            platform.GetComponent<Platform>().danger = Mathf.Clamp(Random.Range(danger-20f, danger+20f), 0, 100);
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
