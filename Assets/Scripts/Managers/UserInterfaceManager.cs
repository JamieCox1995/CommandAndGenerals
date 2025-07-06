using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UserInterfaceManager : MonoBehaviour
{
    public static UserInterfaceManager Instance;

    [Header("Interface Prefabs")]
    public GameObject ConstructionDisplayPrefab;


    public Dictionary<Unit, GameObject> UnitInterfaces = new Dictionary<Unit, GameObject>();


    // Start is called before the first frame update
    void Start()
    {
        if(Instance == null)
        {
            Instance = this;
        } 
        else
        {
            Destroy(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateUIPositions();
    }

    public void UpdateUIPositions()
    {
        foreach(KeyValuePair<Unit, GameObject> kvp in UnitInterfaces)
        {
            // Get the World -> Camera Space Position of the current unit.
            Vector3 cameraSpace = Camera.main.WorldToScreenPoint(kvp.Key.transform.position);

            if(kvp.Value.TryGetComponent(out UserInterfaceOffset offset))
            {
                cameraSpace += offset.Offset;
            }

            kvp.Value.transform.position = cameraSpace;
        }
    }

    public static void DisplayUnitStatistics(Unit _Unit)
    {
        if (Instance.UnitInterfaces.ContainsKey(_Unit))
        {
            return;
        }
    }

    public static void DestroyUnitDispaly(Unit _Unit)
    {
        if(!Instance.UnitInterfaces.ContainsKey(_Unit))
        {
            return;
        }
    }

    public static void ShowConstructionDisplay(Unit _Unit)
    {
        // If we have already got the unit Registered, we should not add a new one as this would try an error
        // when inserting a dupe key to a Dictionary
        if (Instance.UnitInterfaces.ContainsKey(_Unit))
        {
            // Instead, we will call the update method.
            UpdateConstructionDisplay(_Unit);

            // And then return;
            return;
        }

        // Spawning the Prefab in the world.
        GameObject spawned = Instantiate(Instance.ConstructionDisplayPrefab, Instance.transform);
        Instance.UnitInterfaces.Add(_Unit, spawned);

        TMP_Text textbox = spawned.GetComponentInChildren<TMP_Text>();

        string template = textbox.text;

        textbox.text = string.Format(template, "0");
    }

    public static void UpdateConstructionDisplay(Unit _Unit) 
    {
        if (!Instance.UnitInterfaces.ContainsKey(_Unit))
        {
            return;
        }

        // Get the UI GameObject from the Dictionary
        GameObject ui = Instance.UnitInterfaces[_Unit];

        TMP_Text textbox = ui.GetComponentInChildren<TMP_Text>();

        string template = textbox.text;
        float constructionAmount = ((float)_Unit.RemainingHitPoints / _Unit.StartingHitPoints) * 100f;

        //textbox.text = string.Format(template, constructionAmount);
        textbox.text = string.Format("Building {0}%...", constructionAmount);
    }

    public static void DestroyConstructionDisplay(Unit _Unit)
    {
        if (!Instance.UnitInterfaces.ContainsKey(_Unit))
        {
            return;
        }

        Destroy(Instance.UnitInterfaces[_Unit]);
        Instance.UnitInterfaces.Remove(_Unit);
    }
}
