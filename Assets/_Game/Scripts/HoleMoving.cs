using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleMoving : MonoBehaviour
{
    [SerializeField] FloatingOnScreenStick floatingJoystick;
    [SerializeField] float baseSpeed = 5f;
    [SerializeField] Rigidbody rb;
    [SerializeField] DirectionArrow directionArrow;

    private float moveSpeed;

    private void Start()
    {
        ChangeSpeedReferToScale(1);
    }

    public void MoveOnJoystick()
    {
        Vector3 targetMovement = new Vector3(floatingJoystick.Input.x, 0f, floatingJoystick.Input.y);

        targetMovement.Normalize();

        directionArrow.SetMoveDirection(targetMovement);
        Moving(targetMovement);
    }

    public void ChangeSpeedReferToScale(float speedScale)
    {
        moveSpeed = baseSpeed * speedScale;
    }

    private void Moving(Vector3 movement)
    {
        if (movement == Vector3.zero)
            return;

        Vector3 newPosition = transform.position + moveSpeed * Time.deltaTime * movement;
        rb.MovePosition(newPosition);
    }
}
