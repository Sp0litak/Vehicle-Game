using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

public class Player : MonoBehaviour
{
    [Inject] private Health _playerHealth;

    public Vector3 Position => transform.position;
}
