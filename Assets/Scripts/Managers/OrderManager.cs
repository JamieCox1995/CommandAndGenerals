using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderManager : MonoBehaviour
{
    public static OrderManager Instance;
    private EntityOrder _selectedOrder;

    // Start is called before the first frame update
    void Start()
    {
        SetupInstance();
    }

    // Update is called once per frame
    void Update()
    {
        // Initiating a Build Action.
        if(Input.GetKeyDown(KeyCode.B))
        {
            _selectedOrder = new EntityOrder
            {
                OrderName = "build",
                Parameters = new Dictionary<string, object>
                {
                    { "EntityID", 1 }
                }
            };
        }


        if(Input.GetMouseButtonDown(1))
        {
            IssueOrderToSelection();
        }
    }

    private void IssueOrderToSelection()
    {
        // Getting the list of selected entities from the Selection Manager
        List<Unit> selectedEntities = SelectionManager.GetOrderableUnitsFromSelection();

        if (selectedEntities.Count <= 0) return;

        // Now we what we have clicked on to determine the type of action that needs to passed out
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitData;

        // Firing the Raycast
        if(Physics.Raycast(ray, out hitData))
        {
            // Updated the Order Logic, so that we take in to account selecting an Order from the UI
            if(_selectedOrder == null)
            {
                // If the Selected Order is null, we should do something similar to what we were doing before. This is so that we can assume
                // default orders depending on what we have clicked.
                // Try to see if what was hit is an entity.
                if (hitData.collider.GetComponentInParent<Entity>())
                {

                }
                else
                {

                    Vector3 targetLocation = hitData.point;
                    EntityOrder moveOrder = new EntityOrder
                    {
                        OrderName = "moveTo",
                        Parameters = new Dictionary<string, object>
                        {
                            { "destination", targetLocation }
                        },

                        OrderPriority = 1
                    };

                    foreach (Unit u in selectedEntities)
                    {
                        u.IssueOrder(moveOrder);
                    }
                }
            }
            else
            {
                switch (_selectedOrder.OrderName)
                {
                    case "build":

                        // CONSIDERATION: Should the Structure be spawned here so that we can create a the entity and 
                        // just rig this as a "repair" for the actual units?
                        Vector3 targetLocation = hitData.point;

                        Entity spawnedStructure = (Unit)EntityManager.Instance.SpawnEntity((int)_selectedOrder.Parameters["EntityID"], targetLocation, out bool spawnSuccess);
                        
                        _selectedOrder.Parameters.Add("location", targetLocation);
                        _selectedOrder.Parameters.Add("structure", spawnedStructure);
                        break;
                }

                foreach (Unit u in selectedEntities)
                {
                    u.IssueOrder(_selectedOrder);
                }
            }
        }

        _selectedOrder = null;
    }

    private void SetupInstance()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public static void SetSelectedOrder(EntityOrder _Order)
    {
        Instance._selectedOrder = _Order;
    }
}
