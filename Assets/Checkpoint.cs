using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider player)
    {
        if (player.CompareTag("Player"))
        {
        player.gameObject.GetComponent<PlayerController>().CheckPoint = transform.position;
        Debug.Log("checkpoint saved" + player.gameObject.GetComponent<PlayerController>().CheckPoint);
        }
    }
}
