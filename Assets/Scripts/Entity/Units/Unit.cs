using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : Entity, IOrderableEntity
{
    protected Queue<EntityOrder> _orders = new Queue<EntityOrder>();
    protected EntityOrder _currentOrder;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if(_currentOrder == null && _orders.Count > 0)
        {
            Debug.Log("Getting next order");
            _currentOrder = _orders.Dequeue();
        }

        if (_currentOrder == null) return;

        switch(_currentOrder.OrderName)
        {
            case "moveTo":
                // When we recieve a move to order, we want set the navigation mesh agent's target
                string[] loc = _currentOrder.OrderParameters.Split(',');
                Vector3 targetLoc = new Vector3
                {
                    x = (float)Convert.ToDecimal(loc[0].Replace("(", "")),
                    y = (float)Convert.ToDecimal(loc[1]),
                    z = (float)Convert.ToDecimal(loc[2].Replace(")", ""))
                };

                IssueMoveToCommand(targetLoc);
                break;

            default:
                Debug.LogWarning($"Unit {ID} does not have a handler for the current order type: {_currentOrder.OrderName}");
                break;
        }

        CheckCommandCompletion();
    }

    public void IssueOrder(EntityOrder _Order)
    {
        Debug.Log($"Passed Order: \n{_Order.OrderName}, {_Order.OrderParameters}");
        _orders.Enqueue( _Order );
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
}
