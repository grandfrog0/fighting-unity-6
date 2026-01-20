using Unity.VisualScripting;
using UnityEngine;

public class ObservableCamera : MonoBehaviour
{
    public Transform Target;
    public GameObject TargetModel;
    [SerializeField] float mouseSensitivity = 2f;
    [SerializeField] Vector3 _camOffset;
    private float _xRot, _yRot;
    private float _distance = 1;

    private void Update()
    {
        if (Target == null)
            return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        _distance += Input.GetAxis("Mouse ScrollWheel");

        _xRot -= mouseY;
        _xRot = Mathf.Clamp(_xRot, -80f, 80f);
        _yRot += mouseX;
        _distance = Mathf.Clamp(_distance, 0, 3);

        TargetModel.SetActive(_distance != 0);

        transform.localRotation = Quaternion.Euler(_xRot, _yRot, 0);
        transform.position = Target.position + transform.rotation * _camOffset * _distance;
        Target.Rotate(Vector3.up * mouseX);

    }
}
