using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EntityOrder
{
    public string OrderName;
    public Dictionary<string, object> Parameters;

    public int OrderPriority;
}
