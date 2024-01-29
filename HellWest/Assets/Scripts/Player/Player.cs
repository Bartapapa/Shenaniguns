using KinematicCharacterController.Examples;
using KinematicCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public enum PlayerState
    {
        None,
        Character,
        Dead,
        ConfirmingFire,
    }

    private PlayerState _currentState = PlayerState.Character;
    public PlayerState CurrentState
    {
        get
        {
            return _currentState;
        }
        set
        {
            if (_currentState != value) TransitionToState(value);
        }
    }
    [HideInInspector] public static Player Instance;

    public PlayerCharacterController Character;
    public PlayerCamera Camera;
    public PlayerBulletTimer PlayerBT;

    //private const string MouseXInput = "Mouse X";
    //private const string MouseYInput = "Mouse Y";
    private const string MouseScrollInput = "Mouse ScrollWheel";
    //private const string HorizontalInput = "Horizontal";
    //private const string VerticalInput = "Vertical";

    private Vector2 _movementInputVector = Vector2.zero;
    private Vector2 _lookInputVector = Vector2.zero;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        // Tell camera to follow transform
        Camera.SetFollowTransform(Character.CameraFollowPoint);

        // Ignore the character's collider(s) for camera obstruction checks
        Camera.IgnoredColliders.Clear();
        Camera.IgnoredColliders.AddRange(Character.GetComponentsInChildren<Collider>());
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        HandleCharacterInput();
    }

    private void LateUpdate()
    {
        // Handle rotating the camera along with physics movers
        if (Camera.RotateWithPhysicsMover && Character.Motor.AttachedRigidbody != null)
        {
            Camera.PlanarDirection = Character.Motor.AttachedRigidbody.GetComponent<PhysicsMover>().RotationDeltaFromInterpolation * Camera.PlanarDirection;
            Camera.PlanarDirection = Vector3.ProjectOnPlane(Camera.PlanarDirection, Character.Motor.CharacterUp).normalized;
        }

        HandleCameraInput();
    }

    private void TransitionToState(PlayerState toState)
    {
        PlayerState oldState = _currentState;
        switch (oldState)
        {
            case PlayerState.None:
                break;
            case PlayerState.Character:
                break;
            case PlayerState.Dead:
                break;
            case PlayerState.ConfirmingFire:
                break;
            default:
                break;
        }
        _currentState = toState;
        switch (toState)
        {
            case PlayerState.None:
                break;
            case PlayerState.Character:
                break;
            case PlayerState.Dead:
                break;
            case PlayerState.ConfirmingFire:
                break;
            default:
                break;
        }
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        _movementInputVector = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        _lookInputVector = context.ReadValue<Vector2>();
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Debug.Log("Fire!");
        }
    }

    public void OnBulletTime(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            PlayerBT.ToggleBulletTime();
        }
    }

    public void OnStopTime(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Debug.Log("Stop time!");
        }        
    }
    public void OnShadow(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Debug.Log("Shadow!");
        }
    }
    public void OnConfirmFire(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Debug.Log("Confirm fire!");
        }
    }
    public void OnReturnFire(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Debug.Log("Return fire!");
        }
    }

    private void HandleCameraInput()
    {
        // Create the look input vector for the camera
        Vector3 lookInputVector = new Vector3(_lookInputVector.x, _lookInputVector.y, 0f);

        // Prevent moving the camera while the cursor isn't locked
        if (Cursor.lockState != CursorLockMode.Locked)
        {
            lookInputVector = Vector3.zero;
        }

        // Input for zooming the camera (disabled in WebGL because it can cause problems)
        float scrollInput = -Input.GetAxis(MouseScrollInput);
#if UNITY_WEBGL
        scrollInput = 0f;
#endif

        // Apply inputs to the camera
        Camera.UpdateWithInput(Time.unscaledDeltaTime, scrollInput, lookInputVector);

        // Handle toggling zoom level
        if (Input.GetMouseButtonDown(1))
        {
            Camera.TargetDistance = (Camera.TargetDistance == 0f) ? Camera.DefaultDistance : 0f;
        }
    }

    private void HandleCharacterInput()
    {
        PlayerCharacterInputs characterInputs = new PlayerCharacterInputs();

        // Build the CharacterInputs struct
        characterInputs.MoveAxisForward = _movementInputVector.y;
        characterInputs.MoveAxisRight = _movementInputVector.x;
        characterInputs.CameraRotation = Camera.Transform.rotation;
        //characterInputs.JumpDown = Input.GetKeyDown(KeyCode.Space);
        //characterInputs.CrouchDown = Input.GetKeyDown(KeyCode.C);
        //characterInputs.CrouchUp = Input.GetKeyUp(KeyCode.C);

        // Apply inputs to character
        Character.SetInputs(ref characterInputs);
    }
}
