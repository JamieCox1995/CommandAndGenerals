using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductionStructure : StructureUnit
{
    [Tooltip("This is the list of units/upgrades which can be produced from this Structure.")]
    public List<ProductionProfile> AvailableProductions = new List<ProductionProfile>();

    // Creating a queue which will store all of the units which we want to produce
    private Queue<ProductionProfile> _productionQueue = new Queue<ProductionProfile>();
}
