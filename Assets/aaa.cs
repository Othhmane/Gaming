

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aaa : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public Canvas menuCanvas;
    public Transform headTransform;
    public float distanceFromHead = 1.0f;
    private bool isMenuVisible = false;

    void Update()
    {
        // Vérifiez si le bouton approprié est enfoncé (par exemple, le bouton A sur une manette VR).
        if (Input.GetButtonDown("Fire2"))
        {
            // Inversez l'état du canevas (active ou désactive).
            isMenuVisible = !isMenuVisible;

            if (isMenuVisible)
            {
                // Positionnez le canevas en face de la tête.
                Vector3 menuPosition = headTransform.position + headTransform.forward * distanceFromHead;
                menuCanvas.transform.position = menuPosition;

                // Ajustez l'orientation du canevas pour qu'il fasse face à l'arrière de la tête.
                Vector3 lookDir = headTransform.position - menuCanvas.transform.position;
                menuCanvas.transform.rotation = Quaternion.LookRotation(-lookDir);
            }

            // Activez ou désactivez le canevas en fonction de l'état.
            menuCanvas.enabled = isMenuVisible;
        }
    }
}
