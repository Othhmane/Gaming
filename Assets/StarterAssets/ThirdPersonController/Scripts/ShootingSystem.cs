using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShootingSystem : MonoBehaviour
{
    public float firerate;
    public Vector3 force;
    public bool beam;
    public List<GameObject> projectilespawns;
    public GameObject projectile;
    public GameObject target;
    GameObject TurretHead;

    public float fieldofview;
    float firetimer = 0f;
    // Start is called before the first frame update
    // Start is called before the first frame update
    void Start()
    {
        // Assuming this script is attached to the parent GameObject
        Transform parentTransform = transform;
        TurretHead = parentTransform.GetChild(2).gameObject;
    }



    List<GameObject> lastprojectile = new List<GameObject>();
    void SpawnProjectiles()
    {
        if (!projectile)
        {
            return;
        }
        for (int i = 0; projectilespawns.Count > i; i++)
        {
            if (projectilespawns[i])
            {
                GameObject proj = Instantiate(projectile, projectilespawns[i].transform.position, Quaternion.Euler(projectilespawns[i].transform.forward));
                proj.GetComponent<BaseLauncher>().FireProjectile(projectilespawns[i], target, force);

                lastprojectile.Add(proj);
            }
        }
    }
        // Update is called once per frame
        void Update()
        {
        target = gameObject.GetComponent<Launcher>().target;
        Debug.Log(gameObject.GetComponent<Launcher>().target);
        if (target != null)
        {

        if (beam && lastprojectile.Count <= 0)
            {
                float angle = Quaternion.Angle(TurretHead.transform.rotation, Quaternion.LookRotation(target.transform.position - transform.position));
                if (angle > fieldofview)
                {

                    SpawnProjectiles();
                }
            }
            else if (beam && lastprojectile.Count > 0)
            {
                float angle = Quaternion.Angle(TurretHead.transform.rotation, Quaternion.LookRotation(target.transform.position - transform.position));
                if (angle > fieldofview)
                {
                    while (lastprojectile.Count > 0)
                    {
                        Destroy(lastprojectile[0]);
                        lastprojectile.RemoveAt(0);
                    }
                }
            }
            else {
                firetimer += Time.deltaTime;
                if(firetimer >= firerate)
                {
                    float angle = Quaternion.Angle(TurretHead.transform.rotation, Quaternion.LookRotation(target.transform.position - transform.position));
                    if (angle < fieldofview)
                    {
                        SpawnProjectiles();
                        firetimer = 0f;
                    }
                }
            }
            }
        }
        }
