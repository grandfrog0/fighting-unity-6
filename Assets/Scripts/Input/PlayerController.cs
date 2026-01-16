using System;
using System.Globalization;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : NetworkBehaviour
{
    [SerializeField] float walkSpeed = 5f;
    [SerializeField] float runSpeed = 5f;
    [SerializeField] float mouseSensitivity = 2f;

    private InputSystem_Actions _actions;
    private Rigidbody _rigidbody;
    private Camera _cam;
    private Animator _animator;

    private float _vAxis;
    private float _hAxis;
    private float _xRot;

    private bool _isRunning;
    private bool _isCrouching;

    private void Start()
    {
        _actions = new InputSystem_Actions();
        _actions.Enable();

        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponentInChildren<Animator>();

        if (IsOwner)
        {
            _cam = Camera.main;
            _cam.transform.SetParent(transform);
            _cam.transform.localPosition = new Vector3(0, 1.6f, 0);
        }
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
        Vector3 move = (transform.right * _hAxis + transform.forward * _vAxis) * (_isRunning ? 2 : 1);
        _rigidbody.linearVelocity = move * runSpeed * Time.deltaTime;

        bool isJumping = Input.GetKeyDown(KeyCode.Z);
        bool isRolling = Input.GetKeyDown(KeyCode.X);

        float axis = (_isRunning ? 2 : 1) * _vAxis != 0 ? Mathf.Sign(_vAxis) : _hAxis != 0 ? 1 : 0;
        _isCrouching = !_isRunning && Input.GetKeyDown(KeyCode.LeftControl);

        _animator.SetInteger("Axis", (int)axis);
        _animator.SetBool("IsCrouching", _isCrouching);

        if (isJumping) _animator.SetTrigger("OnJumpForward");
        else if (isRolling) _animator.SetTrigger("OnRollForward");
    }

    private void HandleAttack()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            _animator.SetTrigger("OnPunch");
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
    }

    public override void OnNetworkDespawn()
    {
        Debug.Log("Despawning...");
        _cam.transform.parent = null;
        base.OnNetworkDespawn();
    }

    private void OnEnable()
    {
        _actions.Player.Sprint.started += OnSprintStarted;
        _actions.Player.Sprint.canceled += OnSprintCanceled;
    }
    private void OnDisable()
    {
        _actions.Player.Sprint.started -= OnSprintStarted;
        _actions.Player.Sprint.canceled -= OnSprintCanceled;
    }

    private void OnSprintStarted(InputAction.CallbackContext _) => _isRunning = true;
    private void OnSprintCanceled(InputAction.CallbackContext _) => _isRunning = false;
}
