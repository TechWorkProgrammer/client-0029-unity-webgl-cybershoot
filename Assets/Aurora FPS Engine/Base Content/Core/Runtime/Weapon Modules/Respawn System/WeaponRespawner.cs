using UnityEngine;
using System.Collections.Generic;
public class WeaponRespawner : Respawner
{
    #region PREFABS
    [SerializeField] private GameObject weaponPrefab;
    [SerializeField] private GameObject magazinePrefab;
    #endregion

    #region RESPAWNERS
    [SerializeField] private List<ObjectRespawner2> weaponRespawners;
    [SerializeField] private List<ObjectRespawner2> magazineRespawners;
    #endregion
    public void Start() {
        foreach (ObjectRespawner2 weaponRespawner in weaponRespawners) {
            weaponRespawner.SetObjectPrefab(weaponPrefab);
        }
        foreach (ObjectRespawner2 magazineRespawner in magazineRespawners) {
            magazineRespawner.SetObjectPrefab(magazinePrefab);
        }
    }


    public override void Respawn(){
        Respawn(weaponRespawners);
        Respawn(magazineRespawners);
    }
}

