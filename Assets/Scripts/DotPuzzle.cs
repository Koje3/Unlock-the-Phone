using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


[RequireComponent(typeof(LineRenderer))]
public class DotPuzzle : MonoBehaviour
{
    public LayerMask ColliderLayer;
    public List<GameObject> CorrectDots = new List<GameObject>();
    public GameObject LinePointPrefab;
    [SerializeField] private Color _patternCorrectColor;
    [SerializeField] private Color _patternWrongColor;

    //Siirrä tämä PuzzleManageriin
    public Animator PuzzleAnimator;
    public Animator BackgroundAnimator;

    private LineRenderer _lineRenderer;
    private Vector3 _startPos;
    private bool _currentlyDrawingPattern = false;
    private bool _canDrawPattern = false;
    private int _lineRendererIndex = 0;
    private List<RaycastResult> _results = new List<RaycastResult>();
    private List<GameObject> _hitDots = new List<GameObject>();
    private GameObject _linePointsContainer;
    private LineRenderer _currentLinerenderer;



    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.positionCount = 1;
        _lineRenderer.enabled = false;

        _canDrawPattern = true;
        _linePointsContainer = new GameObject("LinePoints");
        _linePointsContainer.transform.parent = this.transform;
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
   

                    if (_canDrawPattern && _results.Count > 0)
                    {
                        // Get the first hit object
                        GameObject hitObject = _results[0].gameObject;

                        Debug.Log("Hit a UI element: " + hitObject.name);

                        if (_hitDots.Contains(hitObject) == false)
                        {
                            StartCoroutine(VibratePhone(0.1f));

                            SelectDot(hitObject);
                            AddNewLineRenderer(hitObject);

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

                    if (_currentlyDrawingPattern && _currentLinerenderer != null)
                    {
                        _currentLinerenderer.SetPosition(1, Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, 5f)));

                    }

                    break;

                case TouchPhase.Ended:
                    if (_currentlyDrawingPattern)
                    {
                        _currentlyDrawingPattern = false;
                        _canDrawPattern = false;
                        
                        if (_currentLinerenderer != null)
                        {
                            _currentLinerenderer.enabled = false;
                        }

                        if (IsPatternCorrect())
                        {
                            PuzzleCompleted();
                        }
                        else
                        {
                            StartCoroutine(PatternWrong());                           
                        }

                    }
                    break;
            }
        }
    }

    void SelectDot(GameObject dot)
    {
        _hitDots.Add(dot);

        if (dot.GetComponent<Animator>() != null)
        {
            dot.GetComponent<Animator>().SetTrigger("Selected");
        }
    }

    void AddNewLineRenderer(GameObject dot)
    {

        Vector3 hitObjectPosition = dot.transform.position;

        _currentlyDrawingPattern = true;

        GameObject newLinePoint = Instantiate(LinePointPrefab, hitObjectPosition, Quaternion.identity);
        newLinePoint.transform.parent = _linePointsContainer.transform;
        newLinePoint.name = "LinePoint" + _lineRendererIndex;

        LineRenderer newLineRend = newLinePoint.GetComponent<LineRenderer>();
        newLineRend.enabled = true;
        newLineRend.positionCount = 2;
        newLineRend.SetPosition(0, hitObjectPosition);

        if (_currentLinerenderer != null)
        {
            _currentLinerenderer.SetPosition(1, hitObjectPosition);
        }

        _lineRendererIndex++;
        _currentLinerenderer = newLineRend;
    }

    IEnumerator VibratePhone(float time)
    {
        Handheld.Vibrate();

        yield return new WaitForSeconds(time);
    }

    void PuzzleCompleted()
    {
        StartCoroutine(LineCompleted());

        if (PuzzleAnimator != null)
        {
            PuzzleAnimator.SetTrigger("Completed");
        }

        if (BackgroundAnimator != null)
        {
            BackgroundAnimator.SetTrigger("Completed");
        }

        foreach (Transform child in this.transform)
        {
            if (child.GetComponent<Animator>() != null)
            {
                child.GetComponent<Animator>().SetTrigger("Completed");
            }

        }

        StartCoroutine(VibratePhone(2f));
    }

    IEnumerator LineCompleted()
    {
        List<LineRenderer> lineRenderers = new List<LineRenderer>();

        foreach (Transform child in _linePointsContainer.transform)
        {
            LineRenderer rend = child.GetComponent<LineRenderer>();
            if (rend != null) 
            {
                lineRenderers.Add(rend);
                rend.startColor = _patternCorrectColor;
                rend.endColor = _patternCorrectColor;
            }
        }

        float waitTime = 0.5f;
        float elapsedTime = 0f;

        Color lerpEndColor = new Color(_patternCorrectColor.r, _patternCorrectColor.g, _patternCorrectColor.b, 0f);

        while (elapsedTime < waitTime)
        {

            Color newColor = Color.Lerp(_patternCorrectColor, lerpEndColor, elapsedTime/waitTime);

            foreach (LineRenderer rend in lineRenderers)
            {
                rend.material.SetColor("_Color", newColor);
            }

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        //Make sure the color changes all the way
        foreach (LineRenderer rend in lineRenderers)
        {
            rend.material.SetColor("_Color", lerpEndColor);
        }
    }
   

    IEnumerator PatternWrong()
    {
        foreach (Transform child in _linePointsContainer.transform)
        {
            LineRenderer rend = child.GetComponent<LineRenderer>();
            rend.startColor = _patternWrongColor;
            rend.endColor = _patternWrongColor;
        }

        List<Color> originalColors = new List<Color>();

        foreach (GameObject dot in _hitDots)
        {
            if (dot.GetComponent<Image>() != null)
            {
                Image dotImage = dot.GetComponent<Image>();
                originalColors.Add(dotImage.color);
                dotImage.color = _patternWrongColor;
            }
        }


        yield return new WaitForSeconds(1f);


        foreach (Transform child in _linePointsContainer.transform)
        {
            Destroy(child.gameObject);
        }

        int index = 0;

        foreach (GameObject dot in _hitDots)
        {
            if (dot.GetComponent<Image>() != null)
            {
                dot.GetComponent<Image>().color = originalColors[index];
            }

            if (dot.GetComponent<Animator>() != null)
            {
                dot.GetComponent<Animator>().SetTrigger("UnSelected");
            }



            index++;

        }

        _hitDots.Clear();
        _canDrawPattern = true;


    }

    bool IsPatternCorrect()
    {
        if (_hitDots.Count != CorrectDots.Count)
            return false;

        for (int i = 0; i < _hitDots.Count; i++)
        {
            if (_hitDots[i] != CorrectDots[i])
            {
                return false;
            }
        }

        return true;
    }
}
