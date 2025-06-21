using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    public static SelectionManager Instance;

    private Dictionary<int, Entity> _selectedEntities = new Dictionary<int, Entity>();
    [SerializeField] private EntitySelectionMode _selectionMode = EntitySelectionMode.Normal;
    private Camera _mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        Setup();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            OnLeftClick();
        }

        HandleSelectionModeInput();
    }

    public void OnLeftClick()
    {
        //  When the mouse button is clicked, we want to check to see if the mouse is over a ISelectableEntity.
        // Creating a ray to raycast with from the mouse position pointing in the direction of the camera.
        Ray cameraRay = _mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;         

        if(Physics.Raycast(cameraRay, out hit))
        {
            if(hit.collider.gameObject.TryGetComponent(out Entity ent))
            {
                // If we are over a selectable entity;
                switch (_selectionMode)
                {
                    //  a) if we are not pressing shift, reset the selected entities and add the single entity to the list.
                    case EntitySelectionMode.Normal:
                        // If we are not pressing any of the keys, we will to just set our selection to the entity we have clicked on.
                        DeselectEntityList();

                        if(!_selectedEntities.ContainsKey(ent.ID))
                        {
                            _selectedEntities.Add(ent.ID, ent);
                            ent.SelectEntity();
                        }

                        break;

                    //  b) if we are pressing shift, add the entity to the list.
                    case EntitySelectionMode.Additive:
                        // Add the clicked entity to the list.
                        if (!_selectedEntities.ContainsKey(ent.ID))
                        {
                            _selectedEntities.Add(ent.ID, ent);
                            ent.SelectEntity();
                        }

                        break;

                    //  c) if we are pressing control, remove the entity from the list.
                    case EntitySelectionMode.Subtractive:
                        // Remove the entity from the list.
                        if (_selectedEntities.ContainsKey(ent.ID))
                        {
                            _selectedEntities.Remove(ent.ID);
                            ent.DeslectEntity();
                        }

                        break;
                }
            }
            else
            {
                // If we haven't hit a selectable entity, we want to cancel our selection
                // Iterate over all of the currently selected entities and tell them that we are deselecting them.
                DeselectEntityList();
            }
        }
    }

    private void SetupInstance()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Setup()
    {
        SetupInstance();

        // Making sure that the list of selected entities is initialized and cleared.
        _selectedEntities = new Dictionary<int, Entity>();

        _mainCamera = Camera.main;
    }

    private void HandleSelectionModeInput()
    {
        if (Input.GetButton("Selection Mode - Additive"))
        {
            _selectionMode = EntitySelectionMode.Additive;
        }
        else if (Input.GetButton("Selection Mode - Subtractive"))
        {
            _selectionMode = EntitySelectionMode.Subtractive;
        }
        else
        {
            _selectionMode = EntitySelectionMode.Normal;
        }
    }

    private void DeselectEntityList()
    {
        foreach (KeyValuePair<int, Entity> entity in _selectedEntities)
        {
            entity.Value.DeslectEntity();
        }

        _selectedEntities = new Dictionary<int, Entity>();
    }

    public static List<Entity> GetSelectedEntities()
    {
        return SelectionManager.Instance._selectedEntities.Values.ToList();
    }
}
