using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapSelectionControl : MonoBehaviour
{
    public enum Maps
    {
        Lab,
        Camp,
        Village,
        Playground
    };

    public String selectedMap;

    private void Awake()
    {
        SelectMap(Random.Range(1, 3));
    }

    public void SelectMap(Maps map)
    {
        switch (map)
        {
            case Maps.Lab:
                selectedMap = "Map1";
                break;
            case Maps.Camp:
                selectedMap = "Map2";
                break;
        }
    }
    
    public void SelectMap(int mapNum)
    {
        switch (mapNum)
        {
            case 1:
                selectedMap = "Map1";
                break;
            case 2:
                selectedMap = "Map2";
                break;
            default: 
                selectedMap = "Map1";
                break;
        }
    }
}
