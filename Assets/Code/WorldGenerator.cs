using UnityEngine;
using System.Collections;

public class WorldGenerator : MonoBehaviour
{
    public GameObject Voxel;

    public float SizeX;
    public float SizeZ;
    public float SizeY;

    void Start()
    {
        StartCoroutine(SimpleGenerator());
    }

    public static void CloneAndPlace(Vector3 newPosition, GameObject originalGameobject)
    {
        GameObject clone = (GameObject)Instantiate(originalGameobject, newPosition, Quaternion.identity);
        clone.transform.position = newPosition;
        clone.name = "Cube@" + clone.transform.position;
    }

    IEnumerator SimpleGenerator()
    {
        uint numberOfInstances = 0;
        uint instancesPerFrame = 50;
        var halfX = (int)(SizeX / 2);
        var halfZ = (int)(SizeZ / 2);
        var halfY = (int)(SizeY / 2);

        for (int x = -1 * halfX; x <= halfX; x++)
        {
            for (int z = -1 * halfZ; z <= halfZ; z++)
            {
                for (int y = -1 * halfY; y <= halfY; y++)
                {
                    var random = Random.Range(0.0f, 1.0f);
                    if (random > 0.1f)
                    {
                        Vector3 newPosition = new Vector3(x, y, z);
                        CloneAndPlace(newPosition, Voxel);
                        numberOfInstances++;

                        if (numberOfInstances == instancesPerFrame)
                        {
                            numberOfInstances = 0;
                            yield return new WaitForEndOfFrame();
                        }
                    }
                }
            }
        }
    }
}