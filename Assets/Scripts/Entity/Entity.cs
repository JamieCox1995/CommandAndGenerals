using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Entity : MonoBehaviour, ISelectableEntity
{
    public int ID { get { return _id; } }
    [SerializeField] private int _id;

    [SerializeField] private GameObject _selectedVisual;

    protected virtual void Start()
    {
        //_selectedVisual = gameObject.FindComponentInChildWithTag<Transform>("Entity Selection").gameObject;
        _selectedVisual.gameObject.SetActive(false);
    }

    public virtual void InitializeEntity(int _ID)
    {
        _id = _ID;
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
