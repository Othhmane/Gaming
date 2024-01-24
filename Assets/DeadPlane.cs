using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadPlane : MonoBehaviour
{
    private void OnTriggerEnter(Collider player)
    {
        player.gameObject.GetComponent<PlayerMovementTutorial>().TeleportToCheckpoint();
        Debug.Log("you dead");
    }

}
