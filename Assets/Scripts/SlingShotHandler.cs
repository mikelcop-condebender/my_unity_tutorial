using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class SlingShotHandler : MonoBehaviour
{

    [Header("Line Renderers")]
    [SerializeField] private LineRenderer leftLineRenderer;
    [SerializeField] private LineRenderer rightLineRenderer;

    [Header("Transform Reference")]
    [SerializeField] private Transform leftStartPosition;
    [SerializeField] private Transform rightStartPosition;
    [SerializeField] private Transform centerPosition;
    [SerializeField] private Transform idlePosition;

    [SerializeField] private Transform _elasticTransform;

    [Header("Sling Shot Stats")]
    [SerializeField] private float maxDistance = 3.5f;
    [SerializeField] private float _shotForce = 5f;
    [SerializeField] private float _timeBetweenReSpawns = 2f;
    [SerializeField] private float _elasticDivider = 1.2f;
    [SerializeField] private AnimationCurve _elasticCurve;

    [Header("Scripts")]
    [SerializeField] private SlingShotArea slingShotArea;

    [Header("Bird")]
    [SerializeField] AngieBird _angieBirdPrefab;
    [SerializeField] private float _angieBirdPositionOffset = 2f;

    [Header("Sound")]
    [SerializeField] private AudioClip _elasticPulledClip;
    [SerializeField] private AudioClip[] _elasticReleasedClips;



    private AngieBird _spawnedAngieBird;

    private Vector2 slingShotLinesPosition;

    private Vector2 _direction;
    private Vector2 _directionNormalized;
    private bool clickedWithinArea;
    private bool _birdOnSlingShot;
    private AudioSource _audioSource;


    private void Awake()
    {

        _audioSource = GetComponent<AudioSource>();
        if (leftLineRenderer.enabled && rightLineRenderer.enabled)
        {
            leftLineRenderer.enabled = false;
            rightLineRenderer.enabled = false;
        }
        SpawnAngieBird();
    }

    private void Update()
    {
        if (InputManager.WasLeftMouseButtonPressed && slingShotArea.isWithinSlingShotArea())
        {
            clickedWithinArea = true;
            if(_birdOnSlingShot){
                SoundManager.instance.PlayClip(_elasticPulledClip, _audioSource);
            }
        }
        if (InputManager.isLeftMousePressed && clickedWithinArea && _birdOnSlingShot)
        {
            DrawSlingShot();
            PositionAndRotateAngieBird();
        }

        if (InputManager.WasLeftMouseButtonReleased && _birdOnSlingShot)
        {
            if (GameManager.instance.HasEnoughShot() && clickedWithinArea)
            {
                clickedWithinArea = false;
                _birdOnSlingShot = false;
                _spawnedAngieBird.LaunchBird(_direction, _shotForce);
                SoundManager.instance.PlayRandomClip(_elasticReleasedClips, _audioSource);
                GameManager.instance.UseShot();
                AnimateSlingShot();
                if (GameManager.instance.HasEnoughShot())
                {
                    StartCoroutine(SpawnAngieBirdAfterTime());
                }


            }
        }

    }

    #region Sling Shot Methods
    private void DrawSlingShot()
    {
        Vector3 touchPosition = Camera.main.ScreenToWorldPoint(InputManager.MousePosition);
        slingShotLinesPosition = centerPosition.position + Vector3.ClampMagnitude(touchPosition - centerPosition.position, maxDistance);
        SetLines(slingShotLinesPosition);
        _direction = (Vector2)centerPosition.position - slingShotLinesPosition;
        _directionNormalized = _direction.normalized;
    }

    private void SetLines(Vector2 touchPosition)
    {
        if (!leftLineRenderer.enabled && !rightLineRenderer.enabled)
        {
            leftLineRenderer.enabled = true;
            rightLineRenderer.enabled = true;
        }

        leftLineRenderer.SetPosition(0, touchPosition);
        leftLineRenderer.SetPosition(1, leftStartPosition.position);

        rightLineRenderer.SetPosition(0, touchPosition);
        rightLineRenderer.SetPosition(1, rightStartPosition.position);
    }
    #endregion

    #region Angie Bird Methods
    private void SpawnAngieBird()
    {
        SetLines(idlePosition.position);
        Vector2 dir = (centerPosition.position - idlePosition.position).normalized;
        Vector2 spawnPosition = (Vector2)idlePosition.position + dir * _angieBirdPositionOffset;
        _spawnedAngieBird = Instantiate(_angieBirdPrefab, spawnPosition, Quaternion.identity);
        _spawnedAngieBird.transform.right = dir;
        _birdOnSlingShot = true;

    }

    private void PositionAndRotateAngieBird()
    {
        _spawnedAngieBird.transform.position = slingShotLinesPosition + _directionNormalized * _angieBirdPositionOffset;
        _spawnedAngieBird.transform.right = _directionNormalized;
    }

    private IEnumerator SpawnAngieBirdAfterTime()
    {
        yield return new WaitForSeconds(_timeBetweenReSpawns);
        SpawnAngieBird();
    }

    #endregion

    private void AnimateSlingShot()
    {
        
        _elasticTransform.position = leftLineRenderer.GetPosition(0);
        float dist = Vector2.Distance(_elasticTransform.position, centerPosition.position);
        float time = dist / _elasticDivider;

        _elasticTransform.DOMove(centerPosition.position, time).SetEase(_elasticCurve);
        StartCoroutine(AnimateSlingShotLines(_elasticTransform, time));
    }

    private IEnumerator AnimateSlingShotLines(Transform trans, float time)
    {
        float elapseTime = 0f;
        while(elapseTime < time)
        {
            elapseTime += Time.deltaTime;
            SetLines(trans.position);
            yield return null;
        }
    }

}

