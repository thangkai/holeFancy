using UnityEngine;

public class DirectionArrow : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform holeTransform;
    [SerializeField] private GameObject arrowSprite;
    [SerializeField] private float arrowDistance = 0.5f;
    [SerializeField] private float rotationSmoothness = 5f;

    [Header("Input")]
    [SerializeField] private float inputThreshold = 0.1f;

    private Vector3 moveDirection;
    private Vector3 targetPosition;
    private Quaternion targetRotation;

    private void Start()
    {
        if (arrowSprite != null)
        {
            arrowSprite.SetActive(false);
        }
    }

    private void Update()
    {
        if (moveDirection.magnitude > inputThreshold)
        {
            if (!arrowSprite.activeSelf)
            {
                arrowSprite.SetActive(true);
            }

            Vector3 directionXZ = new Vector3(moveDirection.x, 0, moveDirection.z).normalized;
            targetPosition = holeTransform.position + directionXZ * arrowDistance;

            targetRotation = Quaternion.LookRotation(directionXZ, Vector3.up);

            arrowSprite.transform.position = Vector3.Lerp(
                arrowSprite.transform.position,
                targetPosition,
                Time.deltaTime * rotationSmoothness
            );

            arrowSprite.transform.rotation = Quaternion.Slerp(
                arrowSprite.transform.rotation,
                targetRotation,
                Time.deltaTime * rotationSmoothness
            );
        }
        else
        {
            if (arrowSprite.activeSelf)
            {
                arrowSprite.SetActive(false);
            }
        }
    }

    public void SetMoveDirection(Vector3 direction)
    {
        moveDirection = direction;
    }
}