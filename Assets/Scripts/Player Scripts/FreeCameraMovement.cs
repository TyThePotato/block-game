using UnityEngine;

public class FreeCameraMovement : MonoBehaviour
{
    private Vector2 _cameraRotation;
    public float cameraSpeed;
    public float maxYAngle = 80f;
    public float movementSpeed;

    public bool toggleLock;

    private void Update() {
        if (Input.GetKey(KeyCode.L)) {
            toggleLock = !toggleLock;
            Cursor.lockState = CursorLockMode.None;
        }

        _cameraRotation.x += Input.GetAxis("Mouse X") * cameraSpeed;
        _cameraRotation.y -= Input.GetAxis("Mouse Y") * cameraSpeed;
        _cameraRotation.x = Mathf.Repeat(_cameraRotation.x, 360);
        _cameraRotation.y = Mathf.Clamp(_cameraRotation.y, -maxYAngle, maxYAngle);

        transform.rotation = Quaternion.Euler(_cameraRotation.y, _cameraRotation.x, 0);

        if (Cursor.lockState != CursorLockMode.Locked && !toggleLock)
            Cursor.lockState = CursorLockMode.Locked;

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        float ud = Input.GetAxisRaw("Updown");
        Vector3 m = new Vector3(h, ud, v);

        transform.Translate(m.normalized * movementSpeed * Time.deltaTime, Space.Self);
    }
}
