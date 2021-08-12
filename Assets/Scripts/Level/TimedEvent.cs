using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TimedEvent : MonoBehaviour
{
    [SerializeField] float ammOfTimeUntilEventEnds;
    Coroutine m_CloseRoutine;

    [SerializeField] private UnityEvent onTimerStart, onTimerEnd;


    public void StartTimer(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        onTimerStart?.Invoke();

        if (m_CloseRoutine != null)
            StopCoroutine(m_CloseRoutine);
        m_CloseRoutine = StartCoroutine(Close(other));
    }

    IEnumerator Close(Collider player)
    {
        yield return new WaitForSeconds(ammOfTimeUntilEventEnds);
        onTimerEnd?.Invoke();
    }
}