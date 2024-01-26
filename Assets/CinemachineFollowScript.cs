using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinemachineFollowScript : MonoBehaviour
{
        GameObject target; // The GameObject you want the camera to follow

        private void Start()
        {
        Transform parentTransform = transform;
        target = parentTransform.GetChild(0).gameObject;

        var virtualCam = FindObjectOfType<CinemachineVirtualCamera>();
        if (virtualCam != null && target != null)
            {
                virtualCam.Follow = target.transform;
            }
}

// Update is called once per frame
void Update()
    {
        
    }
}
