using Unity.Netcode;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class Entity : NetworkBehaviour
{
    public float currentHealth;
    public float maxHealth = 100;
    private bool _isDead = false;

    public void TakeDamage(float value)
    {
        currentHealth -= value;
        Debugger.Log(currentHealth, IsServer);

        if (currentHealth <= 0 && !_isDead)
        {
            Debugger.Log(IsOwner, IsServer);
            _isDead = true;
            currentHealth = 0;
            if (IsOwner)
            {
                PlayerController controller = GetComponent<PlayerController>();
                controller.isAlive = false;
                controller.SetAnimationBoolean("IsDead", true);
                Camera.main.GetComponent<ObservableCamera>().Target = null;
            }
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Entity e))
            e.TakeDamage(20);
    }

    private void Awake()
    {
        currentHealth = maxHealth;
    }
}
