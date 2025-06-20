using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Entity : MonoBehaviour, ISelectableEntity
{
    public int ID { get { return _id; } }
    private int _id;

    public int TeamIndex { get { return _teamIndex; } }
    private int _teamIndex;

    [SerializeField] private GameObject _selectedVisual;

    protected virtual void Start()
    {
        //_selectedVisual = gameObject.FindComponentInChildWithTag<Transform>("Entity Selection").gameObject;
        _selectedVisual.gameObject.SetActive(false);
    }

    public virtual void InitializeEntity(int _ID, int _TeamIndex = 0)
    {
        _id = _ID; 
        _teamIndex = _TeamIndex;
    }

    public void DeslectEntity()
    {
        _selectedVisual.gameObject.SetActive(false);
    }

    public void SelectEntity()
    {
        _selectedVisual.gameObject.SetActive(true);
    }
}
