using StarterAssets;
using UnityEngine;

public class Checkfinalcheckpoint : MonoBehaviour
{
    public GameObject ScoreboardCanvas;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            ScoreboardCanvas.SetActive(true);
        }
    }

}
