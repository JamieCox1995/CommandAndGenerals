using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionUnit : MovableUnit
{
    public List<int> BuildOptions = new List<int>();

    public float ConstructionDelay = 1.5f;
    public int ConstructionAmountPerCycle = 20;

    private int _constructionEntityID = -1;
    private StructureUnit _structure;
    protected bool _constructionStarted = false;

    public override void HandleCommand()
    {
        if (_currentOrder == null || string.IsNullOrWhiteSpace(_currentOrder.OrderName)) return;

        switch (_currentOrder.OrderName)
        {
            case "moveTo":

                // A Move To command should have a destination parameter
                if (_currentOrder.Parameters.TryGetValue("destination", out object location))
                {
                    IssueMoveToCommand((Vector3)location);
                }
                
                break;

            case "build":
                // A Build command should contain 2 parameters, the location the structure is to be built at and the ID of the entity to be built.
                if(!_currentOrder.Parameters.ContainsKey("location") && !_currentOrder.Parameters.ContainsKey("structure"))
                {
                    Debug.LogWarning($"Unable to issue {_currentOrder.OrderName}, it does not have the correct parameters");
                    break;
                }

                // Otherwise we can issue the order. We basically want to
                Vector3 buildLocation = (Vector3)_currentOrder.Parameters["location"];
                int entityId = (int)_currentOrder.Parameters["EntityID"];
                _structure = (StructureUnit)_currentOrder.Parameters["structure"];

                IssueBuildCommand(buildLocation, entityId);

                break;

            default:
                Debug.LogWarning($"Unit {ID} does not have a handler for the current order type: {_currentOrder.OrderName}");
                break;
        }
    }

    protected override void CheckCommandCompletion()
    {
        switch (_currentOrder.OrderName)
        {
            case "moveTo":
                CheckMoveCommandComplete();
                break;
            case "build":
                CheckBuildComplete();
                break;
            default:
                break;
        }
    }

    public void IssueBuildCommand(Vector3 _BuildLocation, int _EntityToBuild)
    {
        // We want to set our target location, as well as setting the target constuction
        _targetLocation = _BuildLocation;
        _constructionEntityID = _EntityToBuild;
    }

    public void CheckBuildComplete()
    {
        if(_agent.destination != _targetLocation)
        {
            _agent.SetDestination(_targetLocation);
        }

        // First we need to make sure that we are not at the location requested
        if(Vector3.Distance(transform.position, _targetLocation) < 1.2f)
        {
            // If we are at the location, we then want to make sure that the building we are trying to build is not completed
            if(!_constructionStarted)
            {
                StartCoroutine(ConstructStructure());
            }
        }

    }

    private IEnumerator ConstructStructure()
    {
        _constructionStarted = true;

        // As we know the Structure should have been spawned we can start updating the hit points.
        while(_structure.RemainingHitPoints != _structure.StartingHitPoints)
        {
            yield return new WaitForSeconds(ConstructionDelay);
            _structure.TakeDamage(-ConstructionAmountPerCycle);
        }

        _constructionStarted = false;
        _currentOrder = null;
    }
}
