using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    public const float followSpeed = 1.0f;
    private void FixedUpdate()
    {
        if (target == null)
            return;
        // Calculate the desired camera position
        Vector3 desiredPosition =  new Vector3(0, 0, target.position.z) + offset;

        // Smoothly move the camera towards the desired position
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime *followSpeed);
    }

    public void setTarget(Transform target)
    {
        this.target = target;
    }

    public void changeOffset(Vector3 value)
    {
        //offset += value;
    }

}