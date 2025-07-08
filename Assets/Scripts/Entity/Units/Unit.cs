using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : Entity, IOrderableEntity
{
    public int StartingHitPoints = 100;

    public int RemainingHitPoints { get { return _remainingHitPoints; } }
    protected int _remainingHitPoints = 0;

    public bool StartsAtFullHealth = true;

    protected Queue<EntityOrder> _orders = new Queue<EntityOrder>();
    protected EntityOrder _currentOrder;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        if(StartsAtFullHealth)
        {
            _remainingHitPoints = StartingHitPoints;
        }
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if(_currentOrder == null && _orders.Count > 0)
        {
            _currentOrder = _orders.Dequeue();
        }

        if (_currentOrder == null) return;

        HandleCommand();

        CheckCommandCompletion();
    }

    public void IssueOrder(EntityOrder _Order)
    {     
        _orders.Enqueue( _Order );
    }

    public virtual void HandleCommand()
    {
        if (_currentOrder == null) return;

        switch (_currentOrder.OrderName)
        {
            case "moveTo":
                // When we recieve a move to order, we want set the navigation mesh agent's target
                _currentOrder.Parameters.TryGetValue("destination", out object location);

                IssueMoveToCommand((Vector3)location);
                break;

            default:
                Debug.LogWarning($"Unit {ID} does not have a handler for the current order type: {_currentOrder.OrderName}");
                _currentOrder = null;
                break;
        }
    }

    public virtual bool IssueMoveToCommand(Vector3 _TargetLocation)
    {
        return true;                                                                                                                                                              
    }

    public virtual bool IssueAttackCommand(Entity _AttackTarget)
    {
        return true;
    }

    protected virtual void CheckCommandCompletion()
    {
        switch (_currentOrder.OrderName)
        {
            case "moveTo":
                CheckMoveCommandComplete();
                break;
            default:
                break;
        }
    }

    public virtual void CheckMoveCommandComplete()
    {

    }

    public virtual void CheckAttackMoveCompleted()
    {

    }

    public virtual void TakeDamage(int _Damage)
    {
        _remainingHitPoints -= _Damage;

        _remainingHitPoints = Mathf.Clamp(_remainingHitPoints, 0, StartingHitPoints);

        UserInterfaceManager.DisplayUnitStatistics(this);

        if(_remainingHitPoints == 0)
        {
            UserInterfaceManager.DestroyUnitDisplay(this);
            Debug.Log("Unit Dead");
        }
    }
}
