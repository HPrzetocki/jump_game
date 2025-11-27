using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;

public class PlayerMovmentState : MonoBehaviour 
{


    public enum MoveState
    {
        Idle,
        Walk,
        Jump,
        Crouch,
    }
    public MoveState CurrentMoveState {get; private set;}

    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D rigidBody;


    private const string idleAnim = "Idle";
    private const string walkAnim = "Walk";
    private const string jumpAnim = "Jump";
    private const string crouchAnim = "Crouch";

    public static Action<MoveState> OnPlayerMoveStateChanged;

    private float xPosLastFrame;

    private void FixedUpdate()
    {
        // Sprawdzenie klawisza kucania
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            SetMoveState(MoveState.Crouch);
        }
        // Jeœli stoi w miejscu i nie skacze
        else if (Mathf.Abs(rigidBody.velocity.x) < 0.1f && rigidBody.velocity.y == 0)
        {
            SetMoveState(MoveState.Idle);
        }
        // Jeœli chodzi w lewo/prawo i nie skacze
        else if (Mathf.Abs(rigidBody.velocity.x) >= 0.1f && rigidBody.velocity.y == 0)
        {
            SetMoveState(MoveState.Walk);
        }
        xPosLastFrame = transform.position.x;
    }

    public void SetMoveState(MoveState moveState)
    {
        if (moveState == CurrentMoveState) return;

        switch (moveState)
        {
            case MoveState.Idle:
                HandleIdle();
                break;

            case MoveState.Walk:
                HandleWalk();
                break;

            case MoveState.Jump:
                HandleJump();
                break;

            case MoveState.Crouch:
                HandleCrouch();
                break;

            default:
                Debug.LogError($"Invalid movment state: {moveState}");
                break;
        }

        OnPlayerMoveStateChanged?.Invoke(moveState);
        CurrentMoveState = moveState;

    }
    private void HandleIdle()
    {
        animator.Play(idleAnim);
    }
    private void HandleWalk()
    {
        animator.Play(walkAnim);
    }
    private void HandleJump()
    {
        animator.Play(jumpAnim);
    }
    private void HandleCrouch()
    {
        animator.Play(crouchAnim);
    }
}
