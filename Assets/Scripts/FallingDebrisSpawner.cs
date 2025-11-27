using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Kompletny spawner deszczu odłamków: cykle, telegraph, autostart, fallback na kamerę.
public class FallingDebrisSpawner : MonoBehaviour
{
    [Header("Wymagane")]
    public DebrisPiece debrisPrefab;         // prefab odłamka (musi spadać sam po Instantiate+Spawn)

    [Header("Auto-start")]
    public bool autoStart = true;            // startuj automatycznie
    public float initialDelay = 0f;          // opóźnienie startu (0 = od razu)
    public bool firstWaveInstant = true;     // pierwsza fala natychmiast?
    public bool skipTelegraphOnFirst = false;// pominąć ghost przy 1. fali?

    [Header("Obszar (opcjonalnie)")]
    public Transform topLeft;                // jeśli nie ustawisz, użyje kamery
    public Transform topRight;
    public int columns = 7;                  // liczba kolumn

    [Header("Fale i cykle")]
    public int wavesPerCycle = 6;            // ile fal w jednej serii
    public float waveInterval = 0.8f;        // odstęp między falami
    public Vector2Int hitsPerWave = new Vector2Int(2, 3); // kolumn trafianych w fali (min/max)
    public bool loopForever = true;          // zapętlaj serie
    public float cooldownBetweenCycles = 4f; // przerwa między seriami

    [Header("Pozycjonowanie i nieregularność")]
    public float columnJitter = 0.15f;       // +/- losowy X względem środka kolumny
    public float yOffset = 0.1f;             // losowy Y ponad linią sufitu
    public float xDriftMin = -0.3f;          // początkowy dryf poziomy odłamka
    public float xDriftMax =  0.3f;

    [Header("Telegraph (opcjonalny)")]
    public GameObject ghostPrefab;           // półprzezroczysty sprite; puste = bez telegrapha
    public float warnTime = 0.35f;

    // --- runtime / pooling ---
    Camera cam;
    float xMin, xMax, topY, colWidth;
    bool isRunning;

    readonly Queue<GameObject> ghostPool = new();
    readonly Queue<DebrisPiece> debrisPool = new();

    void OnEnable()
    {
        cam = Camera.main;
        RecalcArea();
        if (autoStart) BeginPhase(initialDelay);
    }

    void OnDisable()
    {
        StopAllCoroutines();
        isRunning = false;
    }

    // Przelicza obszar spadania (TopLeft/TopRight lub górna krawędź kamery)
    void RecalcArea()
    {
        if (topLeft && topRight)
        {
            xMin = Mathf.Min(topLeft.position.x, topRight.position.x);
            xMax = Mathf.Max(topLeft.position.x, topRight.position.x);
            topY = topLeft.position.y; // zakładamy ten sam Y
        }
        else
        {
            if (!cam) cam = Camera.main;
            // 95% wysokości viewportu (blisko górnej krawędzi)
            var L = cam.ViewportToWorldPoint(new Vector3(0f, 0.95f, Mathf.Abs(cam.transform.position.z)));
            var R = cam.ViewportToWorldPoint(new Vector3(1f, 0.95f, Mathf.Abs(cam.transform.position.z)));
            xMin = Mathf.Min(L.x, R.x);
            xMax = Mathf.Max(L.x, R.x);
            topY = L.y;
        }
        colWidth = (xMax - xMin) / Mathf.Max(1, columns);
        // Debug.Log($"[DebrisSpawner] Area: x[{xMin:F2},{xMax:F2}] topY={topY:F2} colWidth={colWidth:F2}");
    }

    // Publiczne API: uruchom fazę (z opcjonalnym opóźnieniem)
    public void BeginPhase(float delay = 0f)
    {
        if (isRunning) { StopAllCoroutines(); isRunning = false; }
        StartCoroutine(MainLoop(delay));
    }

    IEnumerator MainLoop(float startDelay)
    {
        isRunning = true;
        if (startDelay > 0f) yield return new WaitForSeconds(startDelay);

        do
        {
            // ewentualna natychmiastowa 1. fala (z lub bez ghostów)
            if (firstWaveInstant)
            {
                yield return SpawnWave(instant: true, skipTelegraph: skipTelegraphOnFirst);
                yield return new WaitForSeconds(waveInterval);
            }

            // pozostałe fale w cyklu
            for (int w = 0; w < wavesPerCycle - (firstWaveInstant ? 1 : 0); w++)
            {
                yield return SpawnWave(instant: false, skipTelegraph: false);
                yield return new WaitForSeconds(waveInterval);
            }

            // przerwa między cyklami
            if (loopForever && cooldownBetweenCycles > 0f)
                yield return new WaitForSeconds(cooldownBetweenCycles);

        } while (loopForever);

        isRunning = false;
    }

