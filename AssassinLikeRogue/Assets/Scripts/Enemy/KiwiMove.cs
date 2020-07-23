using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KiwiMove : MonoBehaviour
{
    public float speed = 0.4f;
    public Transform moveSpotPos;
    GameObjectFinder objFinder;

    private void Start()
    {
        objFinder = GetComponent<GameObjectFinder>();
        //moveSpotPos = objFinder.actors[0].transform;
    }

    // Update is called once per frame
    void Update()
    {
        //if (Vector2.Distance(transform.position, moveSpotPos.position) > 0.2f)
        //    transform.position = Vector2.MoveTowards(transform.position, moveSpotPos.position, speed * Time.deltaTime);
        //moveSpotPos = objFinder.actors[0].transform;
    }
}
