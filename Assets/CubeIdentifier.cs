using UnityEngine;

public class CubeIdentifier : MonoBehaviour
{
    private int cubeId;

    public void SetCubeId(int id)
    {
        cubeId = id;
    }

    public int GetCubeId()
    {
        return cubeId;
    }
}
