using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductionStructure : StructureUnit
{
    public Transform SpawnLocation;

    [Tooltip("This is the list of units/upgrades which can be produced from this Structure.")]
    public List<ProductionProfile> AvailableProductions = new List<ProductionProfile>();
    public int ProductionQueueSize = 6;

    // Creating a queue which will store all of the units which we want to produce
    private Queue<ProductionProfile> _productionQueue = new Queue<ProductionProfile>();
    private ProductionProfile _currentProduction;

    protected override void Update()
    {
        base.Update();

        CheckProductionQueue();
    }


    protected override void CheckCommandCompletion()
    {
        switch (_currentOrder.OrderName)
        {
            case "move":
                _currentOrder.Parameters.TryGetValue("destination", out object location);

                IssueMoveToCommand((Vector3)location);
                break;
            default:
                break;
        }
    }

    public override void HandleCommand()
    {
        if (_isConstructed)
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

    public bool IssueProduceCommand(ProductionProfile _Profile)
    {
        // When we are issues a command, we want to try to add the profile to the queue
        if(_productionQueue.Count < ProductionQueueSize)
        {
            _productionQueue.Enqueue(_Profile);
        }
             
        return true;
    }

    public void CheckProduceCommandComplete()
    {
        _currentOrder = null;
    }

    private void CheckProductionQueue()
    {
        if (_currentProduction != null) return;

        StartCoroutine(StartProduction());
    }

    public IEnumerator StartProduction()
    {
        _currentProduction = _productionQueue.Dequeue();

        float buildPercentage = 0f;
        float timeElapsed = 0f;

        while (timeElapsed < _currentProduction.ProductionTime)
        {
            // Update the build percentage and UI
            buildPercentage = timeElapsed / _currentProduction.ProductionTime;

            //UserInterfaceManager.

            yield return new WaitForSeconds(0.5f);

            timeElapsed += 0.5f;
        }

        // Now that the time has elapsed, we can spawn in the entity and set a move command to move it from the spawn to the rally marker.
        Entity entity = EntityManager.Instance.SpawnEntity(_currentProduction.ProductionEntity, SpawnLocation.position, out bool spawnSuccess);

        if (spawnSuccess)
        {
            if (_rallyMarkerIsSet)
            {
                ((Unit)entity).IssueOrder(new EntityOrder
                {
                    OrderName = "move",
                    Parameters = new Dictionary<string, object> { { "destination", _rallyMarker } }
                });
            }
        }

        _currentProduction = null;
    }
}
