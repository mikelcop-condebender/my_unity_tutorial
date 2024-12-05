using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class SlingShotArea : MonoBehaviour
{
    [SerializeField] private LayerMask slingShotAreaMask;
    public bool isWithinSlingShotArea()
    {
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

         return !!Physics2D.OverlapPoint(worldPosition, slingShotAreaMask);        
         
    }
}
