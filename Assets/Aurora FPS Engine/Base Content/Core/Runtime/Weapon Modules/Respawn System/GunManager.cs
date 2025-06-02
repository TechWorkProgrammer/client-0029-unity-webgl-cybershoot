using UnityEngine;
using System.Collections.Generic;
using AuroraFPSRuntime.CoreModules.Pattern;
public class GunManager : Singleton<GunManager> {
    public float respawnTime = 5;
    
    // // Trigger all gun respawners
    // public void TriggerAllGunRespawners(){
    //     foreach (IRespawner respawner in gunRespawners) {
    //         respawner.Respawn();
    //     }
    // }
}