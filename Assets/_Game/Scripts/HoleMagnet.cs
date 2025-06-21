using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleMagnet : MonoBehaviour
{
    [Header("References")]
    [SerializeField] PhysicMaterial lessFrictionMaterial;
    [SerializeField] CapsuleCollider capsuleCollider;
    [SerializeField] Hole hole;

    [Header("Properties")]
    [SerializeField] private float normalRadius = 0.5f;

    private void Awake()
    {
        capsuleCollider.radius = normalRadius;
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.attachedRigidbody.mass)
        {
            case 1:
                break;
            case 2:
                break;
            case 3:
                if (hole.Level < 2)
                    return;
                break;
            case 4:
                if (hole.Level < 4)
                    return;
                break;
        }

        other.material = lessFrictionMaterial;

        Rigidbody rb = other.attachedRigidbody;
        if (rb != null)
        {
            if (rb.isKinematic)
            {
                rb.isKinematic = false;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        other.material = null;
        Rigidbody rb = other.attachedRigidbody;
        if (rb != null)
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }
    }
}
