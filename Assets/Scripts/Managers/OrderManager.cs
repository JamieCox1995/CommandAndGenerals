using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderManager : MonoBehaviour
{
    public static OrderManager Instance;
    private EntityOrder _selectedOrder;

    private GameObject _constructionPreview;
    private bool _isValidConstructionLocation = true;

    // Start is called before the first frame update
    void Start()
    {
        SetupInstance();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateConstructionGhostLocation();

        // Initiating a Build Action.
        if(Input.GetKeyDown(KeyCode.B))
        {
            StartConstructionOrder(1);
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

        // IF we have no selected entities, then why do we need to try working out any orders.
        if (selectedEntities.Count <= 0) return;

        Vector3 selectionCentre = FindSelectionCentre(selectedEntities);

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
                        if (!_isValidConstructionLocation) break;

                        Vector3 targetLocation = hitData.point;

                        Entity spawnedStructure = (Unit)EntityManager.Instance.SpawnEntity((int)_selectedOrder.Parameters["EntityID"], targetLocation, out bool spawnSuccess);
                        
                        Collider coll = spawnedStructure.GetComponentInChildren<Collider>();

                        _selectedOrder.Parameters.Add("location", coll.ClosestPoint(selectionCentre));
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

    private Vector3 FindSelectionCentre(List<Unit> _SelectedEntities)
    {
        Vector3 selectionCenter = Vector3.zero;

        foreach (Unit unit in _SelectedEntities)
        {
            selectionCenter += unit.transform.position;
        }
        selectionCenter = selectionCenter / _SelectedEntities.Count;

        return selectionCenter;
    }

    private void UpdateConstructionGhostLocation()
    {
        // Make sure that we actually have a ghost before doing anything.
        if (_constructionPreview == null) return;

        // We want to fire a Raycast out from the mouse's screen position.
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitData;

        // Firing the Raycast
        //TODO: We want to add a LayerMask here so that we only do this cast against the terrain.
        if (Physics.Raycast(ray, out hitData))
        {
            _constructionPreview.transform.position = hitData.point;
        }
    }

    public static void SetSelectedOrder(EntityOrder _Order)
    {
        Instance._selectedOrder = _Order;
    }

    public void StartConstructionOrder(int _EntityID)
    {
        // Setting the selected order so that when we right click we place a
        // structure.
        _selectedOrder = new EntityOrder
        {
            OrderName = "build",
            Parameters = new Dictionary<string, object>
                {
                    { "EntityID", _EntityID }
                }
        };

        // Spawning in a ghost of the structure and having it follow the mouse.
    }
}
