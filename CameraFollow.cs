using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public Vector3 offset;

    void Start()
    {
        if (player == null)
        {
            // Cherche automatiquement le joueur avec le tag "Player"
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }

        if (player != null)
            offset = transform.position - player.position;
    }

    void LateUpdate()
    {
        if (player != null)
            transform.position = player.position + offset;
    }
}