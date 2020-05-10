using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Steps : MonoBehaviour
{
    [SerializeField]
    private AudioClip dirtClip;
    [SerializeField]
    private AudioClip grassClip;
    [SerializeField]
    private AudioClip stoneClip;
    [SerializeField]
    private AudioClip woodClip;

    //private AudioSource audioSource;
    //private TerrainDetector terrainDetector;

    // Update is called once per frame
    void Awake()
    {
        //audioSource = GetComponent<AudioSource>();
        //terrainDetector = new TerrainDetector();
    }

    private void Step()
    {
        //AudioClip clip = GetAudioClip();
        //audioSource.PlayOneShot(clip);
    }

    /*
    private AudioClip GetAudioClip()
    {
        //int terrainTextureIndex = terrainDetector.GetActiveTerrainTextureIdx(transform.position);
        switch(terrainTextureIndex)
        {
            case 0:
                return dirtClip;
            case 1:
                return stoneClip;
            case 2:
                return woodClip;
            case 3:
            default:
                return grassClip;
        }
    }
    */
}
