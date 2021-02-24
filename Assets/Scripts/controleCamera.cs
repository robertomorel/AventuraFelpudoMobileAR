using UnityEngine;
using System.Collections;

public class controleCamera : MonoBehaviour
{

    /*
     Estratégia para que a câmera sempre fique atrás do player
         */

    public GameObject player;
    private Vector3 offset;

    // Use this for initialization
    void Start()
    {
        offset = transform.position - player.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = player.transform.position + offset;
    }

}
