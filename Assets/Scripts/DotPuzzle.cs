using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.EventSystems;


[RequireComponent(typeof(LineRenderer))]
public class DotPuzzle : MonoBehaviour
{
    public LayerMask ColliderLayer;
    public List<GameObject> CorrectDots = new List<GameObject>();
    [SerializeField] private Color _patternCorrectColor;
    [SerializeField] private Color _patternWrongColor;

    private LineRenderer _lineRenderer;
    private Vector3 _startPos;
    private bool _drawing = false;
    private int _lineRendererIndex = 0;
    private List<RaycastResult> _results = new List<RaycastResult>();
    private List<GameObject> _dots = new List<GameObject>();


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

                            _lineRenderer.startColor = Color.white;
                            _lineRenderer.endColor = Color.white;

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
                        _drawing = false;

                        if (IsPatternCorrect())
                        {
                            PuzzleCompleted();
                        }
                        else
                        {
                            StartCoroutine(PatternWrong());

                            _dots.Clear();
                        }

                    }
                    break;
            }
        }
    }

    void PuzzleCompleted()
    {
        _lineRenderer.startColor = _patternCorrectColor;
        _lineRenderer.endColor = _patternCorrectColor;
        _lineRenderer.positionCount--;
    }

    IEnumerator PatternWrong()
    {
        _lineRenderer.startColor = _patternWrongColor;
        _lineRenderer.endColor = _patternWrongColor;
        _lineRenderer.positionCount--;

        yield return new WaitForSeconds(1f);

        _lineRenderer.enabled = false;
        _lineRendererIndex = 0;
        _lineRenderer.positionCount = 1;

    }

    bool IsPatternCorrect()
    {
        if (_dots.Count != CorrectDots.Count)
            return false;

        for (int i = 0; i < _dots.Count; i++)
        {
            if (_dots[i] != CorrectDots[i])
            {
                return false;
            }
        }

        return true;
    }
}
