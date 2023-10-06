using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimScript : MonoBehaviour
{
    Animator animator;
    PlayerCombat combat;
    PlayerMove move;
    public bool canMove;
    string currentState;

    private void Start()
    {
        animator = GetComponent<Animator>();
        combat = GetComponentInParent<PlayerCombat>();
        move = GetComponentInParent<PlayerMove>();
    }

    public void ChangeAnimationState(string newState)
    {
        if (newState == currentState) { return; }
        currentState = newState;
        animator.Play(newState);
    }

    public void CanPress()
    {
        combat.canPress = true;
    }
    public void CanMove()
    {
        switch (canMove)
        {
            case true:
                canMove = false;
                break;
            case false:
                canMove = true;
                break;
        }
    }
}
