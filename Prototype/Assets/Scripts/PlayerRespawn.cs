/// <summary>
/// Thamnopoulos Thanos 2024
/// 
/// Basic Player Respawn.
/// </summary>
using System.Collections.Generic;
using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    // List of our respawn points
    [SerializeField] List<Transform> respawnPoints = new List<Transform>();
    // Respawn after -30 height
    public float HeightThreshold = -30f;
    private int currentRespawn = 0;

    void Update()
    {
        // After we pass one point we respawn to the next
        if(currentRespawn < respawnPoints.Count - 1)
        { 
            if ((currentRespawn + 1 <= respawnPoints.Count) &&(transform.position.z > respawnPoints[currentRespawn + 1 ].position.z))
            {
                currentRespawn++;
            }
        }

        if (transform.position.y < HeightThreshold)
        {
            RespawnPlayer();
        }
    }

    void RespawnPlayer()
    {
        transform.position = respawnPoints[currentRespawn].position;
    }
}