using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolBehaviour : StateMachineBehaviour
{
    public float startWaitTime = 3.5f;
    private float waitTime;
    public float speed;

    private GameObjectFinder objFinder;
    private MoveSpot moveSpot;
    private Transform moveSpotPos;

    //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //waitTime = startWaitTime;
        //objFinder = animator.GetComponentInParent<GameObjectFinder>();
        //moveSpotPos = objFinder.actors[0].transform;
    }

    //OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //if (Input.GetKeyDown(KeyCode.Alpha9))
        //{
        //    animator.SetBool("isPatrolling", false);
        //}

        //moveSpotPos = objFinder.actors[0].transform;
        //animator.SetFloat("Horizontal", moveSpotPos.position.x - animator.transform.position.x);
        //animator.SetFloat("Vertical", moveSpotPos.position.y - animator.transform.position.y);

        //if ((moveSpotPos.position.x - animator.transform.position.x) > 0) // Face Right
        //{
        //    animator.GetComponent<SpriteRenderer>().flipX = true;
        //}
        //else                                                              // Face Left
        //{
        //    animator.GetComponent<SpriteRenderer>().flipX = false;
        //}
    }

    //OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

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
