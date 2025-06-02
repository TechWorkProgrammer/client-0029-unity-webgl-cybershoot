using UnityEngine;
using System.Collections.Generic;

public abstract class Respawner : MonoBehaviour, IRespawner
{
    // Trigger all Respawn (List<ObjectRespawner2> objectRespawners)
    public abstract void Respawn();

    // Respawn all objects in List<ObjectRespawner2> objectRespawners
    public void Respawn(List<ObjectRespawner2> objectRespawners){
        foreach (ObjectRespawner2 objectRespawner in objectRespawners) {
            if (objectRespawner.objectInstance == null) {
                objectRespawner.SpawnObject();
            }
        }
    }
}