using UnityEngine;

public class ClickOnFaceScript : MonoBehaviour
{
    public Vector3 delta;

    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Left click!");
            Destroy(this.transform.parent.gameObject);
        }

        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("Right click!");
            WorldGenerator.CloneAndPlace(this.transform.parent.transform.position + delta,
            this.transform.parent.gameObject);
        }
    }
}