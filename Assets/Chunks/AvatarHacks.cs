using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarHacks : MonoBehaviour {

    public MVCController controller;
    

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        controller.ColliderHitHack(hit);
    }

}
