using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class MaceController : MonoBehaviour
{
    private enum State
    {
        Slam,
        Rest,
        Reset
    }

    [SerializeField, Tooltip("Input the player character.")]
    private GameObject player;

    [SerializeField, Tooltip("Mace downwards speed")]
    private float downwardSpeed;
    [SerializeField, Tooltip("Mace upwards speed")] 
    private float upwardSpeed;
    [SerializeField, Tooltip("Slam cooldown")]
    private float cooldown;

    private Vector3 restingPosition;
    private State state;
    // Start is called before the first frame update
    void Start()
    {
        restingPosition = transform.position;
        state = State.Slam;
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case State.Slam:
                float step = downwardSpeed * Time.deltaTime;
                transform.position += (Vector3.down * step);
                break;
            case State.Reset:
                HandleReset();
                break;
            default:
                break;
        }
    }

    private void HandleReset()
    {
        float step = upwardSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, restingPosition, step);
        if (transform.position == restingPosition)
        {
            state = State.Rest;
            StartCoroutine(HandleSlamCooldown());
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Ground"))
        {
            state = State.Reset;
        }
    }

    private IEnumerator HandleSlamCooldown()
    {
        yield return new WaitForSeconds(cooldown);
        state = State.Slam;
    }
}
