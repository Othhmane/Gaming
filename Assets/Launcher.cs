using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Launcher : MonoBehaviour
{
    public float speed;
    public GameObject target;
    Vector3 lastPosition;
    Quaternion lookAtRotation;
    GameObject TurretHead;

    // Start is called before the first frame update
    void Start()
    {
        // Assuming this script is attached to the parent GameObject
        Transform parentTransform = transform;
        TurretHead = parentTransform.GetChild(2).gameObject;
        gameObject.GetComponent<ShootingSystem>().enabled = false;

    }

    // Update is called once per frame
    void Update()
    {
        if(target != null)
        {
        if (lastPosition != target.transform.position)
        {
            lastPosition = target.transform.position;
            lookAtRotation = Quaternion.LookRotation(lastPosition - transform.position);
        }
        if(TurretHead.transform.rotation != lookAtRotation)
            {

                TurretHead.transform.rotation = Quaternion.RotateTowards(TurretHead.transform.rotation, lookAtRotation, speed*Time.deltaTime);
            }

    }
 }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameObject.GetComponent<ShootingSystem>().enabled = false;
            target = null;
        }
        if (other.CompareTag("Projectile"))
        {
            Destroy(other.gameObject);
        }
    }
    

    private void OnTriggerStay(Collider collision)
    {
        if (collision.CompareTag("Player")) {
            gameObject.GetComponent<ShootingSystem>().enabled = true;
            //target = collision.gameObject;
            Transform parentTransformTarget = transform;
            parentTransformTarget = collision.gameObject.transform;
            target = parentTransformTarget.GetChild(0).gameObject;
        }
    }
}
