using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdController : MonoBehaviour
{
    [SerializeField]
    private Sprite deadSprite, aliveSprite;

    [SerializeField]
    private float flyForce;

    [SerializeField]
    private Rigidbody2D birdRigidbody;

    [SerializeField]
    private GameObject birdCage;

    private bool alive = true;

    private Vector2 startPosition;

    protected bool Alive {
        get => alive;
        set { 
            if(value == false)
            {
                birdRigidbody.velocity = Vector2.zero;
                GameObject.Find(GameData.GAMEMANAGER_OBJECT_NAME).GetComponent<GameManager>().StopGame();
                GetComponent<SpriteRenderer>().sprite = deadSprite;
            }

            else if(value == true)
                GetComponent<SpriteRenderer>().sprite = aliveSprite;

            birdCage.SetActive(value);
            alive = value;
         }
    }

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0) && alive)
            Fly();

        if(alive)
        {
            float angle = Mathf.Atan2(birdRigidbody.velocity.y, 1) * Mathf.Rad2Deg;
            angle = Mathf.Clamp(angle, -60, 60);
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    private void Fly()
    {
        birdRigidbody.velocity = Vector2.zero;
        birdRigidbody.AddForce(Vector2.up * flyForce, ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        Alive = false;
    }

    public void Init()
    {
        birdRigidbody.gravityScale = 1;
        Alive = true;
        transform.position = startPosition;
        transform.rotation = Quaternion.identity;
    }
}
