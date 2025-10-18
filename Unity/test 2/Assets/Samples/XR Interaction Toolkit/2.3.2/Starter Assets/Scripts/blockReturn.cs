using UnityEngine;

public class BlockReset : MonoBehaviour
{
    [Tooltip("The location to teleport the block back to")]
    public Transform resetLocation;

    [Tooltip("Maximum allowed distance from the reset location")]
    public float maxDistance = 10f;

    void Update()
    {
        if (resetLocation == null)
        {
            Debug.LogWarning("Reset location not set!");
            return;
        }

        float distance = Vector3.Distance(transform.position, resetLocation.position);

        if (distance > maxDistance)
        {
            Debug.Log("Block too far, teleporting back...");
            transform.position = resetLocation.position;
            // Optional: reset velocity if using physics
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
    }
}

