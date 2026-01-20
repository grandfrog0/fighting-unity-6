using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float damage = 20f;
    [SerializeField] float distance = 1f;
    private Rigidbody _rigidbody;
    private GameObject _parent;
    public void Init(GameObject parent, Vector3 direction)
    {
        _rigidbody = GetComponent<Rigidbody>();
        _parent = parent;

        transform.forward = direction;
        _rigidbody.linearVelocity = direction;

        StartCoroutine(ShootRoutine());
    }
    private IEnumerator ShootRoutine()
    {
        yield return new WaitForSeconds(distance / _rigidbody.linearVelocity.magnitude);
        Destroy(gameObject);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != _parent && other.TryGetComponent(out Entity entity))
        {
            entity.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
