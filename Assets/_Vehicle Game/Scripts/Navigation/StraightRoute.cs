using UnityEngine;

public readonly struct StraightRoute
{
    public Vector3 Start { get; }
    public Vector3 Direction { get; }
    public Vector3 Right => Vector3.Cross(Vector3.up, Direction);
    public float Length { get; }

    public StraightRoute(Vector3 start, Vector3 finish)
    {
        Start = start;
        Vector3 offset = finish - start;
        offset.y = 0f;
        Length = offset.magnitude;
        Direction = offset.normalized;
    }

    public float GetDistance(Vector3 position)
    {
        return Vector3.Dot(position - Start, Direction);
    }

    public Vector3 GetPosition(float distance, float sideOffset)
    {
        return Start + Direction * distance + Right * sideOffset;
    }
}
