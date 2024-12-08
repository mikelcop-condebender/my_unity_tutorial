using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class AngieBird : MonoBehaviour
{
    [SerializeField] private AudioClip _hitClip;
    private Rigidbody2D _rb;
    private CircleCollider2D _circleCollider;
    private AudioSource _audioSource;

    private bool _hasBeenLaunched;
    private bool _shouldFaceVelocityDirection;

    private void Awake(){
        _rb = GetComponent<Rigidbody2D>();
        _circleCollider = GetComponent<CircleCollider2D>();      
        _audioSource = GetComponent<AudioSource>(); 
    }

    private void Start()
    {
         _rb.bodyType = RigidbodyType2D.Kinematic;
        _circleCollider.enabled = false;
    }

    private void FixUpdate()
    {
        if(_hasBeenLaunched && _shouldFaceVelocityDirection){
           transform.right = _rb.linearVelocity; 
        }        
    }
    public void LaunchBird(Vector2 direction, float force){
        _rb.bodyType = RigidbodyType2D.Dynamic;
        _circleCollider.enabled = true;

        // apply force
        _rb.AddForce(direction * force, ForceMode2D.Impulse);

        _hasBeenLaunched = true;
        _shouldFaceVelocityDirection = true;
    
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        _shouldFaceVelocityDirection = false;
        SoundManager.instance.PlayClip(_hitClip, _audioSource);
        Destroy(this);
    }
}
