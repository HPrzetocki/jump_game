using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawnDirector2D : MonoBehaviour
{
    [Header("Co spawnujemy")]
    public GameObject pickupPrefab;

    [Header("Gdzie może się pojawić (punkty nad platformami)")]
    public List<Transform> spawnPoints = new();   // w Inspectorze dodaj punkty

    [Header("Zachowanie")]
    public Vector2 respawnEveryRange = new Vector2(8f, 12f); // losowo co 8–12 s
    public bool avoidRepeats = true;              // nie powtarzaj tej samej platformy
    public GateController2D gate;                 // opcjonalnie: bramka/ściana do otwierania

    private GameObject current;
    private int lastIndex = -1;

    void Start() => StartCoroutine(Loop());

    private IEnumerator Loop()
    {
        // krótki startowy delay
        yield return new WaitForSeconds(Random.Range(0.5f, 1.5f));

        while (true)
        {
            if (current == null)
            {
                int idx = ChooseSpawnIndex();
                Vector3 pos = spawnPoints[idx].position;

                current = Instantiate(pickupPrefab, pos, Quaternion.identity);

                // podaj referencje do bramki + callback po zebraniu
                var p = current.GetComponent<Pickup2D>();
                if (p)
                {
                    if (!p.gate) p.gate = gate;
                    p.onCollected = () => current = null;
                }

                lastIndex = idx;
            }

            // czekaj aż obecny zniknie (zebrany)
            yield return new WaitUntil(() => current == null || current.Equals(null));

            // odlicz losowy respawn
            float delay = Random.Range(respawnEveryRange.x, respawnEveryRange.y);
            yield return new WaitForSeconds(delay);
        }
    }

    private int ChooseSpawnIndex()
    {
        if (spawnPoints == null || spawnPoints.Count == 0)
            throw new System.Exception("Brak spawnPoints w ItemSpawnDirector2D.");

        if (!avoidRepeats || spawnPoints.Count == 1)
            return Random.Range(0, spawnPoints.Count);

        int i;
        do { i = Random.Range(0, spawnPoints.Count); } while (i == lastIndex);
        return i;
    }

    void OnDrawGizmos()
    {
        if (spawnPoints == null) return;
        Gizmos.color = Color.yellow;
        foreach (var t in spawnPoints)
            if (t) Gizmos.DrawWireSphere(t.position, 0.2f);
    }
}
