using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StructureUnit : Unit
{

    private bool _isConstructed = false;

    [SerializeField] private GameObject _rallyMarker;

    public override void HandleCommand()
    {
        if(_isConstructed)
        {
            base.HandleCommand();
        }
    }

    public override bool IssueMoveToCommand(Vector3 _TargetLocation)
    {
        // Not all Structures may need a rally marker, so if they do not have one set we will just return from the method.
        if (_rallyMarker == null) return true;

        //TODO: We should check that the _TargetLocation is a valid location to set the Rally Marker to.
        _rallyMarker.transform.position = _TargetLocation;
        return true;
    }

    public override void CheckMoveCommandComplete()
    {
        _currentOrder = null;
    }

    public override void TakeDamage(int _Damage)
    {
        base.TakeDamage(_Damage);

        if(!_isConstructed && _remainingHitPoints == StartingHitPoints)
        {
            _isConstructed = true;
        }
    }
}
