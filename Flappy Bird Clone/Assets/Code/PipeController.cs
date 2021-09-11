using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeController : MonoBehaviour
{
    private void Start()
    {
        gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.left * 2;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == GameData.PIPE_DESTROYER_TAG)
            Destroy(gameObject);
    }
}
