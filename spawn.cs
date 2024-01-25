using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;
using UnityEngine.XR.Interaction.Toolkit;


public class spawn : MonoBehaviour
{
    public GameObject cubePrefab;
    private string jsonFileName = "prefabPositions.json";
    private List<PrefabTransformData> prefabDataList = new List<PrefabTransformData>();
    private Dictionary<int, GameObject> prefabDictionary = new Dictionary<int, GameObject>();

    public void OnTriggerEnter(Collider other)
    {
        Destroy(other.gameObject);
    } 

    public void OnButtonClick()
    {
        Vector3 spawnPosition = Camera.main.transform.position + Camera.main.transform.forward * 2.0f;

        // Check if a prefab with the same ID already exists
        int existingIndex = prefabDataList.FindIndex(data => data.id == GetNextId());

        if (existingIndex != -1)
        {
            // Update the position for the existing prefab with the same ID
            prefabDataList[existingIndex].position = spawnPosition;
        }
        else
        {
            // Instantiate a new prefab
            GameObject newCube = Instantiate(cubePrefab, spawnPosition, Quaternion.identity);
            int newId = GetNextId();
            prefabDictionary.Add(newId, newCube);

            // Add a new entry to the prefabDataList
            prefabDataList.Add(new PrefabTransformData { id = newId, position = spawnPosition, prefabName = cubePrefab.name });
        }

        // Save the changes
      //  SavePrefabTransforms();
    }

public void SaveAllPrefabs()
{
    // Efface la liste existante des préfabs
    prefabDataList.Clear();

    // Parcourt toutes les préfabs de la scène
    GameObject[] allPrefabs = GameObject.FindGameObjectsWithTag("a");
    foreach (GameObject prefab in allPrefabs)
    {
        // Ajoute les informations de la préfab à la liste seulement si le prefabName n'est pas "Sphere"
        if (!prefab.name.Contains("Sphere"))
        {
            // Ajoute les informations de la préfab à la liste
            int prefabId = GetNextId();
            Vector3 prefabPosition = prefab.transform.position;

            // Nettoie le nom de la préfab en supprimant "(Clone)"
            string prefabName = prefab.name.Replace("(Clone)", "");

            prefabDataList.Add(new PrefabTransformData { id = prefabId, position = prefabPosition, prefabName = prefabName });
        }
    }

    // Enregistre les préfabs dans le fichier JSON
    SavePrefabTransforms();

    // Affiche les informations des préfabs
    DisplayPrefabInfo();
}


    // Appeler cette méthode pour quitter le jeu
    public void QuitGame()
    {
         #if UNITY_EDITOR
             UnityEditor.EditorApplication.isPlaying = false;
         #else
             Application.Quit();
         #endif
    }

public TextAsset jsonFile; // Assurez-vous de faire glisser votre fichier JSON dans cet attribut dans l'éditeur Unity.
    public Transform parentTransform; // L'objet parent pour les préfabriqués instanciés.

void LoadPrefabTransforms()
    {
        if (jsonFile != null)
        {
            string jsonText = jsonFile.text;
            PrefabDataListWrapper prefabDataList = JsonUtility.FromJson<PrefabDataListWrapper>(jsonText);

            if (prefabDataList != null)
            {
                foreach (PrefabTransformData prefabData in prefabDataList.prefabDataList)
                {
                    // Instancier le préfabriqué en fonction des données du JSON.
                    GameObject prefab = Resources.Load<GameObject>(prefabData.prefabName);
                    if (prefab != null)
                    {
                        GameObject instantiatedPrefab = Instantiate(prefab, prefabData.position, Quaternion.identity, parentTransform);
                    }
                    else
                    {
                        Debug.LogError("Prefab not found: " + prefabData.prefabName);
                    }
                }
            }
            else
            {
                Debug.LogError("Failed to deserialize JSON.");
            }
        }
        else
        {
            Debug.LogError("JSON file not assigned.");
        }
    }


    void Start()
    {
        LoadPrefabTransforms();
        DisplayPrefabInfo();    
    }

    void Update()
    {
        foreach (var entry in prefabDictionary)
    {
        int id = entry.Key;
        GameObject prefab = entry.Value;

        // Move the prefab in the game based on user input or other logic.
        // For example, you can use Input.GetAxis or other input methods.

        // Update the position in the data list
        int index = prefabDataList.FindIndex(data => data.id == id);
        if (index != -1)
        {
            prefabDataList[index].position = prefab.transform.position;
        }
    }

 //   SavePrefabTransforms();

    }
    

    public void save(){
        SavePrefabTransforms();
            }

    void SavePrefabTransforms()
{
    string jsonPath = "Assets/Resources/" + jsonFileName; // Spécifiez le chemin relatif au dossier "Assets"

    // Sérialiser les nouvelles données en JSON
    string updatedJson = JsonUtility.ToJson(new PrefabDataListWrapper { prefabDataList = prefabDataList });

    // Écrire les données JSON mises à jour dans le fichier
    File.WriteAllText(jsonPath, updatedJson);

    Debug.Log("Transformations de prefab enregistrées dans : " + jsonPath);
}



    int GetNextId()
    {
        return prefabDataList.Count > 0 ? prefabDataList[prefabDataList.Count - 1].id + 1 : 1;
    }

    [System.Serializable]
    public class PrefabTransformData
    {
        public int id;
        public Vector3 position;
        public string prefabName;
    }

    [System.Serializable]
    public class PrefabDataListWrapper
    {
        public List<PrefabTransformData> prefabDataList;
    }


     void DisplayPrefabInfo()
{
    foreach (var prefabData in prefabDataList)
    {
        Debug.Log("ID: " + prefabData.id +
                  ", Position: " + prefabData.position +
                  ", Prefab Name: " + prefabData.prefabName);
    }
}
}
