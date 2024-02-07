using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.EventSystems;

public class DotPuzzle : MonoBehaviour
{
    public LayerMask colliderLayer;

    private LineRenderer _lineRenderer;
    private Vector3 _startPos;
    private bool _drawing = false;
    private List<RaycastResult> _results = new List<RaycastResult>();
    private List<GameObject> _dots = new List<GameObject>();
    private int _lineRendererIndex = 0;


    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.positionCount = 1;
        _lineRenderer.enabled = false;
    }


    void Update()
    {

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            Ray ray = Camera.main.ScreenPointToRay(touch.position);

            // Create a pointer event data
            PointerEventData eventData = new PointerEventData(EventSystem.current);

            // Set the raycast position
            eventData.position = touch.position;

            // Perform the raycast
            EventSystem.current.RaycastAll(eventData, _results);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                

                    break;

                case TouchPhase.Moved:
   

                    if (_results.Count > 0)
                    {
                        // Get the first hit object
                        GameObject hitObject = _results[0].gameObject;

                        Debug.Log("Hit a UI element: " + hitObject.name);

                        if (_dots.Contains(hitObject) == false)
                        {
                            _dots.Add(hitObject);

                            _startPos = hitObject.transform.position;
                            _drawing = true;
                            _lineRenderer.enabled = true;

                            _lineRenderer.SetPosition(_lineRendererIndex, _startPos);
                            _lineRendererIndex++;
                            _lineRenderer.positionCount = _lineRendererIndex + 1;
                        }
                        else
                        {
                            Debug.Log("UI element: " + hitObject.name + " already hit");
                        }
                            

  

                    }
                    else
                    {
                        // Ray did not hit any UI element
                        Debug.Log("Did not hit a UI element.");
                    }

                    if (_drawing)
                    {
                        _lineRenderer.SetPosition(_lineRendererIndex, Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, 5f)));

                    }

                    break;

                case TouchPhase.Ended:
                    if (_drawing)
                    {
                        _lineRenderer.enabled = false;
                        _drawing = false;
                        _lineRendererIndex = 0;
                        _lineRenderer.positionCount = 1;

                        _dots.Clear();
                    }
                    break;
            }
        }
    }
}
