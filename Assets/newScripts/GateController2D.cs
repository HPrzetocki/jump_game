using System.Collections;
using UnityEngine;

public class GateController2D : MonoBehaviour
{
    [Header("Pozycje (ustaw w Inspectorze)")]
    public Transform closedPoint;   // gdzie stoi na początku
    public Transform openPoint;     // gdzie ma się podnieść
    [Header("Ruch")]
    public float moveSpeed = 3f;    // prędkość przesuwu

    private Coroutine runCo;
    private float holdUntil = 0f;   // czas (Time.time), do którego ma być otwarte

    void Awake()
    {
        if (closedPoint) transform.position = closedPoint.position;
    }

    /// <summary>Otwórz bramkę i trzymaj przez 'seconds'. Jeśli już otwarta – wydłuż czas.</summary>
    public void OpenFor(float seconds)
    {
        holdUntil = Mathf.Max(holdUntil, Time.time + seconds);
        if (runCo == null) runCo = StartCoroutine(OpenCloseLoop());
    }

    private IEnumerator OpenCloseLoop()
    {
        // Otwieranie
        yield return MoveTo(openPoint.position);

        // Trzymanie otwartej
        while (Time.time < holdUntil) yield return null;

        // Zamknięcie
        yield return MoveTo(closedPoint.position);

        runCo = null;
    }

    private IEnumerator MoveTo(Vector3 target)
    {
        while ((transform.position - target).sqrMagnitude > 0.0004f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = target;
    }
}
