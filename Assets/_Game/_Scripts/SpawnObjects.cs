using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;
//using UnityEngine.XR.Interaction.Toolkit;


public class SpawnObjects : MonoBehaviour
{

    private List<PrefabTransformData> prefabDataList = new List<PrefabTransformData>();
    private Dictionary<int, GameObject> prefabDictionary = new Dictionary<int, GameObject>();

    public void OnTriggerEnter(Collider other)
    {
        Destroy(other.gameObject);
    }

    TextAsset jsonFile; // Assurez-vous de faire glisser votre fichier JSON dans cet attribut dans l'éditeur Unity.
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
#if UNITY_EDITOR
                    string prefabPath = "Assets/StarterAssets/ThirdPersonController/Prefabs/" + prefabData.prefabName + ".prefab";
                    GameObject prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
#endif


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
    private void Awake()
    {
        GameObject mapJsonObject = GameObject.Find("Map Json");
        jsonFile = mapJsonObject.GetComponent<JsonMap>().JsonFile;
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
