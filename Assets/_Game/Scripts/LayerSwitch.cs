using UnityEngine;

public class LayerSwitch : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] string enterLayer;
    [SerializeField] string exitLayer;

    private void OnTriggerEnter(Collider other)
    {
        other.gameObject.layer = LayerMask.NameToLayer(enterLayer);
    }

    private void OnTriggerExit(Collider other)
    {
        other.gameObject.layer = LayerMask.NameToLayer(exitLayer);
    }
}
