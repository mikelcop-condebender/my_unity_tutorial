using Unity.VisualScripting;
using UnityEngine;

public class Baddie : MonoBehaviour
{
    [SerializeField] private float _maxHealth = 3f;
    [SerializeField] private float _damageThreshold = 0.5f;
    [SerializeField] private GameObject _baddieDeathParticle;
    [SerializeField] private AudioClip _deathClip;
    public bool _isFriend = false;
    private float _currentHealth;

    private void Awake()
    {
        _currentHealth = _maxHealth;
    }

    public void DamageBaddie(float damageAmount)
    {
        _currentHealth -= damageAmount;
         Debug.Log($"Current Health: {_currentHealth}, Damage Amount: {damageAmount} Max Health: {_maxHealth}");

        if(_currentHealth <= 0)
        {
            if(_isFriend){
                Debug.Log("Remove one life");
                GameManager.instance.UseShot();
                Die();
            }else{
                Die();
            }
            
        }
    }

    private void Die()
    {
        GameManager.instance.RemoveBaddie(this);
        Instantiate(_baddieDeathParticle, transform.position, Quaternion.identity);
        AudioSource.PlayClipAtPoint(_deathClip, transform.position);
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