    // Jedna fala: wybór kolumn, opcjonalny telegraph, spawn debrisów
    IEnumerator SpawnWave(bool instant, bool skipTelegraph)
    {
        // wybierz liczbę trafień w fali
        int hits = Mathf.Clamp(Random.Range(hitsPerWave.x, hitsPerWave.y + 1), 1, columns);
        var cols = PickColumns(hits);

        // TELEGRAPH (pomijany jeśli brak ghostPrefab albo instant+skipTelegraph)
        if (!instant || !skipTelegraph)
        {
            if (ghostPrefab)
            {
                foreach (int c in cols)
                {
                    Vector3 pos = PosForColumn(c);
                    ShowGhost(pos);
                }
                yield return new WaitForSeconds(instant ? 0f : warnTime);
            }
        }

        // DROP
        foreach (int c in cols)
        {
            Vector3 pos = PosForColumn(c);
            SpawnDebris(pos, Random.Range(xDriftMin, xDriftMax));
        }
    }

    // Pozycja środka kolumny z jitterem
    Vector3 PosForColumn(int c)
    {
        float cx = xMin + colWidth * (c + 0.5f);
        float x = cx + Random.Range(-columnJitter, columnJitter);
        float y = topY + Random.Range(0f, yOffset);
        return new Vector3(x, y, 0f);
    }

    // Unikalne kolumny do trafienia
    HashSet<int> PickColumns(int count)
    {
        var set = new HashSet<int>();
        int tries = 0;
        while (set.Count < count && tries < 100)
        {
            set.Add(Random.Range(0, columns));
            tries++;
        }
        return set;
    }

    // — Pooling (bardzo lekki) —
    void ShowGhost(Vector3 pos)
    {
        GameObject g = ghostPool.Count > 0 ? ghostPool.Dequeue() : Instantiate(ghostPrefab, transform);
        g.transform.position = pos;
        g.SetActive(true);
        StartCoroutine(ReturnGhostAfter(g, warnTime));
    }

    IEnumerator ReturnGhostAfter(GameObject g, float t)
    {
        yield return new WaitForSeconds(t);
        if (g)
        {
            g.SetActive(false);
            ghostPool.Enqueue(g);
        }
    }

    void SpawnDebris(Vector3 pos, float xDrift)
    {
        DebrisPiece d = debrisPool.Count > 0 ? debrisPool.Dequeue() : Instantiate(debrisPrefab);
        d.gameObject.SetActive(true);
        d.Spawn(pos, xDrift);
        StartCoroutine(ReturnDebrisAfter(d, d.lifeTime));
    }

    IEnumerator ReturnDebrisAfter(DebrisPiece d, float t)
    {
        yield return new WaitForSeconds(t);
        if (d)
        {
            d.Despawn();
            debrisPool.Enqueue(d);
        }
    }

    // — Debug helpers —
    [ContextMenu("Spawn ONE debris NOW (center)")]
    void SpawnOneNow()
    {
        RecalcArea();
        Vector3 pos = new Vector3((xMin + xMax) * 0.5f, topY + 0.1f, 0f);
        var d = Instantiate(debrisPrefab, pos, Quaternion.identity);
        d.Spawn(pos, 0f);
        Debug.Log("[FallingDebrisSpawner] SpawnOneNow @ " + pos);
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (!Application.isPlaying) // w edytorze przelicz obszar roboczo
        {
            if (!topLeft || !topRight)
            {
                var c = Camera.main;
                if (c)
                {
                    var L = c.ViewportToWorldPoint(new Vector3(0f, 0.95f, Mathf.Abs(c.transform.position.z)));
                    var R = c.ViewportToWorldPoint(new Vector3(1f, 0.95f, Mathf.Abs(c.transform.position.z)));
                    xMin = Mathf.Min(L.x, R.x);
                    xMax = Mathf.Max(L.x, R.x);
                    topY = L.y;
                }
            }
            else
            {
                xMin = Mathf.Min(topLeft.position.x, topRight.position.x);
                xMax = Mathf.Max(topLeft.position.x, topRight.position.x);
                topY = topLeft.position.y;
            }
            colWidth = (xMax - xMin) / Mathf.Max(1, columns);
        }

        // rysuj linię sufitu i podział kolumn
        Gizmos.color = new Color(0f, 1f, 1f, 0.35f);
        Gizmos.DrawLine(new Vector3(xMin, topY, 0f), new Vector3(xMax, topY, 0f));
        Gizmos.color = new Color(0f, 1f, 1f, 1f);
        for (int i = 0; i <= columns; i++)
        {
            float x = xMin + colWidth * i;
            Gizmos.DrawLine(new Vector3(x, topY, 0f), new Vector3(x, topY - 0.6f, 0f));
        }
    }
#endif
}
