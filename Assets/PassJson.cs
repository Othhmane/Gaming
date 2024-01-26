using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JsonMap : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public TextAsset JsonFile;
}
