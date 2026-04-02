using UnityEngine;
public class Cam_move : MonoBehaviour
{
    public GameObject player;
    void Update()
    {
        transform.position = new Vector3(
            player.transform.position.x + 11,
            0,
            -20
        );
    }
}