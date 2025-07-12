using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInterfaceTextTemplate : MonoBehaviour
{
    [SerializeField]
    private string _textTemplate;

    public string TextTemplate { get { return _textTemplate; } }
}
