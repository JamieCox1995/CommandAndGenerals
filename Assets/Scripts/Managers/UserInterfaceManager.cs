using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UserInterfaceManager : MonoBehaviour
{
    public static UserInterfaceManager Instance;

    [Header("Interface Prefabs")]
    public GameObject ConstructionDisplayPrefab;
    public GameObject HealthDisplayPrefab;
    public GameObject ProgressDisplay;
    public Dictionary<Unit, GameObject> UnitInterfaces = new Dictionary<Unit, GameObject>();

    private Camera _mainCamera;

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

        _mainCamera = Camera.main;
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
            Vector3 cameraSpace = _mainCamera.WorldToScreenPoint(kvp.Key.transform.position);

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
            UpdateUnitStatistics(_Unit);
            return;
        }

        // Spawning the Prefab in the world.
        GameObject spawned = Instantiate(Instance.HealthDisplayPrefab, Instance.transform);
        Instance.UnitInterfaces.Add(_Unit, spawned);

        float amount = (float)_Unit.RemainingHitPoints / _Unit.StartingHitPoints;

        Slider healthSlider = spawned.GetComponentInChildren<Slider>();
        healthSlider.value = amount;

        UnitHealthDisplay uhd = spawned.GetComponent<UnitHealthDisplay>();
        Color c = uhd.HealthGradient.Evaluate(amount);
        healthSlider.fillRect.GetComponent<Image>().color = c;
    }

    public static void UpdateUnitStatistics(Unit _Unit)
    {
        if (!Instance.UnitInterfaces.ContainsKey(_Unit))
        {
            return;
        }
        GameObject ui = Instance.UnitInterfaces[_Unit];
        float amount = (float)_Unit.RemainingHitPoints / _Unit.StartingHitPoints;

        Slider healthSlider = ui.GetComponentInChildren<Slider>();
        healthSlider.value = amount;

        UnitHealthDisplay uhd = ui.GetComponent<UnitHealthDisplay>();
        Color c = uhd.HealthGradient.Evaluate(amount);
        healthSlider.fillRect.GetComponent<Image>().color = c;
    }

    public static void DestroyUnitDisplay(Unit _Unit)
    {
        if(!Instance.UnitInterfaces.ContainsKey(_Unit))
        {
            return;
        }

        Destroy(Instance.UnitInterfaces[_Unit]);
        Instance.UnitInterfaces.Remove(_Unit);
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

        if(spawned.TryGetComponent(out UserInterfaceTextTemplate textTemplate))
        {
            template = textTemplate.TextTemplate;
        }

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

        if (ui.TryGetComponent(out UserInterfaceTextTemplate textTemplate))
        {
            template = textTemplate.TextTemplate;
        }

        string textToDisplay = constructionAmount.ToString("N0");

        textbox.text = string.Format(template, textToDisplay);
        //textbox.text = string.Format("Building {0}%...", constructionAmount);
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

    public static void ShowProgressDisplay(Unit _Unit, float _Value)
    {
        // If we have already got the unit Registered, we should not add a new one as this would try an error
        // when inserting a dupe key to a Dictionary
        if (Instance.UnitInterfaces.ContainsKey(_Unit))
        {
            // Instead, we will call the update method.
            UpdateProgressDisplay(_Unit, _Value);

            // And then return;
            return;
        }

        // Spawning the Prefab in the world.
        GameObject spawned = Instantiate(Instance.ProgressDisplay, Instance.transform);
        Instance.UnitInterfaces.Add(_Unit, spawned);

        Slider progressSlider = spawned.GetComponentInChildren<Slider>();
        progressSlider.value = _Value;
    }

    public static void UpdateProgressDisplay(Unit _Unit, float _Value)
    {
        if (!Instance.UnitInterfaces.ContainsKey(_Unit))
        {
            return;
        }

        // Get the UI GameObject from the Dictionary
        GameObject ui = Instance.UnitInterfaces[_Unit];

        Slider progressSlider = ui.GetComponentInChildren<Slider>();
        progressSlider.value = _Value;
    }

    public static void DestroyProgressDisplay(Unit _Unit)
    {
        if (!Instance.UnitInterfaces.ContainsKey(_Unit))
        {
            return;
        }

        Destroy(Instance.UnitInterfaces[_Unit]);
        Instance.UnitInterfaces.Remove(_Unit);
    }
}
