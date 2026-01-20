using Unity.Netcode;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class Entity : NetworkBehaviour
{
    public float currentHealth;
    public float maxHealth = 100;

    public void TakeDamage(float value)
    {
        currentHealth -= value;
        Debug.Log(currentHealth);

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            GetComponent<PlayerController>().SetAnimationBoolean("IsDead", true);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!IsOwner) return;

        if (other.TryGetComponent(out Entity e))
            e.TakeDamage(20);
    }

    private void Start()
    {
        currentHealth = maxHealth;
    }
}
