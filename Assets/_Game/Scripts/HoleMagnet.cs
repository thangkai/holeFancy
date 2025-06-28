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
    
    [SerializeField] private float suctionForce = 10f; // Cường độ lực hút
    [SerializeField] private float maxDistance = 5f; // Khoảng cách tối đa để lực hút có hiệu
    private void OnTriggerStay(Collider other)
    {
        // Kiểm tra điều kiện để áp dụng lực hút
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
            default:
                return;
        }

        // Lấy Rigidbody của vật thể
        Rigidbody rb = other.attachedRigidbody;
        if (rb != null)
        {
            // Tính toán hướng và khoảng cách từ vật thể đến trung tâm hố
            Vector3 directionToHole = transform.position - other.transform.position;
            float distance = directionToHole.magnitude;

            // Chỉ áp dụng lực nếu vật thể nằm trong khoảng cách tối đa
            if (distance < maxDistance)
            {
                // Chuẩn hóa hướng và tính lực hút
                Vector3 force = directionToHole.normalized * suctionForce * (1 - distance / maxDistance);
                // Nhân với level để lực hút mạnh hơn ở level cao
                force *= (1 + hole.Level * 0.5f);

                // Áp dụng lực
                rb.AddForce(force / rb.mass, ForceMode.Force);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.LogError("hole magnet");
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
