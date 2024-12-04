using UnityEngine;
using UnityEngine.InputSystem;

public class SlingShotHandler : MonoBehaviour
{
    [SerializeField] private LineRenderer leftLineRenderer;
    [SerializeField] private LineRenderer rightLineRenderer;
    [SerializeField] private Transform leftStartPosition;
    [SerializeField] private Transform rightStartPosition;
    [SerializeField] private Transform centerPosition;
    [SerializeField] private Transform idlePosition;

    [SerializeField] private float maxDistance = 3.5f;
    private Vector2 slingShotLinesPosition;


   void Update()
    {
       if(Mouse.current.leftButton.isPressed){
        DrawSlingShot();
       }
        
    }

    private void DrawSlingShot()
    {
        Vector3 touchPosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        slingShotLinesPosition = centerPosition.position + Vector3.ClampMagnitude(touchPosition - centerPosition.position, maxDistance);
        SetLines(slingShotLinesPosition);
    }

    private void SetLines(Vector2 touchPosition){
        leftLineRenderer.SetPosition(0, touchPosition);
        leftLineRenderer.SetPosition(1, leftStartPosition.position);

        rightLineRenderer.SetPosition(0, touchPosition);
        rightLineRenderer.SetPosition(1, rightStartPosition.position);
    }
}
