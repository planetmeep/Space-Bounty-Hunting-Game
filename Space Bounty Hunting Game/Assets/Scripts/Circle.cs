using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Circle : MonoBehaviour
{
    public LineRenderer circleRenderer;
    void Start()
    {
        DrawCircle(200, 100);
    }
    void DrawCircle(int steps, float radius)
    {
        circleRenderer.positionCount = steps;

        for (int currentStep = 0; currentStep < steps; currentStep++)
        {
            float circumferenceProgress = (float)currentStep / (steps - 2);

            float currentRadian = circumferenceProgress * 2 * Mathf.PI;

            float xScaled = Mathf.Cos(currentRadian);
            float yScaled = Mathf.Sin(currentRadian);

            float x = radius * xScaled;
            float y = radius * yScaled;
            float z = 0;

            Vector3 currentPosition = new Vector3(x, y, z);

            circleRenderer.SetPosition(currentStep, currentPosition);
        }
    }
}
