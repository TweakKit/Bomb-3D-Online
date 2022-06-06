using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// This mesh combiner helps combine different meshes into one mesh for reducing the amount of batches/draw calls, improving the game performance.
/// </summary>
public class MeshCombiner : MonoBehaviour
{
    #region Class Methods

    public void CombineMeshes()
    {
        MeshFilter[] meshFilters = transform.GetComponentsInChildren<MeshFilter>();
        GameObject[] _combinedObjects = new GameObject[meshFilters.Length];
        ArrayList materials = new ArrayList();
        ArrayList combineInstanceArrays = new ArrayList();

        for (int i = 0; i < meshFilters.Length; i++)
        {
            _combinedObjects[i] = meshFilters[i].gameObject;
            if (meshFilters[i].sharedMesh == null)
            {
                Debug.LogWarning("Mesh Filter has no Shared Mesh, mesh will not be included in combine.");
                break;
            }

            MeshFilter[] innerMeshFilters = meshFilters[i].GetComponentsInChildren<MeshFilter>();
            foreach (MeshFilter innerMeshFilter in innerMeshFilters)
            {
                MeshRenderer innerMeshRenderer = innerMeshFilter.GetComponent<MeshRenderer>();
                if (innerMeshRenderer == null)
                {
                    Debug.LogWarning("Mesh Filter has no Mesh Renderer, mesh will not be included in combine.");
                    break;
                }

                for (int j = 0; j < innerMeshFilter.sharedMesh.subMeshCount; j++)
                {
                    if (j < innerMeshRenderer.sharedMaterials.Length && j < innerMeshFilter.sharedMesh.subMeshCount)
                    {
                        int materialArrayIndex = Contains(materials, innerMeshRenderer.sharedMaterials[j]);
                        if (materialArrayIndex == -1)
                        {
                            materials.Add(innerMeshRenderer.sharedMaterials[j]);
                            materialArrayIndex = materials.Count - 1;
                        }

                        combineInstanceArrays.Add(new ArrayList());
                        CombineInstance combineInstance = new CombineInstance();
                        combineInstance.transform = innerMeshRenderer.transform.localToWorldMatrix;
                        combineInstance.subMeshIndex = j;
                        combineInstance.mesh = innerMeshFilter.sharedMesh;
                        ArrayList tempCombineInstanceArrays = combineInstanceArrays[materialArrayIndex] as ArrayList;
                        tempCombineInstanceArrays.Add(combineInstance);
                    }
                    else Debug.LogWarning(innerMeshRenderer.gameObject.name + " is missing a material (Mesh or sub-mesh ignored from combine).");
                }
            }
        }

        Mesh[] meshes = new Mesh[materials.Count];
        CombineInstance[] combineInstances = new CombineInstance[materials.Count];
        for (int m = 0; m < materials.Count; m++)
        {
            CombineInstance[] combineInstanceArray = (combineInstanceArrays[m] as ArrayList).ToArray(typeof(CombineInstance)) as CombineInstance[];
            meshes[m] = new Mesh() { indexFormat = IndexFormat.UInt32 };
            meshes[m].CombineMeshes(combineInstanceArray, true, true);
            combineInstances[m] = new CombineInstance();
            combineInstances[m].mesh = meshes[m];
            combineInstances[m].subMeshIndex = 0;
        }

        gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();
        gameObject.GetComponent<MeshFilter>().sharedMesh = new Mesh();
        gameObject.GetComponent<MeshFilter>().sharedMesh.CombineMeshes(combineInstances, false, false);

        foreach (Mesh mesh in meshes)
        {
            mesh.Clear();
            DestroyImmediate(mesh);
        }

        MeshRenderer meshRendererCombine = gameObject.GetComponent<MeshRenderer>();
        Material[] materialsArray = materials.ToArray(typeof(Material)) as Material[];
        meshRendererCombine.materials = materialsArray;

        for (int i = _combinedObjects.Length - 1; i >= 0; i--)
        {
            if (_combinedObjects[i].transform.parent.GetComponent<Collider>() == null)
                Destroy(_combinedObjects[i].transform.parent.gameObject);
            else
                Destroy(_combinedObjects[i].gameObject);
        }
    }

    private int Contains(ArrayList arrayList, Material n)
    {
        for (int i = 0; i < arrayList.Count; i++)
            if ((arrayList[i] as Material) == n)
                return i;

        return -1;
    }

    #endregion Class Methods
}