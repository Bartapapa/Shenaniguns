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

    private PlayerState _currentState = PlayerState.None;
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
    public PlayerShoot3 PlayerShoot;
    public PlayerUI UI;

    [Header("Camera")]
    public Transform PlayerCameraFocusPoint;
    public float CameraMovementSpeed = 5f;

    //private const string MouseXInput = "Mouse X";
    //private const string MouseYInput = "Mouse Y";
    private const string MouseScrollInput = "Mouse ScrollWheel";
    //private const string HorizontalInput = "Horizontal";
    //private const string VerticalInput = "Vertical";

    private Vector2 _movementInputVector = Vector2.zero;
    private Vector2 _lookInputVector = Vector2.zero;

    [Header("Bullets")]
    public int MaxBullets = 6;
    public int CurrentBullets = 6;

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
        Camera.SetFollowTransform(PlayerCameraFocusPoint);

        // Ignore the character's collider(s) for camera obstruction checks
        Camera.IgnoredColliders.Clear();
        Camera.IgnoredColliders.AddRange(Character.GetComponentsInChildren<Collider>());

        CurrentState = PlayerState.Character;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Cursor.lockState = CursorLockMode.Locked;
        }


        if(CurrentState == PlayerState.Character)
        {
            HandleCharacterInput();
        }

        if(CurrentState == PlayerState.ConfirmingFire)
        {
            NullCharacterInput();

            Vector3 cameraForward = Camera.Camera.transform.forward;
            Vector3 cameraRight = Camera.Camera.transform.right;
            PlayerCameraFocusPoint.position += (cameraForward * _movementInputVector.y * CameraMovementSpeed * Time.unscaledDeltaTime)
                                            + (cameraRight * _movementInputVector.x * CameraMovementSpeed * Time.unscaledDeltaTime);
        }
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

    public void TransitionToState(PlayerState toState)
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
                PlayerBT.MoveToTimeScaleBulletTime(PlayerBT.BulletTimeFactor);
                //PlayerShoot.ClearVisualizationLines();
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
                PlayerCameraFocusPoint.position = Character.CameraFollowPoint.position;
                PlayerCameraFocusPoint.parent = Character.CameraFollowPoint;

                Camera.CurrentState = PlayerCamera.CameraState.FPS;
                //Camera.DefaultDistance = 0f;
                //Camera.MinDistance = 0f;
                //Camera.MaxDistance = 0f;
                //Camera.DistanceMovementSpeed = 5f;
                //Camera.DistanceMovementSharpness = 10f;

                UI.BulletMode.SetActive(false);
                //Initialize camera distance values
                break;
            case PlayerState.Dead:
                PlayerCameraFocusPoint.position = Character.CameraFollowPoint.position;
                PlayerCameraFocusPoint.parent = Character.CameraFollowPoint;

                Camera.CurrentState = PlayerCamera.CameraState.FPS;
                PlayerBT.StopBulletTime();
                break;
            case PlayerState.ConfirmingFire:
                PlayerBT.MoveToTimeScaleBulletTime(PlayerBT.MegaBulletTime);
                PlayerCameraFocusPoint.parent = null;

                Camera.CurrentState = PlayerCamera.CameraState.Detached;
                //Camera.DefaultDistance = 5f;
                //Camera.MinDistance = 0f;
                //Camera.MaxDistance = 10f;
                //Camera.DistanceMovementSpeed = 5f;
                //Camera.DistanceMovementSharpness = 10f;

                UI.BulletMode.SetActive(true);
                //Initialize camera distance values
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
        if (context.started && CurrentState == PlayerState.Character && CurrentBullets > 0)
        {
            PlayerShoot.VisualizeProjectileTrajectory();
        }
        else if (context.started && CurrentState == PlayerState.ConfirmingFire)
        {
            //Set new trajectory point;
            PlayerShoot.SetNewTrajectoryPoint();
        }
    }

    public void OnBulletTime(InputAction.CallbackContext context)
    {
        if (context.started && CurrentState == PlayerState.Character)
        {
            PlayerBT.ToggleBulletTime();
        }
    }

    public void OnStopTime(InputAction.CallbackContext context)
    {
        if (context.started && CurrentState == PlayerState.Character)
        {
            Debug.Log("Stop time!");
        }        
    }
    public void OnShadow(InputAction.CallbackContext context)
    {
        if (context.started && CurrentState == PlayerState.Character)
        {
            Debug.Log("Shadow!");
        }
    }
    public void OnConfirmFire(InputAction.CallbackContext context)
    {
        if (context.started && CurrentState == PlayerState.ConfirmingFire && CurrentBullets > 0)
        {
            List<Vector3> wayPoints = new List<Vector3>();
            for (int i = 0; i < PlayerShoot.ShootingLines.Count; i++)
            {
                if (PlayerShoot.ShootingLines[i].IsLocked)
                {
                    Vector3 newWayPoint = PlayerShoot.ShootingLines[i].DestinationPoint();
                    wayPoints.Add(newWayPoint);
                }
            }
            PlayerShoot.ConfirmFire(wayPoints);
        }
    }
    public void OnReturnFire(InputAction.CallbackContext context)
    {
        if (context.started && CurrentState == PlayerState.ConfirmingFire)
        {
            PlayerShoot.ReturnFire();
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

    private void NullCharacterInput()
    {
        PlayerCharacterInputs characterInputs = new PlayerCharacterInputs();

        // Build the CharacterInputs struct
        characterInputs.MoveAxisForward = 0f;
        characterInputs.MoveAxisRight = 0f;
        characterInputs.CameraRotation = Character.transform.rotation;

        // Apply inputs to character
        Character.SetInputs(ref characterInputs);
    }

    public void AcquireBullets(int bullets)
    {
        CurrentBullets += bullets;
        if (CurrentBullets > MaxBullets)
        {
            CurrentBullets = MaxBullets;
        }
    }
}
