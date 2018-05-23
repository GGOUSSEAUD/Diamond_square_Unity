using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DiamondSquare : MonoBehaviour {
    Terrain my_terrain;
    public int Details;//Must be 0 or 1, TODO: Changer ça en valeur restreinte
    public float dimension_fractale;
    public float minstart, maxstart;
    private int heighmap_width;
    private int heighmap_height;
    private float[,] toNorm;

    // Terrain painting ------------------
    [System.Serializable]
    public class SplatHeights
    {
        public int textureIndex;
        public float startingHeight;
    }

    public SplatHeights[] splatHeights;
    // ------------------------------------

    int find_pow(int num)
    {
        int res = 0;
        while(num > 1)
        {
            num /= 2;
            res++;
        }
        return res;
    }



    void my_normalize(int widthsize, int heighsize)
    {
        float max = toNorm[0, 0], min = toNorm[1, 0], tmp;
        
        if (max < min)
        {
            tmp = max;
            max = min;
            min = tmp;
        }

        //Trouve le minimum et le maximum d'un tableau
        for (int x = 0; x < widthsize; x++)
        {
            for (int y = 0; y < heighsize; y++)
            {
                if (toNorm[y,x] > max)
                {
                    max = toNorm[y, x];
                }
                else if(toNorm[y,x] < min)
                {
                    min = toNorm[y, x];
                }
            }
        }

        for (int x = 0; x < widthsize; x++)
        {
            for(int y = 0; y < heighsize; y++)
            {
                toNorm[y, x] = ((toNorm[y,x] - min)/(max - min));
            }
        }

    }

    void paintTerrain()
    {
        float amplitude = 1.0f;
        TerrainData terrainData = my_terrain.terrainData;
        float[,,] splatmapData = new float[terrainData.alphamapWidth,
                                            terrainData.alphamapHeight,
                                            terrainData.alphamapLayers];
        float[,] heights = terrainData.GetHeights(0, 0, terrainData.heightmapWidth, terrainData.heightmapHeight);

        for (int y = 0; y < terrainData.alphamapHeight; y++)
        {
            for (int x = 0; x < terrainData.alphamapWidth; x++)
            {
                //print("Height test: " + terrainData.GetHeight(y, x) + " | " + heights[y, x]);
                float terrainHeight = heights[y, x];

                float[] splat = new float[splatHeights.Length];
                for (int i = 0; i < splat.Length; i++)
                {
                    splat[i] = 0;
                }
                
                for (int i = 0; i < splatHeights.Length; i++)
                {
                    //Ici il faudra calculer l'amplitude ...

                    if (i == splatHeights.Length - 1 && terrainHeight >= splatHeights[i].startingHeight)
                        splat[i] = 1;
                    //superposition des textures
                    /*
                    else if (terrainHeight <= (splatHeights[i+1].startingHeight + (0.05 * amplitude)) &&
                        terrainHeight >= (splatHeights[i+1].startingHeight - (0.05 * amplitude)) && i+1 < splatHeights.Length-1)
                    {
                        splat[i] = 0.5f;
                        splat[i + 1] = 0.5f;
                    }
                    */
                    else if (terrainHeight >= splatHeights[i].startingHeight &&
                        terrainHeight <= splatHeights[i + 1].startingHeight)
                        splat[i] = 1;
                }

                for (int j = 0; j < splatHeights.Length; j++)
                {
                    splatmapData[y, x, j] = splat[j];
                }
            }
        }

        //print("Height test: "+ terrainData.GetHeight(0,0) + " | "+ heights[0,0]);
        terrainData.SetAlphamaps(0, 0, splatmapData);
    }

    //Compute heights
    void Compheights(int widthsize, int heightsize)
    {

        toNorm = my_terrain.terrainData.GetHeights(0, 0, heighmap_width, heighmap_height);
        //Init Corners
        toNorm[0, 0]
            = Random.Range(minstart, maxstart);

        toNorm[0, widthsize-1]
            = Random.Range(minstart, maxstart);

        toNorm[heightsize-1, 0]
            = Random.Range(minstart, maxstart);

        toNorm[heightsize-1, widthsize-1]
            = Random.Range(minstart, maxstart);

        //Diamond
        int step = widthsize - 1;
        float moyenne, sum;
        int profondeur = 0;
        int jump = 0;
        int fois;
        int semi_step;
        while (step > 1)
        {
            semi_step = step / 2;
            //Diamond phase (Square selon Belhadj)
            for(int x = semi_step; x  < widthsize; x+= step)
            {
                for (int y = semi_step; y < heightsize; y += step)
                {
                    moyenne 
                        =(toNorm[y - semi_step, x - semi_step] 
                        + toNorm[y + semi_step, x - semi_step] 
                        + toNorm[y + semi_step, x + semi_step] 
                        + toNorm[y - semi_step, x + semi_step]) / 4;
                    
                    toNorm[y, x] = moyenne + ((Random.Range(0.0f, 1.0f) * 2.0f - 1.0f) * Mathf.Sqrt(2) * (widthsize / Mathf.Pow(2, dimension_fractale * profondeur)));
                         
                    //print("First phase :" + toNorm[y, x]);
                }
            }

            //Square phase (Diamond selon Belhadj)
            for(int x = 0; x < widthsize; x += semi_step)
            {
                if( x % step == 0)
                {
                    jump = semi_step;
                }
                else
                {
                    jump = 0;
                }
                for(int y = jump; y < widthsize; y+= step)
                {
                    sum = 0;
                    fois = 0;
                    if (x >= semi_step)
                    {
                        sum += toNorm[y, x - semi_step];
                        fois++;
                    }
                    if (x + semi_step < widthsize)
                    {
                        sum += toNorm[y, x + semi_step];
                        fois++;
                    }
                    if (y >= semi_step)
                    {
                        sum += toNorm[y - semi_step, x];
                        fois++;
                    }
                    if (y + semi_step < widthsize)
                    {
                        sum += toNorm[y + semi_step, x];
                        fois++;
                    }
                    toNorm[y,x] = (sum / fois) + ((Random.Range(0.0f, 1.0f) * 2.0f - 1.0f) * (widthsize / Mathf.Pow(2, dimension_fractale * profondeur)));

                    //print("Second phase :" + heights[y, x]);
                }
            }
            profondeur++;
            step = semi_step;
        }
        my_normalize(widthsize, heightsize);
        my_terrain.terrainData.SetHeights(0, 0, toNorm);
    }
    

    // Use this for initialization
    void Start()
    {
        my_terrain = Terrain.activeTerrain;
        heighmap_width = my_terrain.terrainData.heightmapWidth;
        heighmap_height = my_terrain.terrainData.heightmapHeight;
        my_terrain.heightmapMaximumLOD = Details; // Minimum details: 1 , max details: 0
        Compheights(heighmap_width, heighmap_height);
        paintTerrain();
    }

    // Update is called once per frame
    void Update()
    {
             
    }
}