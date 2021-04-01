using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class LoadedBall : MonoBehaviour
{
    [SerializeField]
    private Slingshot _slingshot;

    private void Start()
    {
        if (!_slingshot)
        {
            _slingshot = FindObjectOfType<Slingshot>();
        }
    }

    public void OnMouseDown()
    {
        _slingshot.StartAimBall();
    }

    private void OnMouseDrag()
    {
        Vector2 slingshotPos = _slingshot.transform.position;
        Vector2 newPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Vector2.Distance(slingshotPos, newPoint) > _slingshot.MAXTension)
        {
            transform.position = slingshotPos + (newPoint - slingshotPos).normalized * _slingshot.MAXTension;
        }
        else
        {
            transform.position = newPoint;
        }
    }

    private void OnMouseUp()
    {
        _slingshot.Fire();
    }
}