using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debut : MonoBehaviour
{
        int i = 0;
    private Vector3 debut;
    private void OnTriggerEnter(Collider player)
    {
        if (player.CompareTag("Player"))
        {
    debut = new Vector3(8.70400047f, 3.72199988f, 45.3419991f);
        Vector3 offset = new Vector3(i * 1f, 0f, 0f);
        Vector3 debutoffset = offset + debut;
        i = i + 1;
            player.gameObject.GetComponent<PlayerController>().DebutCheck = debutoffset;
            player.gameObject.GetComponent<PlayerController>().TeleportToDebut();
        }
    }
}
