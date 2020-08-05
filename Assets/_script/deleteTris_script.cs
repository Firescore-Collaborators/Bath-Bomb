using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class deleteTris_script : MonoBehaviour
{

    void DeleteTri(int index)
    {
        Destroy(this.gameObject.GetComponent<MeshCollider>());
        Mesh mesh = transform.GetComponent<MeshFilter>().mesh;
        int[] oldTriangles = mesh.triangles;
        int[] newTriangles = new int[mesh.triangles.Length - 3];

        int i = 0;
        int j = 0;
        while(j < mesh.triangles.Length)
        {
            if (j != index * 3)
            {
                newTriangles[i++] = oldTriangles[j++];
                newTriangles[i++] = oldTriangles[j++];
                newTriangles[i++] = oldTriangles[j++];
            }
            else
                j += 3;
        }
        transform.GetComponent<MeshFilter>().mesh.triangles = newTriangles;
        this.gameObject.AddComponent<MeshCollider>();

    }

    public Texture aaas;
    void Update()
    {
        RaycastHit hit;
        if (Input.GetMouseButton(0))
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            {
                if (hit.transform.gameObject.tag == "DeformableMesh")
                {
                    DeleteTri(hit.triangleIndex);
                }
            }
        }
    }
}
