using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Entity : MonoBehaviour, ISelectableEntity
{
    public int ID { get { return _id; } }
    [SerializeField] private int _id;

    public void DeslectEntity()
    {
        
    }

    public void SelectEntity()
    {

    }
}
