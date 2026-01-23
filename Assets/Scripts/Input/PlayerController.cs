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

    private Rigidbody _rigidbody;
    private Animator _animator;

    private ObservableCamera _cam;

    private float _vAxis;
    private float _hAxis;
    private bool _canMove = true;

    private bool _isCrouching;
    private bool _isRunning;

    public bool isAlive = true;

    [SerializeField] Transform modelParent;
    [SerializeField] GameObject punch;

    private void Start()
    {
        if (IsOwner)
        {
            _cam = Camera.main.GetComponent<ObservableCamera>();
            _cam.Target = transform;
            _cam.TargetModel = transform.GetChild(0).gameObject;
        }
    }

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (!IsOwner || !isAlive || !_canMove) return;

        HandleAttack();
        HandleMove();
    }

    private void HandleMove()
    {
        _hAxis = Input.GetAxis("Horizontal");
        _vAxis = Input.GetAxis("Vertical");
        _isRunning = Input.GetKey(KeyCode.LeftShift) && _vAxis == 1;
        _isCrouching = !_isRunning && Input.GetKey(KeyCode.LeftControl);

        Vector3 move = (_isRunning ? 2 : _isCrouching ? 0.5f : 1) * (transform.right * _hAxis + transform.forward * _vAxis).normalized;
        _rigidbody.linearVelocity = move * walkSpeed * 100 * Time.fixedDeltaTime;

        float axis = (_isRunning ? 2 : 1) * (_vAxis != 0 ? Mathf.Sign(_vAxis) : _hAxis != 0 ? -1 : 0);

        SetIntegerServerRpc("Axis", (int)axis);
        SetBooleanServerRpc("IsCrouching", _isCrouching);

        bool isRolling = Input.GetButtonDown("Jump");
        if (isRolling) RollForwardServerRpc();
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
        SetTriggerClientRpc("OnRollForward");
        yield return new WaitForSeconds(0.4f);
        _canMove = true;
    }

    private void HandleAttack()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            SetTriggerServerRpc("OnPunch");
            Bullet p = Instantiate(punch, transform.position, transform.rotation).GetComponent<Bullet>();
            p.Init(gameObject, p.transform.forward * 10);
        }
    }

    public override void OnNetworkPreDespawn()
    {
        if (IsOwner) _cam.transform.parent = null;

        base.OnNetworkPreDespawn();
    }

    public override void OnNetworkDespawn()
    {
        Debugger.Log("Despawning...", IsServer);
        base.OnNetworkDespawn();
    }
}
