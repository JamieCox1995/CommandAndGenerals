using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;

    [Header("General:")]
    [Tooltip("The speed that the camera will reach it's target location. The higher the number, the quicker and snappier the camera will feel. A lower number will make the camera more fluid.")]
    public float CameraMovementResponsivness = 2f;
    private Transform _cameraTransform;
    private Vector3 _targetCameraPosition;
    private Vector3 _dragStartPosition, _dragEndPosition;

    [Header("Camera Input Options:")]
    public bool AllowKeyboardMovement = false;
    public bool AllowEdgeScrolling = false;
    public bool AllowMouseDrag = false;

    [Header("Keyboard:")]
    public float KeyboardMovementSpeed = 0.1f;
    public float IncreasedKeyboardMovementSpeed = 0.5f;
    [SerializeField, Space] private float _keyboardMoveSensitivity = 1f;
    private float _currentMovementSpeed;

    [Header("Edge Scroll:")]
    public int EdgeScrollDistance = 50;
    public CursorAppearance CursorAppearance;
    private Vector2Int _CursorDirection = new Vector2Int(0, 0);
    private bool _isCursorSet = false;
    private Texture2D _currentTexture;

    [Header("Mouse Drag:")]
    public Texture2D MouseDragCursor;
    private bool _isMouseDragging = false;

    [Header("Rotation:")]
    public float RotationSpeed = 20f;
    public float RotationDampening = 1f;
    private Quaternion _targetRotation;

    [Header("Zoom:")]
    public float ZoomSensitivity = 30f;
    public float ZoomDampening = 1f;
    [Space]
    [SerializeField] private float _mouseScrollDeadzone = 0.075f;
    public float MinimumZoom;
    public float MaximumZoom;

    [Space]
    public bool LookAtCameraCentre = true;
    private float _currentZoom;

    private Camera _camera;

    // Start is called before the first frame update
    void Start()
    {
        _camera = Camera.main;
        _cameraTransform = _camera.transform;

        _currentZoom = Mathf.Clamp(_currentZoom, MinimumZoom, MaximumZoom);
    }

    // Update is called once per frame
    void Update()
    {
        // TODO: Create actual Input Key for this button so that it can be remapped.
        _currentMovementSpeed = Input.GetKey(KeyCode.LeftControl) ? IncreasedKeyboardMovementSpeed : KeyboardMovementSpeed;

        HandleRotation();

        HandleKeyboardMovement();

        HandleEdgeScrolling();

        HandleMouseDragging();

        HandleZoom();

        transform.position = Vector3.Lerp(transform.position, _targetCameraPosition, Time.deltaTime * CameraMovementResponsivness);

        ChangeCursor();
    }

    private void HandleKeyboardMovement()
    {
        // Obviously, if we have not enabled the Keyboard movement we do not want to try to run any of the below logic to move the camera via keyboard controls
        if (!AllowKeyboardMovement) return;

        // Creating a Vector2 to store the input from the player.
        Vector2 input = new Vector2
        {
            x = Input.GetAxis("Horizontal"),
            y = Input.GetAxis("Vertical")
        };

        // Updating the cameras location based off of the input vector we just created.
        _targetCameraPosition += (transform.forward * input.y * _keyboardMoveSensitivity);
        _targetCameraPosition += (transform.right * input.x * _keyboardMoveSensitivity);
    }

    private void HandleEdgeScrolling()
    {
        if (!AllowEdgeScrolling) return;

        _CursorDirection = Vector2Int.zero;

        int xLimit = Screen.width - EdgeScrollDistance;
        int yLimit = Screen.height - EdgeScrollDistance;

        int distanceInEdge = 0;
        float moveAmount = 0f;

        if(Input.mousePosition.x > xLimit)
        {
            distanceInEdge = (int)Input.mousePosition.x - xLimit;
            moveAmount = (float)distanceInEdge / EdgeScrollDistance;

            _targetCameraPosition += (transform.right * _currentMovementSpeed * _keyboardMoveSensitivity * moveAmount);

            _CursorDirection.x = 1;
        }
        else if(Input.mousePosition.x < EdgeScrollDistance)
        {
            distanceInEdge = EdgeScrollDistance - (int)Input.mousePosition.x;
            moveAmount = (float)distanceInEdge / EdgeScrollDistance;

            _targetCameraPosition += (transform.right * -_currentMovementSpeed * _keyboardMoveSensitivity * moveAmount);

            _CursorDirection.x = -1;
        }

        if (Input.mousePosition.y > yLimit)
        {
            distanceInEdge = (int)Input.mousePosition.y - yLimit;
            moveAmount = (float)distanceInEdge / EdgeScrollDistance;

            _targetCameraPosition += (transform.forward * _currentMovementSpeed * _keyboardMoveSensitivity * moveAmount);

            _CursorDirection.y = 1;
        }

        if (Input.mousePosition.y < EdgeScrollDistance)
        {
            distanceInEdge = EdgeScrollDistance - (int)Input.mousePosition.y;
            moveAmount = (float)distanceInEdge / EdgeScrollDistance;

            _targetCameraPosition += (transform.forward * -_currentMovementSpeed * _keyboardMoveSensitivity * moveAmount);

            _CursorDirection.y = -1;
        }
    }

    private void HandleMouseDragging()
    {
        if (!AllowMouseDrag) return;

        _isMouseDragging = false;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, Vector3.zero);
        float point;

        // When we press down the middle mouse button, we want to begin dragging the mouse.
        if(Input.GetMouseButtonDown(2))
        {
            if(plane.Raycast(ray, out point))
            {
                _dragStartPosition = ray.GetPoint(point);
            }
        }

        if(Input.GetMouseButton(2))
        {
            if (plane.Raycast(ray, out point))
            {
                _dragEndPosition = ray.GetPoint(point);

                _targetCameraPosition = transform.position + _dragStartPosition - _dragEndPosition;

                _isMouseDragging = true;
            }
        }
    }

    private void HandleZoom()
    {
        _cameraTransform.position = Vector3.Lerp(_cameraTransform.position, new Vector3(_cameraTransform.position.x, _currentZoom, _cameraTransform.position.z), Time.deltaTime * ZoomDampening);

        if (LookAtCameraCentre)
        {
            _cameraTransform.LookAt(transform.position);
        }

        float zoomInput = -Input.GetAxis("Mouse ScrollWheel");                                                                                                                                                                                                                                       Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(zoomInput) < _mouseScrollDeadzone)
        {
            return;
        }

        _currentZoom = _cameraTransform.position.y + zoomInput * ZoomSensitivity;

        _currentZoom = Mathf.Clamp(_currentZoom, MinimumZoom, MaximumZoom); 
    }

    private void HandleRotation()
    {
        float rotationInput = -Input.GetAxis("Camera Rotation");

        _targetRotation = Quaternion.Euler(0f, rotationInput * RotationSpeed + transform.rotation.eulerAngles.y, 0f);

        transform.rotation = Quaternion.Lerp(transform.rotation, _targetRotation, Time.deltaTime * RotationDampening);
    }

    private void ChangeCursor()
    {
        if(_isMouseDragging)
        {
            Cursor.SetCursor(MouseDragCursor, Vector2.zero, CursorMode.Auto);
        }
        else
        {
            if (_CursorDirection == new Vector2Int(0, 1))
            {
                Cursor.SetCursor(CursorAppearance.CursorTop, Vector2.zero, CursorMode.Auto);
            }
            else if (_CursorDirection == new Vector2Int(1, 1))
            {
                Cursor.SetCursor(CursorAppearance.CursorTopLeft, Vector2.zero, CursorMode.Auto);
            }
            else if (_CursorDirection == new Vector2Int(1, 0))
            {
                Cursor.SetCursor(CursorAppearance.CursorLeft, Vector2.zero, CursorMode.Auto);
            }
            else if (_CursorDirection == new Vector2Int(1, -1))
            {
                Cursor.SetCursor(CursorAppearance.CursorBottomLeft, Vector2.zero, CursorMode.Auto);
            }
            else if (_CursorDirection == new Vector2Int(0, -1))
            {
                Cursor.SetCursor(CursorAppearance.CursorBottom, Vector2.zero, CursorMode.Auto);
            }
            else if (_CursorDirection == new Vector2Int(-1, -1))
            {
                Cursor.SetCursor(CursorAppearance.CursorBottomRight, Vector2.zero, CursorMode.Auto);
            }
            else if (_CursorDirection == new Vector2Int(-1, 0))
            {
                Cursor.SetCursor(CursorAppearance.CursorRight, Vector2.zero, CursorMode.Auto);
            }
            else if (_CursorDirection == new Vector2Int(-1, 1))
            {
                Cursor.SetCursor(CursorAppearance.CursorTopRight, Vector2.zero, CursorMode.Auto);
            }
            else
            {
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            }
        }

        //_isCursorSet = true;
    }
}
