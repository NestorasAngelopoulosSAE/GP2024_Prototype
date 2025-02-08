using System.Collections.Generic;
using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [SerializeField] List<Transform> respawnPoints = new List<Transform>();
    public float HeightThreshold = -30f;
    private int currentRespawn = 0;

    void Update()
    {
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