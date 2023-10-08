using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AttackBehave : StateMachineBehaviour
{
    PlayerMove move;
    PlayerCombat combat;
    AnimScript animScript;
    public BaseAttack attack;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(animator == null || combat == null || move == null)
        {
            animScript = animator.GetComponent<AnimScript>();
            combat = animator.GetComponentInParent<PlayerCombat>();
            move = animator.GetComponentInParent<PlayerMove>();
        }
        combat.canPress = false;
        combat.punchPresses = 0;
        combat.kickPresses = 0;
        animScript.canTransition = false;
        move.state = PlayerMove.State.Attacking;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!animScript.canTransition) { return; }

        if(combat.punchPresses > 0 && combat.kickPresses == 0)
        {
            animScript.ChangeAnimationState(attack.nextOptions[0].stateName);
        }
        else if(combat.punchPresses == 0 && combat.kickPresses > 0)
        {
            animScript.ChangeAnimationState(attack.nextOptions[3].stateName);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }
}
