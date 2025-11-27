using UnityEngine;

public class Pickup2D : MonoBehaviour
{
    public float gateOpenSeconds = 5f;    // jak długo otwiera się ściana
    public GateController2D gate;         // przeciągnij bramkę w prefabie
    public System.Action onCollected;     // ustawi to spawner

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        gate?.OpenFor(gateOpenSeconds);
        onCollected?.Invoke();
        Destroy(gameObject);
    }
}
