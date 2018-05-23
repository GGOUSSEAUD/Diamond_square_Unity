using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fly : MonoBehaviour {

    private bool avance = false;
    private bool recule = false;

    public GameObject objFlying;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        if( Input.GetKeyDown("z"))
            avance = true;
        if (Input.GetKeyUp("z"))
            avance = false;
        if (Input.GetKeyDown("s"))
            recule = true;
        if (Input.GetKeyUp("s"))
            recule = false;

        if (avance)
            transform.position += transform.forward * Time.deltaTime * 40.0f;
        if (recule)
            transform.position += -(transform.forward * Time.deltaTime * 40.0f);

        transform.Rotate(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"), 0.0f);

        float terrainHeight = Terrain.activeTerrain.SampleHeight(transform.position);

        //colision avec le terrain
        if (terrainHeight > transform.position.y - (2 * objFlying.GetComponent<Collider>().bounds.size.y))
            transform.position = new Vector3(transform.position.x,
                                                terrainHeight + 2 * objFlying.GetComponent<Collider>().bounds.size.y,
                                                transform.position.z);
    }
}
