using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragSlot : MonoBehaviour {

    static public DragSlot instance;

    public Slot dragSlot;

    void Start()
    {
        instance = this;
    }
}
