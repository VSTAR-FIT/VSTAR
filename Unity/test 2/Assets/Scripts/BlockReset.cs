using UnityEngine;

public class BlockReset : MonoBehaviour
{
    [Header("Reset Settings")]
    public Transform resetLocation;
    public float maxDistance = 10f;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate() // use FixedUpdate for physics
    {
        if (resetLocation == null)
        {
            Debug.LogWarning($"{name}: Reset location not set!");
            return;
        }

        float distance = Vector3.Distance(transform.position, resetLocation.position);
        // Debug.Log($"{name}: Distance from reset point = {distance}");

        if (distance > maxDistance)
        {
            Debug.Log($"{name}: Too far! Teleporting to reset location.");
            transform.position = resetLocation.position;

            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
    }
}
