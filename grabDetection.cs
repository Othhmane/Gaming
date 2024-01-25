using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GrabDetection : MonoBehaviour
{
    private XRGrabInteractable grabInteractable;

    public AudioClip grabSound;
    public AudioClip releaseSound;

    private AudioSource audioSource;

    private void Start()
    {
        // Assurez-vous d'ajouter un AudioSource au même GameObject que ce script
        audioSource = GetComponent<AudioSource>();

        // Obtenez ou attachez XR Grab Interactable au GameObject
        grabInteractable = GetComponent<XRGrabInteractable>();

        // Abonnez-vous aux événements de saisie et de désélection
        grabInteractable.onSelectEnter.AddListener(OnGrab);
        grabInteractable.onSelectExit.AddListener(OnRelease);
    }

    private void OnGrab(XRBaseInteractor interactor)
    {
        // Jouez le son de saisie (grab) lorsque l'objet est saisi
        if (grabSound != null)
        {
            audioSource.PlayOneShot(grabSound);
        }
    }

    private void OnRelease(XRBaseInteractor interactor)
    {
        // Jouez le son de désélection (release) lorsque l'objet est désélectionné
        if (releaseSound != null)
        {
            audioSource.PlayOneShot(releaseSound);
        }
    }
}
