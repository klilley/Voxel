
using UnityEngine;
using System.Collections;

public class ImprovisedOcclusionCulling : MonoBehaviour
{

    public bool castPositiveMidfieldRays = true;
    public bool castNegativeMidfieldRays = true;
    public bool makeRaysVisible = false;

    public int defaultFarPlane = 100;
    public int minDistance = 20;
    public int maxDistance = 200;
    public int farPlaneBuffer = 10;
    public int rateOfChange = 16;

    // Dense crosshair field. These arrays describe a square grid centered around the crosshair, spaced 0.01 units apart. I pre-calculate it to make things easy.
    private float[] crosshairFieldX = new float[] { 0, -0.02f, 0.02f, 0, 0, -0.01f, -0.01f, 0.01f, 0.01f, -0.02f, -0.02f, 0.02f, 0.02f };
    private float[] crosshairFieldY = new float[] { 0, 0, 0, 0.02f, -0.02f, -0.01f, 0.01f, -0.01f, 0.01f, 0.02f, -0.02f, 0.02f, -0.02f };
    private int crosshairFieldLength;

    // Midfield. These arrays describe a semicircle with equation y=sqrt(0.0125-x^2). The actual calculation is done in Start().
    private float[] midFieldX = new float[] { -0.11f, -0.10f, -0.09f, -0.08f, -0.07f, -0.06f, -0.05f, -0.04f, -0.03f, -0.02f, -0.01f, 0.00f, 0.01f, 0.02f, 0.03f, 0.04f, 0.05f, 0.06f, 0.07f, 0.08f, 0.09f, 0.10f, 0.11f };
    private float[] midFieldY;
    private int midFieldLength;

    void Start()
    {
        Camera.main.farClipPlane = defaultFarPlane;

        crosshairFieldLength = crosshairFieldX.Length;
        midFieldLength = midFieldX.Length;

        midFieldY = new float[midFieldLength];
        for (int i = 0; i < midFieldLength; i++)
        {
            midFieldY[i] = Mathf.Sqrt(0.0125f - (midFieldX[i] * midFieldX[i]));
        }

        StartCoroutine(AdjustFarPlane());
    }

    IEnumerator AdjustFarPlane()
    {
        while (true)
        {
            // Rays are fired in circles and semicircles around the centre of the viewport.
            int distance = minDistance;

            for (int i = 0; i < crosshairFieldLength; i++)
            {
                int tempDist = CastOcclusionRay(crosshairFieldX[i], crosshairFieldY[i]);
                if (tempDist > distance) distance = tempDist;
            }

            yield return 0;

            if (castPositiveMidfieldRays == true)
            {
                for (int i = 0; i < midFieldLength; i++)
                {
                    int tempDist = CastOcclusionRay(midFieldX[i], midFieldY[i]);
                    if (tempDist > distance) distance = tempDist;
                }

                yield return 0;
            }

            if (castNegativeMidfieldRays == true)
            {
                for (int i = 0; i < midFieldLength; i++)
                {
                    int tempDist = CastOcclusionRay(midFieldX[i], -midFieldY[i]);
                    if (tempDist > distance) distance = tempDist;
                }

                yield return 0;
            }

            distance += farPlaneBuffer;

            if (Camera.main.farClipPlane > distance)
            {
                Camera.main.farClipPlane -= rateOfChange;

                if (Camera.main.farClipPlane < distance)
                {
                    Camera.main.farClipPlane = distance;
                }
            }
            else if (Camera.main.farClipPlane < distance)
            {
                Camera.main.farClipPlane += rateOfChange;

                if (Camera.main.farClipPlane > distance)
                {
                    Camera.main.farClipPlane = distance;
                }
            }
        }
    }

    int CastOcclusionRay(float graphX, float graphY)
    {
        int distance = 0;

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f + graphX, 0.5f + graphY, 0));

        if (makeRaysVisible == true) Debug.DrawRay(ray.origin, ray.direction * 10, Color.red);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            distance = (int)hit.distance;
        }
        else
        {
            // No collisions, therefore infinite distance.
            distance = (int)maxDistance;
        }

        return distance;
    }
}
