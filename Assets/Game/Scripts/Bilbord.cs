using UnityEngine;

public class Bilbord : MonoBehaviour
{
    [SerializeField] private Transform player;

    private void Update()
    {
        transform.LookAt(player.position, player.up);
    }
}