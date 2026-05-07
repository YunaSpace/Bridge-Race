using System;
using UnityEngine;
using UnityEngine.AI;

namespace YunaSpace.BridgeRace
{
    public class PlayerMovementHandler : MonoBehaviour
    {
        [SerializeField] private Player manager;

        [SerializeField] private float speed = 5f;
        [SerializeField] private float rotationSpeed = 10f;

        [SerializeField] private float checkDistance = 0.5f;
        [SerializeField] private LayerMask obstacleLayer;

        private Vector2 movementInput;

        private void Awake()
        {
            manager.Agent.updateRotation = false;
            manager.SetSpeed(speed);

            GlobalEvent.OnJoystickMoved += OnJoystickMoved;
        }

        private void OnDestroy()
        {
            GlobalEvent.OnJoystickMoved -= OnJoystickMoved;
        }

        private void Update()
        {
            if (manager.IsStumbling)
            {
                return;
            }

            var h = movementInput.x;
            var v = movementInput.y;

            var moveInput = new Vector3(h, 0, v).normalized;

            if (moveInput.magnitude > 0.01f)
            {
                var targetRotation = Quaternion.LookRotation(moveInput);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

                Vector3 finalMove = moveInput;

                if (Physics.Raycast(transform.position, moveInput, out RaycastHit hit, checkDistance, obstacleLayer))
                {
                    finalMove = Vector3.ProjectOnPlane(moveInput, hit.normal);
                }

                manager.Agent.velocity = finalMove * speed;

                manager.PlayAnimation(GlobalValue.AnimationRun);
            }
            else
            {
                manager.Agent.velocity = Vector3.zero;

                manager.PlayAnimation(GlobalValue.AnimationIdle);
            }
        }

        private void OnJoystickMoved(float horizontal, float vertical)
        {
            movementInput = new(horizontal, vertical);
        }
    }
}