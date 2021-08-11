using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TimedGate : MonoBehaviour
{
    [SerializeField] float delay;
    [SerializeField] GameObject door;
    [SerializeField] int yeetPower;
    Coroutine closeRoutine;
    public LayerMask playerMask;


    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        door.SetActive(false);

        if (closeRoutine != null)
            StopCoroutine(closeRoutine);
        closeRoutine = StartCoroutine(Close(other));
    }

    IEnumerator Close(Collider player)
    {
        yield return new WaitForSeconds(delay);

        if (Vector3.Distance(player.transform.position, door.transform.position) <= door.transform.localScale.x)
        {
            Vector3 direction = (player.transform.position - door.transform.position).normalized;
            direction.y = 0.5f;
            player.attachedRigidbody.AddRelativeForce(direction * yeetPower);
        }
        door.SetActive(true);
    }

}
