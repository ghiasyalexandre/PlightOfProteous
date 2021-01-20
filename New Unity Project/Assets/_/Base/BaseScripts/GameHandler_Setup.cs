using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey;
using CodeMonkey.Utils;
using CodeMonkey.MonoBehaviours;
using GridPathfindingSystem;

public class GameHandler_Setup : MonoBehaviour {

    [SerializeField] private CameraFollow cameraFollow;
    [SerializeField] private Player player;

    private void Start() {
        cameraFollow.Setup(GetCameraPosition, () => 50f);
    }

    private Vector3 GetCameraPosition() {
        return player.GetPosition();
    }

}
