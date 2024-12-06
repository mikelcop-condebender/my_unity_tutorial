using Unity.VisualScripting;
using UnityEngine;

public class Baddie : MonoBehaviour
{
    [SerializeField] private float _maxHealth = 3f;
    [SerializeField] private float _damageThreshold = 0.5f;
    private float _currentHealth;

    private void Awake()
    {
        _currentHealth = _maxHealth;
    }

    public void DamageBaddie(float damageAmount)
    {
        _currentHealth -= damageAmount;

        if(_currentHealth < _maxHealth)
        {
            Die();
        }
    }

    private void Die()
    {
        GameManager.instance.RemoveBaddie(this);
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision){
        float impactVelocity = collision.relativeVelocity.magnitude;
        if(impactVelocity > _damageThreshold)
        {
            DamageBaddie(impactVelocity);
        }
    }

}
