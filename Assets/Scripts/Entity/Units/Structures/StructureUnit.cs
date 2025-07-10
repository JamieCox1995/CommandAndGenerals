using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StructureUnit : Unit
{
    private bool _isConstructed = false;

    [SerializeField] private GameObject _rallyMarker;

    protected override void Start()
    {
        base.Start();


        if(!_isConstructed)
        {
            UserInterfaceManager.ShowConstructionDisplay(this);
        }
    }

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
        _remainingHitPoints -= _Damage;

        _remainingHitPoints = Mathf.Clamp(_remainingHitPoints, 0, StartingHitPoints);

        if (_remainingHitPoints == 0)
        {
            Debug.Log("Unit Dead");
        }

        if (!_isConstructed)
        {
            if(_remainingHitPoints == StartingHitPoints)
            {
                UserInterfaceManager.DestroyConstructionDisplay(this);
                _isConstructed = true;
            }
            else
            {
                UserInterfaceManager.UpdateConstructionDisplay(this);
            }
        }
    }
}
