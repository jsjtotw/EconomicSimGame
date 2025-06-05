using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class GraphPlotter : MonoBehaviour
{
    public RectTransform graphArea;
    public int maxPoints = 30;
    private LineRenderer lineRenderer;

    public List<float> dataPoints = new List<float>();

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
        lineRenderer.useWorldSpace = false;
    }

    public void UpdateGraph(List<float> values)
    {
        dataPoints = values;

        float width = graphArea.rect.width;
        float height = graphArea.rect.height;

        int count = Mathf.Min(values.Count, maxPoints);
        lineRenderer.positionCount = count;

        float xStep = width / Mathf.Max(count - 1, 1);
        float maxY = Mathf.Max(1f, Mathf.Max(values.ToArray()));

        for (int i = 0; i < count; i++)
        {
            float x = i * xStep;
            float y = (values[i] / maxY) * height;
            lineRenderer.SetPosition(i, new Vector3(x, y, 0));
        }
    }
}
