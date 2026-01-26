using Unity.VisualScripting;
using UnityEngine;

public class ObservableCamera : MonoBehaviour
{
    public Transform Target;
    public GameObject TargetModel;
    [SerializeField] float sensitivity;
    [SerializeField] Vector3 offset;
    [SerializeField] float maxDistance;
    private float _distance;
    private float _xRot, _yRot;

    private void Update()
    {
        if (Target == null)
            return;

        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        _xRot -= mouseY;
        _xRot = Mathf.Clamp(_xRot, -80, 80);
        _yRot += mouseX;

        if (Input.GetKeyDown(KeyCode.V))
        {
            if (_distance == 0)
            {
                _distance = maxDistance;
                TargetModel.SetActive(true);
            }
            else
            {
                _distance = 0;
                TargetModel.SetActive(false);
            }
        }

        transform.localRotation = Quaternion.Euler(_xRot, _yRot, 0);
        transform.position = Target.position + transform.rotation * offset * _distance;
        Target.Rotate(Vector3.up * mouseY);
    }
}

/*
 * public Transform Target;
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

        if (Input.GetKeyDown(KeyCode.V))
        {
            _distance = _distance == 0 ? 3 : 0;
        }

        TargetModel.SetActive(_distance != 0);

        transform.localRotation = Quaternion.Euler(_xRot, _yRot, 0);
        transform.position = Target.position + transform.rotation * _camOffset * _distance;
        Target.Rotate(Vector3.up * mouseX);

    }*/