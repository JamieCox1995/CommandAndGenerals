using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderManager : MonoBehaviour
{
    public static OrderManager Instance;

    // Start is called before the first frame update
    void Start()
    {
        SetupInstance();
    }

    // Update is called once per frame
    void Update()
    {
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
            // Try to see if what was hit is an entity.
            if(hitData.collider.GetComponentInParent<Entity>())
            {

            }
            else
            {
                Vector3 targetLocation = hitData.point;
                EntityOrder moveOrder = new EntityOrder
                {
                    OrderName = "moveTo",
                    OrderParameters = targetLocation.ToString(),
                    OrderPriority = 1
                };

                foreach(Unit u in selectedEntities)
                {
                    u.IssueOrder(moveOrder);
                }
            }
        }
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
}
