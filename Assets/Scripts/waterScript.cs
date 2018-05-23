using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class waterScript : MonoBehaviour {

    private Terrain parentTerrain;
    private GameObject water;

    public float splatHeigth;


	// Use this for initialization
	void Start ()
    {
        water = this.gameObject;
        // Parent du gameObject
        parentTerrain = Terrain.activeTerrain;
        var width = parentTerrain.terrainData.size.x;
        var lenght = parentTerrain.terrainData.size.z;
        var heigth = parentTerrain.terrainData.size.y;

        var finalheigth = heigth * (splatHeigth * 100) / 100;


        //DiamondSquare.splatHeights
        water.transform.localScale = new Vector3(width, 1, lenght);
        water.transform.position = new Vector3(water.transform.position.x + width/2, finalheigth, water.transform.position.z + lenght/2);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
