using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

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

    [Header("Sling Shot Stats")]
    [SerializeField] private float maxDistance = 3.5f;
    [SerializeField] private float _shotForce = 5f;
    [SerializeField] private float _timeBetweenReSpawns = 2f;

    [Header("Scripts")]
    [SerializeField] private SlingShotArea slingShotArea;

    [Header("Bird")]
    [SerializeField] AngieBird _angieBirdPrefab;
    [SerializeField] float _angieBirdPositionOffset = 2f;

    private AngieBird _spawnedAngieBird;

    private Vector2 slingShotLinesPosition;

    private Vector2 _direction;
    private Vector2 _directionNormalized;
    private bool clickedWithinArea;
    private bool _birdOnSlingShot;

    private void Awake()
    {
        if (leftLineRenderer.enabled && rightLineRenderer.enabled)
        {
            leftLineRenderer.enabled = false;
            rightLineRenderer.enabled = false;
        }
        SpawnAngieBird();
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame && slingShotArea.isWithinSlingShotArea())
        {
            clickedWithinArea = true;
        }
        if (Mouse.current.leftButton.isPressed && clickedWithinArea && _birdOnSlingShot)
        {
            DrawSlingShot();
            PositionAndRotateAngieBird();
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame && _birdOnSlingShot)
        {
            if (GameManager.instance.HasEnoughShot())
            {
                clickedWithinArea = false;
                _spawnedAngieBird.LaunchBird(_direction, _shotForce);
                GameManager.instance.UseShot();
                _birdOnSlingShot = false;
                SetLines(centerPosition.position);
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
        Vector3 touchPosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
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
}
