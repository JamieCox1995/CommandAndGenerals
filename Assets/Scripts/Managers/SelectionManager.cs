using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SelectionManager : MonoBehaviour
{
    public static SelectionManager Instance;

    public Image UIMouseDrag;

    private Dictionary<int, Entity> _selectedEntities = new Dictionary<int, Entity>();
    private EntitySelectionMode _selectionMode = EntitySelectionMode.Normal;

    private Camera _mainCamera;
    private Vector2 _mouseDragStartLocation;
    private Vector2 _mouseCurrentDragLocation;
    private bool _isMouseHeldDown = false;


    // Start is called before the first frame update
    void Start()
    {
        Setup();
    }

    // Update is called once per frame
    void Update()
    {
        // Detecting input from the mouse
        HandleMouseClicks();

        HandleMouseDrag();


        // Detecting input from the player to change the type of unit selection.
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
            Entity ent = hit.collider.gameObject.GetComponentInParent<Entity>();

            if (ent != null)
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

    private void HandleMouseClicks()
    {
        // If we are holding down the mouse button, we want to draw a box to screen to represent a drag selection.
        if (Input.GetMouseButton(0))
        {
            // If we were not holding the mouse button down last frame
            if (!_isMouseHeldDown)
            {
                // Capture the position of the mouse this frame
                _mouseDragStartLocation = Input.mousePosition;
                // and mark that we are holding the mouse down.
                _isMouseHeldDown = true;
            }

            // We then want to capture the current mouse's position.
            _mouseCurrentDragLocation = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            if(Vector2.Distance(_mouseDragStartLocation, _mouseCurrentDragLocation) < 10)
            {
                OnLeftClick();
            }

            //Reset all of the variables which we were using to determine the size of the drag selection.
            _isMouseHeldDown = false;
            _mouseDragStartLocation = Vector2.zero;
            _mouseCurrentDragLocation = Vector2.zero;
        }
    }

    private void HandleMouseDrag()
    {
        if (_isMouseHeldDown) 
        {
            // If we have not dragged very much, we just want to return
            if (Vector2.Distance(_mouseDragStartLocation, _mouseCurrentDragLocation) < 10)
            {
                return;
            }

            // Enabling the UI element for the box
            UIMouseDrag.gameObject.SetActive(true);

            // Getting the size we want based off of where we started dragging the mouse and where the mouse currently is
            float width = _mouseCurrentDragLocation.x - _mouseDragStartLocation.x;
            float height = _mouseCurrentDragLocation.y - _mouseDragStartLocation.y;

            // Calculating where the anchor location of the Sprite is based on the freshly calculated width/height so that it is in the centre.
            UIMouseDrag.rectTransform.anchoredPosition = _mouseDragStartLocation + new Vector2(width / 2, height / 2);
            // Now to just set the size. Using the Absolute size so that we get the proper size of the object.
            UIMouseDrag.rectTransform.sizeDelta= new Vector2(Mathf.Abs(width), Mathf.Abs(height));

            // Now we can try selecting the units that are inside the bounds of the selection.
            // Creating a Bounding Box
            Bounds selectionBounds = new Bounds(UIMouseDrag.rectTransform.anchoredPosition, UIMouseDrag.rectTransform.sizeDelta);

            List<Entity> entities = EntityManager.Instance.GetAllEntities();

            for(int index = 0; index < entities.Count; index++)
            {
                // Getting the screen location of the current entity
                Entity ent = entities[index];
                Vector2 screenPos = Camera.main.WorldToScreenPoint(ent.gameObject.transform.position);

                // Now we want to check to see if the current entity's location is within the selection bounds.

                // We want to check to see what selection mode we are in

                // If we are in additive, we add what is in the bounds and keep whatever is already selected.

                if(IsEntityInSelection(screenPos, selectionBounds))
                {
                    if(_selectionMode == EntitySelectionMode.Subtractive)
                    {
                        if (_selectedEntities.ContainsKey(ent.ID))
                        {
                            _selectedEntities.Remove(ent.ID);
                            ent.DeslectEntity();
                        }
                    }
                    else
                    {
                        if (!_selectedEntities.ContainsKey(ent.ID))
                        {
                            _selectedEntities.Add(ent.ID, ent);
                            ent.SelectEntity();
                        }
                    }
                }
                else
                {
                    if(_selectionMode == EntitySelectionMode.Normal)
                    {
                        if (_selectedEntities.ContainsKey(ent.ID))
                        {
                            _selectedEntities.Remove(ent.ID);
                            ent.DeslectEntity();
                        }
                    }
                }
            }
        }
        else
        {
            UIMouseDrag.gameObject.SetActive(false);
        }


    }

    private bool IsEntityInSelection(Vector2 _EntityScreenLocation, Bounds _SelectionBounds)
    {
        return _EntityScreenLocation.x >= _SelectionBounds.min.x && _EntityScreenLocation.x <= _SelectionBounds.max.x
                && _EntityScreenLocation.y >= _SelectionBounds.min.y && _EntityScreenLocation.y <= _SelectionBounds.max.y;
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

    public static List<Unit> GetOrderableUnitsFromSelection()
    {
        return GetSelectedEntities().Select(unit => unit as Unit).Where(unit => unit != null).ToList();
    }
}
