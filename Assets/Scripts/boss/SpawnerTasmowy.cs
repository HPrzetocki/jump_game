using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Spawner taśmy platform: nieregularny timing, rosnąca prędkość, brak flipów.
/// Każda platforma na starcie ma hazard na górze (Top) albo na dole (Bottom).
public class SpawnerTasmowy : MonoBehaviour
{
    [Header("Prefab i miejsce spawnu")]
    public PlatformaTasmowa prefab;      // prefab z PlatformaTasmowa + Init(HazardSide)
    public Transform spawnPoint;         // jeśli puste, użyje pozycji obiektu ze spawnerem

    [Tooltip("Jeśli true, spawnuje tuż za prawą krawędzią kamery (wygodne przy ruchomej kamerze).")]
    public bool spawnRelativeToCamera = true;
    public Camera cam;                   // domyślnie Camera.main
    [Tooltip("O ile jednostek poza prawą krawędzią kamery spawnować.")]
    public float offsetFromRight = 0.5f;

    [Header("Czas spawnu (nieregularny)")]
    [Tooltip("Minimalna przerwa między platformami (sekundy).")]
    public float spawnMin = 0.75f;
    [Tooltip("Maksymalna przerwa między platformami (sekundy).")]
    public float spawnMax = 1.35f;

    [Header("Prędkość platform (rampa w czasie)")]
    [Tooltip("t = sekundy od startu fazy, value = prędkość platformy (u/s).")]
    public AnimationCurve speedOverTime = AnimationCurve.EaseInOut(0f, 2.4f, 20f, 4.2f);

    [Header("Różnicowanie pozycji")]
    [Tooltip("Losowy jitter w osi Y dla urozmaicenia ścieżki (0 = wyłączony).")]
    public float verticalJitter = 0f;

    [Header("Losowanie typu hazardu")]
    [Range(0f, 1f)]
    [Tooltip("Prawdopodobieństwo, że platforma będzie z kolcem U GÓRY (Top). Pozostałe przypadki = dół (Bottom).")]
    public float probabilityTopHazard = 0.5f;

    [Tooltip("Ułatwienie debug: jeśli true, nadpisze losowanie i wymusi jeden typ.")]
    public bool forceSingleHazardForDebug = false;
    public HazardSide debugHazardSide = HazardSide.Top;

    [Header("Zatrzymywanie kilku na koniec (mostek)")]
    [Tooltip("Ile ostatnich platform zatrzymać na stałe, gdy wezwiesz ZatrzymajOstatnieNaMiejscu().")]
    public int ileZatrzymacNaKoniec = 4;
    [Tooltip("Miejsca docelowe dla zatrzymanych platform (ustaw w scenie).")]
    public Transform[] stopPositions;

    [Tooltip("Po tylu sekundach od startu fazy zacznij zbierać 'ostatnie' platformy do kolejki (do późniejszego zatrzymania). 0 = od razu.")]
    public float czasDoZatrzymywania = 10f;

    [Tooltip("Jeśli true, spawner sam zawoła ZatrzymajOstatnieNaMiejscu() gdy uzbiera wystarczającą liczbę platform.")]
    public bool zatrzymujAutomatycznieNaKoncu = false;

    // --- runtime ---
    private Coroutine loop;
    private readonly Queue<PlatformaTasmowa> kolejkaOstatnich = new Queue<PlatformaTasmowa>();
    private float startTime;
    private bool collectLast;

    void OnEnable()
    {
        if (spawnRelativeToCamera && cam == null) cam = Camera.main;
        startTime = Time.time;
        collectLast = (czasDoZatrzymywania <= 0f);
        loop = StartCoroutine(Loop());
    }

    void OnDisable()
    {
        if (loop != null) StopCoroutine(loop);
        loop = null;
        kolejkaOstatnich.Clear();
    }

    IEnumerator Loop()
    {
        while (true)
        {
            // nieregularny odstęp między spawnami
            float delay = Random.Range(spawnMin, spawnMax);
            yield return new WaitForSeconds(delay);

            // po zadanym czasie zacznij kolejkować „ostatnie”
            if (!collectLast && (Time.time - startTime) >= czasDoZatrzymywania)
                collectLast = true;

            SpawnOne();

            // auto-stop (opcjonalnie)
            if (zatrzymujAutomatycznieNaKoncu && collectLast &&
                kolejkaOstatnich.Count >= ileZatrzymacNaKoniec &&
                stopPositions != null && stopPositions.Length > 0)
            {
                ZatrzymajOstatnieNaMiejscu();
                zatrzymujAutomatycznieNaKoncu = false; // wykonaj raz
            }
        }
    }

    void SpawnOne()
    {
        if (!prefab)
        {
            Debug.LogWarning("[SpawnerTasmowy] Brak przypiętego prefabu PlatformaTasmowa.");
            return;
        }

        // pozycja spawnu
        Vector3 pos = spawnPoint ? spawnPoint.position : transform.position;

        if (spawnRelativeToCamera && cam)
        {
            // prawa krawędź widoku kamery, środek w pionie
            Vector3 right = cam.ViewportToWorldPoint(new Vector3(1f, 0.5f, Mathf.Abs(cam.transform.position.z)));
            pos.x = right.x + offsetFromRight;
        }

        if (verticalJitter != 0f)
            pos.y += Random.Range(-verticalJitter, verticalJitter);

        // utwórz platformę
        PlatformaTasmowa p = Instantiate(prefab, pos, Quaternion.identity);

        // ustaw hazard typu (Top/Dół) JUŻ NA STARcie
        HazardSide side = forceSingleHazardForDebug
            ? debugHazardSide
            : (Random.value < probabilityTopHazard ? HazardSide.Top : HazardSide.Bottom);

        p.Init(side);

        // ustaw prędkość wg krzywej czasu
        float t = Time.time - startTime; // sekundy od startu fazy
        p.speed = speedOverTime.Evaluate(t);

        // jeśli zbieramy „ostatnie” – zapamiętaj
        if (collectLast)
        {
            kolejkaOstatnich.Enqueue(p);
            while (kolejkaOstatnich.Count > ileZatrzymacNaKoniec)
                kolejkaOstatnich.Dequeue();
        }
    }

    /// Zatrzymuje ostatnie N platform i ustawia je na pozycjach z 'stopPositions'.
    /// Wywołaj to, gdy chcesz zakończyć taśmę i zostawić mostek do kolejnej fazy.
    public void ZatrzymajOstatnieNaMiejscu()
    {
        int i = 0;
        foreach (var p in kolejkaOstatnich)
        {
            if (!p) continue;

            if (stopPositions != null && i < stopPositions.Length)
                p.transform.position = stopPositions[i].position;

            p.ZatrzymajIUtrzymaj(); // przestaje jechać; zostaje w swoim typie (Top/Bottom)
            i++;
        }
        kolejkaOstatnich.Clear();
    }

    // Możesz tymi metodami ręcznie sterować, np. z FSM bossa:
    public void RozpocznijZbieranieOstatnich() => collectLast = true;
    public void PrzestanZbieracOstatnie() { collectLast = false; kolejkaOstatnich.Clear(); }
}
