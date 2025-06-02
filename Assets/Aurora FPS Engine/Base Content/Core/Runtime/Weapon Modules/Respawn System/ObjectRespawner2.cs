using UnityEngine;

public class ObjectRespawner2: MonoBehaviour
{
    private GameObject objectPrefab;
   
    public GameObject objectInstance;
    // public GunManager gunManager;
    private float respawnTime;

    private void Awake() {
        // gunManager = GunManager.GetRuntimeInstance();
    }
    private void Start() {
        respawnTime = GameManager.Instance.gunRespawnTimeValue;
    }
    public void SetObjectPrefab(GameObject objectPrefab){
        this.objectPrefab = objectPrefab;
    }

    private void Update() {
        if (objectInstance == null) {
            respawnTime -= Time.deltaTime;
            if (respawnTime <= 0) {
                SpawnObject();
                respawnTime = GameManager.Instance.gunRespawnTimeValue;
            }
        }
    }

    public void SpawnObject() {
        objectInstance = Instantiate(objectPrefab, transform.position, transform.rotation);
        objectInstance.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
    }   
}
