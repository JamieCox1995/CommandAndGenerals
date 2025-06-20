using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    public static EntityManager Instance;

    public List<Entity> SpawnableEntities = new List<Entity>();
    private List<Entity> _spawnedEntities = new List<Entity>();

    public const int MAX_ENTITY_COUNT = 2048;

    // Start is called before the first frame update
    void Start()
    {
        // On Start, we want to initialize the list of Entities that have been spawned.
        _spawnedEntities = new List<Entity>(MAX_ENTITY_COUNT);
    }

    private void Update()
    {
        // TODO: This will be removed once we have buildables implemented.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(cameraRay, out hit))
            {
                SpawnEntity(SpawnableEntities[0], hit.point, out int index);
            }
        }
    }

    public bool SpawnEntity(Entity _RequestedEntity, out int _SpawnedIndex)
    {
       return SpawnEntity(_RequestedEntity, Vector3.zero, out _SpawnedIndex);
    }

    public bool SpawnEntity(Entity _RequestedEntity, Vector3 _SpawnLocation, out int _SpawnedIndex)
    {
        // Making sure that the Entity we are trying to spawn is registered.
        if(!SpawnableEntities.Contains(_RequestedEntity))
        {
            _SpawnedIndex = -1;
            return false;
        }

        // Making sure that there is space in the list to spawn the entity.
        if(_spawnedEntities.Count >= MAX_ENTITY_COUNT)
        {
            _SpawnedIndex = -1;
            return false;
        }

        _SpawnedIndex = _spawnedEntities.Count;

        Entity spawnedEntity = Instantiate(_RequestedEntity, _SpawnLocation, Quaternion.identity);
        _spawnedEntities.Add(spawnedEntity);

        spawnedEntity.InitializeEntity(_SpawnedIndex);
        return true;
    }
}
