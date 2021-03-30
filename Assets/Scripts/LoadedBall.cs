using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class LoadedBall : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField]
    private Slingshot _slingshot;

    public void OnMouseDown()
    {
        Debug.Log("click");
        _slingshot.StartAimBall();
    }

    private void OnMouseDrag()
    {
        Vector2 newPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = newPoint;
    }

    private void OnMouseUp()
    {
        _slingshot.Fire();
        Debug.Log("up");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("drag");
        _slingshot.StartAimBall();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _slingshot.Fire();
    }

    public void OnDrag(PointerEventData eventData)
    {
    }
}