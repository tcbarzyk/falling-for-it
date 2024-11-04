using UnityEngine;

public class DangerManager : MonoBehaviour
{
    public static DangerManager Instance { get; private set; }

    // Danger value is public and serialized so it can be edited in the Inspector
    [SerializeField]
    private float danger;

    public float Danger
    {
        get => danger;
        set => danger = Mathf.Max(value, 0); // Ensures danger doesn’t go below zero
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate instances
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Keeps this instance across scenes if needed
    }

    // Optional: Methods to modify danger
    public void IncreaseDanger(float amount)
    {
        Danger += amount;
    }

    public void DecreaseDanger(float amount)
    {
        Danger -= amount;
    }
}