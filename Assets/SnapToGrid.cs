using UnityEngine;

[ExecuteInEditMode]
public class SnapToGrid : MonoBehaviour
{
    public float snapValue = 25f; // Value to snap to (e.g., 1 unit)

    private void Update()
    {
        if (!Application.isPlaying)
        {
            SnapPosition();
        }
    }

    private void SnapPosition()
    {
        Vector3 position = transform.position;
        position.x = Mathf.Round(position.x / snapValue) * snapValue;
        position.y = Mathf.Round(position.y / snapValue) * snapValue;
        position.z = Mathf.Round(position.z / snapValue) * snapValue;
        transform.position = position;
    }
}