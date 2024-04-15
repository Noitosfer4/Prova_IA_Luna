using UnityEngine;

public class camera : MonoBehaviour
{
    public Transform target; 
    public Vector3 offset; 

    void LateUpdate(){
        if (target != null){            
            Vector3 desiredPosition = target.position + offset;         
            desiredPosition.z = transform.position.z;
            transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * 5f);
        }
    }
}