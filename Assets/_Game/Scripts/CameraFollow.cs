using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Follow Settings")]
    public Transform target;
    public Vector3 offset = new Vector3(0, 5, -10);

    [Header("Smooth Settings")]
    [Range(0.1f, 10f)]
    public float followSpeed = 2f;

    [Header("Scale Settings")]
    [Range(0.1f, 5f)]
    public float scaleTransitionSpeed = 2f;

    private Vector3 velocity = Vector3.zero;
    private Vector3 baseOffset;
    private float currentScale = 1f;
    private float targetScale = 1f;
    private bool isScaling = false;

    void Start()
    {
        baseOffset = offset;
    }

    void FixedUpdate()
    {
        HandleScaling();
        FollowTarget();
    }

    void HandleScaling()
    {
        if (isScaling)
        {
            currentScale = Mathf.Lerp(currentScale, targetScale, scaleTransitionSpeed * Time.deltaTime);

            offset = baseOffset * currentScale;

            if (Mathf.Abs(currentScale - targetScale) < 0.01f)
            {
                currentScale = targetScale;
                isScaling = false;
            }
        }
    }

    void FollowTarget()
    {
        if (target == null) return;

        Vector3 targetPosition = target.position + offset;
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref velocity,
            1f / followSpeed
        );
    }

    public void ScaleUpSmooth(float scale)
    {
        targetScale = scale;
        isScaling = true;
    }
}
