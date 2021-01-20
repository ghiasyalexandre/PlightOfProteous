using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAfterImage : MonoBehaviour
{
    [SerializeField]
    private float activeTime = 0.1f;
    private float timeActivated;
    private float alpha;
    [SerializeField]
    private float alphaSet = 0.8f;
    private float alphaMultiplier = 0.05f;

    private Transform player;
    private SpriteRenderer[] rend;
    private SpriteRenderer[] playerRend;

    private Color color;

    private void OnEnable()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerRend = player.GetComponent<SpriteMaker>().frameRenderers;
        rend = new SpriteRenderer[playerRend.Length];
        alpha = alphaSet;
        for (int i = 0; i < playerRend.Length; i++)
        {
            rend[i].sprite = playerRend[i].sprite;
        }
    }
}
