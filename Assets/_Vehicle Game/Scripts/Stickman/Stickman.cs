using UnityEngine;

public class Stickman : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Stickman collided with {other.gameObject.name}!");
        if (other.CompareTag("Player"))
        {
            Debug.Log("Stickman collided with Player!");
            Player player = other.GetComponent<Player>();
            Debug.Log($"Player Position: {player.Position}");
        }
    }
}
