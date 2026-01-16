using UnityEngine;
using UnityEngine.InputSystem;

public class Inputs : MonoBehaviour
{/*
    private InputSystem_Actions actions;
    [SerializeField] PlayerController playerController;

    private void StartAxis(InputAction.CallbackContext context)
    {
        Vector2 axis = context.ReadValue<Vector2>();
        playerController. = axis;
    }
    private void StopAxis(InputAction.CallbackContext context)
    {
        playerController.Axis = Vector3.zero;
    }

    private void OnEnable()
    {
        actions.Enable();
        actions.Player.Move.performed += StartAxis;
        actions.Player.Move.canceled += StopAxis;
    }
    private void OnDisable()
    {
        actions.Disable();
        actions.Player.Move.performed -= StartAxis;
        actions.Player.Move.canceled -= StopAxis;
    }
    private void Awake()
    {
        actions = new();
    }*/
}
