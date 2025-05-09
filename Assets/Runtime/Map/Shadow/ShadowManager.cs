using EditorExtend.GridEditor;
using System.Collections.Generic;
using UnityEngine;

public class ShadowManager : MonoBehaviour
{
    public static int SampleDistance = 20;
    public static Vector3 CellSize = new(1f, 1f, 0.5f);

    public static Vector3Int[] XYDirections =
    {
        Vector3Int.up,
        Vector3Int.left + Vector3Int.up,
        Vector3Int.left,
        Vector3Int.left + Vector3Int.down,
        Vector3Int.down,
        Vector3Int.right + Vector3Int.down,
        Vector3Int.right,
        Vector3Int.right + Vector3Int.up,
    };

    public static Vector3Int[] XZDirections =
    {
        Vector3Int.forward,
        Vector3Int.right + Vector3Int.forward,
        Vector3Int.right,
        //Vector3Int.right + Vector3Int.back,
        //Vector3Int.back,
        //Vector3Int.left + Vector3Int.back,
        Vector3Int.left,
        Vector3Int.left + Vector3Int.forward,
    };

    public static Vector3Int[] YZDirections =
    {
        Vector3Int.forward,
        Vector3Int.up + Vector3Int.forward,
        Vector3Int.up,
        //Vector3Int.up + Vector3Int.back,
        //Vector3Int.back,
        //Vector3Int.down + Vector3Int.back,
        Vector3Int.down,
        Vector3Int.down + Vector3Int.forward,
    };

    private IsometricGridManager igm;
    private readonly HashSet<Vector3Int> objectCache = new();
    private readonly Dictionary<ShadowVertex, float> radianceCache = new();

    public float radiance_up = 1f;
    public float radiance_left = 0.3f;
    public float radiance_right = 0.7f;
    public float projectShadowIntensity = 0.5f;

    public float GetRadiance(ShadowVertex vertex)
    {
        if (!radianceCache.ContainsKey(vertex))
            radianceCache.Add(vertex, AmbientOcculasion(vertex));
        return radianceCache[vertex];
    }

    public float GetProjectRadiance()
    {
        return projectShadowIntensity * radiance_up;
    }

    public bool VisibleCheck(ShadowVertex vertex)
    {
        if (igm.ObjectDict.TryGetValue(vertex.cellPosition + vertex.cellNormal, out GridObject gridObject))
        {
            if (gridObject.IsGround)
                return false;
        }
        return true;
    }

    public float AmbientOcculasion(ShadowVertex vertex)
    {
        Vector3Int[] directions;
        float visibility = 0f;
        if (vertex.cellNormal.x != 0)
            directions = YZDirections;
        else if (vertex.cellNormal.y != 0)
            directions = XZDirections;
        else if (vertex.cellNormal.z != 0)
            directions = XYDirections;
        else
            return 0f;

        for (int i = 0; i < directions.Length; i++)
        {
            visibility += AmbientOcculasion(vertex, directions[i]);
        }
        visibility /= directions.Length;
        float radiance;
        if (vertex.cellNormal == Vector3Int.forward)
            radiance = radiance_up;
        else if (vertex.cellNormal == Vector3Int.left)
            radiance = radiance_left;
        else if (vertex.cellNormal == Vector3Int.down)
            radiance = radiance_right;
        else
            throw new System.ArgumentException();
        return radiance * visibility;
    }

    private float AmbientOcculasion(ShadowVertex vertex, Vector3Int direction)
    {
        float minVisibility = 1f;
        Vector3Int basePosition = vertex.cellPosition;
        float unitDistance = new Vector3(CellSize.x * direction.x, CellSize.y * direction.y, CellSize.z * direction.z).magnitude;
        float unitHeight = vertex.cellNormal.z != 0 ? 0.5f : 1f;
        for (int i = 0; i < SampleDistance; i++)
        {
            basePosition += direction;
            float h = GetHeight(basePosition, vertex.cellNormal) * unitHeight;
            float d = (i + 0.5f) * unitDistance;
            if (h > 0)
            {
                float angle = Mathf.Atan(h / d);
                minVisibility = Mathf.Min(minVisibility, 1f - angle * 2 / Mathf.PI);
            }
        }
        return minVisibility;
    }

    private float GetHeight(Vector3Int basePosition, Vector3Int normal)
    {
        int h = 0;
        while (objectCache.Contains(basePosition))
        {
            basePosition += normal;
            h++;
        }
        return h - 1;
    }

    private void Awake()
    {
        igm = GetComponent<IsometricGridManager>();
        FaceShadow[] shadows = GetComponentsInChildren<FaceShadow>();
        for (int i = 0; i < shadows.Length; i++)
        {
            Vector3Int cellPosition = igm.WorldToCell(shadows[i].transform.position);
            objectCache.Add(cellPosition);
        }
    }
}
