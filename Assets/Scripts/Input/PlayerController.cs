using System;
using System.Globalization;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows.Speech;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : NetworkBehaviour
{
    [SerializeField] float walkSpeed = 5f;
    [SerializeField] float mouseSensitivity = 2f;
    [SerializeField] Vector3 _camOffset;

    private Rigidbody _rigidbody;
    private Camera _cam;
    private Animator _animator;

    private float _vAxis;
    private float _hAxis;
    private float _xRot;

    private bool _isCrouching;

    private void Start()
    {
        if (IsOwner)
        {
            _cam = Camera.main;
            _cam.transform.SetParent(transform);
            _cam.transform.localPosition = _camOffset;
        }
    }

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (!IsOwner) return;

        HandleMove();
        HandleAttack();
        HandleLook();
    }

    private void HandleMove()
    {
        _hAxis = Input.GetAxis("Horizontal");
        _vAxis = Input.GetAxis("Vertical");

        Vector3 move = (transform.right * _hAxis + transform.forward * _vAxis).normalized * 100;
        _rigidbody.linearVelocity = move * walkSpeed * Time.deltaTime;

        bool isJumping = Input.GetKeyDown(KeyCode.Z);
        bool isRolling = Input.GetKeyDown(KeyCode.X);

        float axis = _vAxis != 0 ? Mathf.Sign(_vAxis) : _hAxis != 0 ? -1 : 0;
        _isCrouching = axis == 0 && Input.GetKey(KeyCode.LeftControl);

        SetIntegerServerRpc("Axis", (int)axis);
        SetBooleanServerRpc("IsCrouching", _isCrouching);

        if (isJumping) SetTriggerServerRpc("OnJumpForward");
        else if (isRolling) SetTriggerServerRpc("OnRollForward");
    }

    [ServerRpc]
    private void SetIntegerServerRpc(string key, int value)
        => SetIntegerClientRpc(key, value);
    [ServerRpc]
    private void SetBooleanServerRpc(string key, bool value)
        => SetBooleanClientRpc(key, value);
    [ServerRpc]
    private void SetTriggerServerRpc(string key)
        => SetTriggerClientRpc(key);
    [ClientRpc]
    private void SetIntegerClientRpc(string key, int value)
        => _animator.SetInteger(key, value);
    [ClientRpc]
    private void SetBooleanClientRpc(string key, bool value)
        => _animator.SetBool(key, value);
    [ClientRpc]
    private void SetTriggerClientRpc(string key)
        => _animator.SetTrigger(key);

    private void HandleAttack()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            SetTriggerServerRpc("OnPunch");
        }
    }

    private void HandleLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        _xRot -= mouseY;
        _xRot = Mathf.Clamp(_xRot, -80f, 80f);

        _cam.transform.localRotation = Quaternion.Euler(_xRot, 0, 0);
        transform.Rotate(Vector3.up * mouseX);
        _cam.transform.localPosition = transform.rotation * _camOffset;
    }
    public override void OnNetworkDespawn()
    {
        Debug.Log("Despawning...");
        base.OnNetworkDespawn();
    }
}
