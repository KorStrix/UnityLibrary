using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpanwerExample_Controller : MonoBehaviour
{
    public List<CSpawnerBase> listSpawnPoint;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            for (int i = 0; i < listSpawnPoint.Count; i++)
                listSpawnPoint[i].DoSpawnObject();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            for (int i = 0; i < listSpawnPoint.Count; i++)
                listSpawnPoint[i].DoDisable_AllSpawnObject();
        }
    }
}
