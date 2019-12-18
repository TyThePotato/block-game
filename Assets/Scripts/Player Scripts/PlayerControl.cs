using UnityEngine;
using QFSW.QC;

public class PlayerControl : MonoBehaviour
{
    public float movementSpeed;
    public float sprintMultiplier = 1.5f;
    public float sprintFOVMultiplier = 1.2f;
    public float FOV = 90f;
    public bool allowAirControl;
    public float jumpPower;
    public float gravity;
    public bool allowFOVChange = true;
    public float lookSpeed;
    public bool movementEnabled = false;
    public bool cameraEnabled = true;
    public bool cursorLocked = true;
    public Vector3 SpawnPoint;

    private float _speedMultiplier;
    private bool _playerMoving;
    private bool _movementKeysPressed = false;
    private Transform _camTransform;
    private Vector3 _moveDirection;
    private Vector2 _camRotation;
    private CharacterController _cc;
    private Camera _cam;

    private void Start() {
        _cam = Camera.main;
        _camTransform = _cam.gameObject.transform;
        _cc = GetComponent<CharacterController>();

        SpawnPoint = World.instance.WorldSpawn;
        SetPosition(SpawnPoint.x, SpawnPoint.y, SpawnPoint.z);
    }

    private void Update() {
        // MOVEMENT
        if (!movementEnabled) {
            return;
        }

        Vector3 cameraForward = new Vector3(_camTransform.forward.x, 0, _camTransform.forward.z).normalized;
        Vector3 cameraRight = new Vector3(_camTransform.right.x, 0, _camTransform.right.z).normalized;
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        float inputModifyFactor = (horizontal != 0.0f && vertical != 0.0f) ? 0.7071f : 1.0f;

        _movementKeysPressed = (horizontal != 0.0f || vertical != 0.0f) ? true : false;

        if (Input.GetButton("Sprint") && _movementKeysPressed) {
            _speedMultiplier = sprintMultiplier;
            _cam.fieldOfView = (allowFOVChange) ? FOV * sprintFOVMultiplier : FOV; // The transition is so fucking jumpy tbh
        } else {
            _cam.fieldOfView = FOV;
            _speedMultiplier = 1f;
        }

        if (_cc.isGrounded) {
            _moveDirection = cameraForward * (vertical * inputModifyFactor) + cameraRight * (horizontal * inputModifyFactor);
            _moveDirection *= movementSpeed * _speedMultiplier;
            _playerMoving = true;

            if (Input.GetButton("Jump")) {
                _moveDirection.y = jumpPower;
            }
        } else {
            if (allowAirControl && _playerMoving) {
                Vector3 _tempVector = cameraForward * (vertical * inputModifyFactor) + cameraRight * (horizontal * inputModifyFactor);
                _moveDirection.x = _tempVector.x * movementSpeed * _speedMultiplier;
                _moveDirection.z = _tempVector.z * movementSpeed * _speedMultiplier;
            }
        }

        _moveDirection.y -= gravity * Time.deltaTime;
        _cc.Move(_moveDirection * Time.deltaTime);

        // cursor lock stuff
        if (cursorLocked == true) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        } else {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (!cameraEnabled) {
            return;
        }
        // Camera stuff
        _camRotation.x += Input.GetAxis("Mouse X") * lookSpeed;
        _camRotation.y -= Input.GetAxis("Mouse Y") * lookSpeed;

        _camRotation.x = Mathf.Repeat(_camRotation.x, 360);
        _camRotation.y = Mathf.Clamp(_camRotation.y, -89, 89);

        _camTransform.rotation = Quaternion.Euler(_camRotation.y, _camRotation.x, 0);
    }

    [Command("tp")]
    public void SetPosition (float x, float y, float z) {
        transform.position = new Vector3(x+0.5f,y+1,z+ 0.5f);
    }

    [Command("fov")]
    public void SetFOV (float fov) {
        FOV = fov;
    }
}
