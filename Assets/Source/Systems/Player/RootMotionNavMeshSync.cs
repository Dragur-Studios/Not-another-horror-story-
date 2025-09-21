using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(Animator))]
public class RootMotionNavMeshSync : MonoBehaviour
{
    NavMeshAgent agent;
    Animator anim;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

    void OnAnimatorMove()
    {
        if (!anim.applyRootMotion) return;

        Vector3 proposedPosition = anim.rootPosition;
        proposedPosition.y = agent.nextPosition.y;

        if (NavMesh.SamplePosition(proposedPosition, out NavMeshHit hit, 0.5f, NavMesh.AllAreas))
        {
            transform.position = hit.position;
            agent.nextPosition = hit.position;
        }
        else
        {
            transform.position = agent.nextPosition;
        }
    }
}
