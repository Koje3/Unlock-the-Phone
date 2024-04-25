using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(DotMatrixCreator))]
public class DotMatrixCreatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        DotMatrixCreator dotMatrixCreator = (DotMatrixCreator)target;

        if (GUILayout.Button("Create Dot Matrix"))
        {
            dotMatrixCreator.CreateDotMatrix();
        }
    }
}
#endif

[ExecuteInEditMode]
public class DotMatrixCreator : MonoBehaviour
{
    public int horizontalDots = 5;
    public int verticalDots = 5;
    public float dotSize = 20f;
    public float padding = 0f;
    public Sprite dotImage;
    private float _horizontalSpacing;
    private float _verticalSpacing;

    //[ExecuteAlways]
    //private void OnEnable()
    //{
    //    CreateDotMatrix();
    //}

    public void CreateDotMatrix()
    {
        // Clear existing dots
        ClearDotMatrix();

        // Calculate the spacings
        RectTransform rectTransform = GetComponent<RectTransform>();
        _horizontalSpacing = (rectTransform.sizeDelta.x - dotSize * horizontalDots) / (horizontalDots - 1);
        _verticalSpacing = (rectTransform.sizeDelta.y - dotSize * verticalDots) / (verticalDots - 1);

        // Calculate the starting position
        float startX = -(horizontalDots * (dotSize + _horizontalSpacing)) / 2f + (dotSize + _horizontalSpacing) / 2f;
        float startY = (verticalDots * (dotSize + _verticalSpacing)) / 2f - (dotSize + _verticalSpacing) / 2f;

        int dotIndex = 1;
        // Create dots
        for (int row = 0; row < verticalDots; row++)
        {
            for (int col = 0; col < horizontalDots; col++)
            {
                CreateDot(new Vector3(startX + col * (dotSize + _horizontalSpacing), startY - row * (dotSize + _verticalSpacing), 0), dotIndex);
                dotIndex++;
            }
        }
    }

    private void CreateDot(Vector3 position, int index)
    {
        GameObject dot = new GameObject("Dot" + index, typeof(Image));
        dot.transform.SetParent(transform);
        RectTransform rectTransform = dot.GetComponent<RectTransform>();

        rectTransform.localScale = Vector3.one;
        rectTransform.sizeDelta = new Vector2(dotSize, dotSize);
        rectTransform.anchoredPosition = position;
        rectTransform.GetComponent<Image>().raycastPadding = new Vector4(-padding,-padding,-padding,-padding);

        dot.GetComponent<Image>().sprite = dotImage;
    }

    private void ClearDotMatrix()
    {
        foreach (Transform child in transform.Cast<Transform>().ToArray())
        {
            DestroyImmediate(child.gameObject);
        }
    }
}
