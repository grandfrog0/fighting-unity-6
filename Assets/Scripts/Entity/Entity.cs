using Unity.Netcode;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class Entity : NetworkBehaviour
{
    public float currentHealth;
    public float maxHealth = 100;
    private bool _isDead = false;
    public float HealthPercent => currentHealth / maxHealth;

    public int ChosenElixir;

    public float damage1;
    public float damage2;
    public float cooldown1;
    public float cooldown2;
    public float stunningProbability;
    public Vector2 stunningTimeRange;

    public void TakeDamage(float value)
    {
        TakeDamageServerRpc(value);
    }

    [ServerRpc(RequireOwnership = false)]
    private void TakeDamageServerRpc(float value)
    {
        TakeDamageClientRpc(value);
    }

    [ClientRpc]
    private void TakeDamageClientRpc(float value)
    {
        currentHealth -= value;
        Debugger.Log(currentHealth, IsServer);

        switch (ChosenElixir)
        {
            case 0:
                if (HealthPercent < 20)
                {
                    //
                }
                break;
        }

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
            e.TakeDamage(damage1);
    }

    [ClientRpc]
    public void InitClientRpc(float health, float damage1, float damage2, float cooldown1, float cooldown2, float stunningProbability, Vector2 stunningTimeRange)
    {
        currentHealth = maxHealth = health;
        this.damage1 = damage1;
        this.damage2 = damage2;
        this.cooldown1 = cooldown1;
        this.cooldown2 = cooldown2;
        this.stunningProbability = stunningProbability;
        this.stunningTimeRange = stunningTimeRange;

    }
}
