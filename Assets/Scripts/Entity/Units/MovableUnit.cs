using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MovableUnit : Unit
{
    private NavMeshAgent _agent;

    // Start is called before the first frame update
    void Start()
    {
        _agent = GetComponentInChildren<NavMeshAgent>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public override bool IssueMoveToCommand(Vector3 _TargetLocation)
    {
        _agent.SetDestination(_TargetLocation);

        return true;
    }

    public override bool IssueAttackCommand(Entity _AttackTarget)
    {
        // When we are issued an attack command, we need to essentially set a move command to move towards the target, then once within 
        // weapon range we want to stop moving and start shooting.


        return true;
    }

    public override void CheckMoveCommandComplete()
    {
        if(Vector3.Distance(transform.position, _agent.destination) <= 0.2)
        {
            _currentOrder = null;
        }
    }
}
