using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
[RequireComponent(typeof(NavMeshSurface))]
public class LevelNavigation : MonoBehaviour
{
    private const float SampleDistance = 0.1f;
    private const float MaxHorizontalOffset = 0.05f;
    private NavMeshSurface _surface;

    public void Initialize()
    {
        _surface = GetComponent<NavMeshSurface>();
        _surface.BuildNavMesh();
    }

    public bool TryGetPosition(Vector3 position, out Vector3 result)
    {
        result = position;

        NavMeshQueryFilter filter = new NavMeshQueryFilter
        {
            agentTypeID = _surface.agentTypeID,
            areaMask = NavMesh.AllAreas
        };

        if (!NavMesh.SamplePosition(position, out NavMeshHit hit, SampleDistance, filter))
            return false;

        Vector3 offset = hit.position - position;
        offset.y = 0f;

        if (offset.sqrMagnitude > MaxHorizontalOffset * MaxHorizontalOffset)
            return false;

        result.y = hit.position.y;
        return true;
    }
}