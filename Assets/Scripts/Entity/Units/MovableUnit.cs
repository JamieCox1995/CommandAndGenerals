using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MovableUnit : Unit
{
    public float Speed = 5f;

    protected NavMeshAgent _agent;
    protected Vector3 _targetLocation;

    // Start is called before the first frame update
    void Start()
    {
        _agent = GetComponentInChildren<NavMeshAgent>();
        _agent.speed = Speed;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public override bool IssueMoveToCommand(Vector3 _TargetLocation)
    {
        _targetLocation = _TargetLocation;

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
        if(_agent.destination != _targetLocation)
        {
            _agent.destination = _targetLocation;
        }

        // Do Move Logic in Here?


        if(Vector3.Distance(transform.position, _targetLocation) <= 0.2)
        {
            _currentOrder = null;
        }
    }
}
