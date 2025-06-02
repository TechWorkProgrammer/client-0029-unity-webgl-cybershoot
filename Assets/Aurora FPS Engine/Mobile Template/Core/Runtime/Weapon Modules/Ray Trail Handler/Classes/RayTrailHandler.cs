using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.WeaponModules;
using AuroraFPSRuntime.SystemModules;
using UnityEngine;

namespace AuroraFPSRuntime.Addons.Mobile.WeaponModules
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/Addons/Mobile/Weapon Modules/Ray Trail/Ray Trail Handler")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(WeaponRayShootingSystem))]
    public sealed class RayTrailHandler : MonoBehaviour
    {
        [SerializeField]
        [NotNull]
        private RayTrail trail;

        // Stored required components.
        private PoolManager poolManager;
        private WeaponRayShootingSystem rayShootingSystem;

        private void Awake()
        {
            poolManager = PoolManager.GetRuntimeInstance();
            rayShootingSystem = GetComponent<WeaponRayShootingSystem>();
            rayShootingSystem.OnFireRayCallback += FireAction;
        }

        private void FireAction(RaycastHit hitInfo)
        {
            Vector3 firePoint = rayShootingSystem.GetShootPoint().position;
            Vector3 fireForward = rayShootingSystem.GetShootPoint().forward;
            RayTrail trailClone = poolManager.CreateOrPop<RayTrail>(trail, firePoint, Quaternion.LookRotation(fireForward));
            trailClone.Visualize(firePoint, hitInfo.point);
        }
    }
}