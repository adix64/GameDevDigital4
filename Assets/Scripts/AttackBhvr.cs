using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBhvr : StateMachineBehaviour
{
    public HumanBodyBones bone;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float t = stateInfo.normalizedTime;
        if (t > 0.035f && t < 0.2f)
        {//hitbox activ doar in framurile corespunzatoare atacului
            animator.GetBoneTransform(bone).GetComponent<SphereCollider>().enabled = true;
        }
        else
        {//hitbox inactiv
            animator.GetBoneTransform(bone).GetComponent<SphereCollider>().enabled = false;
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {//hitbox inactiv cand se iese din stare ca sa asigure collider inactiv
        animator.GetBoneTransform(bone).GetComponent<SphereCollider>().enabled = false;
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
