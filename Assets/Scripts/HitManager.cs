using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitManager : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("trigger with: "+other.tag);
        handleCollision(other.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("collision with: " + collision.gameObject.tag);
        handleCollision(collision.gameObject);
    }

    public void handleCollision(GameObject gameObject)
    {
        GameObject controller = GameObject.Find("GameController");
        string collidedTag = gameObject.tag;

        switch (collidedTag)
        {
            case Const.TAG_COIN: controller.BroadcastMessage("collectCoin", gameObject);
                break;
            case Const.TAG_OBSTACLE: controller.BroadcastMessage("collided", gameObject);
                break;
            case Const.TAG_CHECK_POINT: controller.BroadcastMessage("checkpoint");
                break;
            case Const.TAG_BOOSTER_COIN_DOUBLE:
            case Const.TAG_BOOSTER_SCORE:
            case Const.TAG_BOOSTER_INVINCIBLE:
                controller.BroadcastMessage("collectBooster", gameObject);
                break;
        }
    }
}
