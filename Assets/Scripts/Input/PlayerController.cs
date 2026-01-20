using System;
using System.Collections;
using System.Globalization;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows.Speech;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : NetworkBehaviour
{
    [SerializeField] float walkSpeed = 5f;
    [SerializeField] float jumpPower = 5f;
    [SerializeField] float mouseSensitivity = 2f;
    [SerializeField] Vector3 _camOffset;

    private Rigidbody _rigidbody;
    private Camera _cam;
    private Animator _animator;

    private float _vAxis;
    private float _hAxis;
    private float _xRot;
    private bool _canMove = true;

    private bool _isCrouching;
    private bool _isRunning;

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

        HandleAttack();
        HandleMove();
        HandleLook();
    }

    private void HandleMove()
    {
        if (_canMove)
        {
            _hAxis = Input.GetAxis("Horizontal");
            _vAxis = Input.GetAxis("Vertical");
            _isRunning = Input.GetKey(KeyCode.LeftShift) && _vAxis == 1;
            _isCrouching = !_isRunning && Input.GetKey(KeyCode.LeftControl);

            Vector3 move = (_isRunning ? 2 : _isCrouching ? 0.5f : 1) * (transform.right * _hAxis + transform.forward * _vAxis).normalized * 50;
            _rigidbody.linearVelocity = move * walkSpeed * Time.deltaTime;

            float axis = (_isRunning ? 2 : 1) * (_vAxis != 0 ? Mathf.Sign(_vAxis) : _hAxis != 0 ? -1 : 0);

            SetIntegerServerRpc("Axis", (int)axis);
            SetBooleanServerRpc("IsCrouching", _isCrouching);

            bool isRolling = Input.GetKeyDown(KeyCode.X);
            if (isRolling) RollForwardServerRpc();
        }
    }

    public void SetAnimationBoolean(string key, bool value)
    {
        SetBooleanServerRpc(key, value);
    }
    [ServerRpc]
    public void SetIntegerServerRpc(string key, int value)
        => SetIntegerClientRpc(key, value);
    [ServerRpc]
    public void SetBooleanServerRpc(string key, bool value)
        => SetBooleanClientRpc(key, value);
    [ServerRpc]
    public void SetTriggerServerRpc(string key)
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

    [ServerRpc]
    private void RollForwardServerRpc()
        => RollForwardClientRpc();
    [ClientRpc]
    private void RollForwardClientRpc()
        => StartCoroutine(RollForwardRoutine());
    private IEnumerator RollForwardRoutine()
    {
        _canMove = false;
        _rigidbody.linearVelocity = Vector3.zero;
        _rigidbody.AddForce(transform.forward * jumpPower * 1.5f, ForceMode.Impulse);
        SetTriggerServerRpc("OnRollForward");
        yield return new WaitForSeconds(0.25f);
        _canMove = true;
    }

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
        //_cam.transform.localPosition = transform.rotation * _camOffset;
    }
    public override void OnNetworkDespawn()
    {
        Debug.Log("Despawning...");
        base.OnNetworkDespawn();
    }
}
