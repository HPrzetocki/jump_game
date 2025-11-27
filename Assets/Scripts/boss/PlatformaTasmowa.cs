using UnityEngine;

public enum HazardSide { Bottom, Top }   // gdzie jest hazard

public class PlatformaTasmowa : MonoBehaviour
{

public Vector2 CurrentDelta { get; private set; }           // już masz
public Vector2 SurfaceVelocity => CurrentDelta / Time.fixedDeltaTime;


    [Header("Ruch")]
    public float speed = 2.4f;
    public bool jestZatrzymana;

    [Header("Stan")]
    public HazardSide hazardSide = HazardSide.Bottom; // domyślnie dół kłuje
    private bool initialized;

    [Header("Refs")]
    public Rigidbody2D rb;
    public PlatformEffector2D topEffector;   // na ROOT
    public Collider2D topCollider;           // BoxCollider2D na ROOT (Used By Effector = ON)
    public Collider2D hazardBottom;          // TRIGGER (dziecko)
    public Collider2D hazardTop;             // TRIGGER (dziecko)
    public GameObject belkaDolna;            // mały podest pod spodem (opcjonalnie)
    public SpriteRenderer sr;

    [Header("Wizual kolców (opcjonalne)")]
    public GameObject spikeVisualTop;        // włączany przy Top
    public GameObject spikeVisualBottom;     // włączany przy Bottom
    public bool flipRootVisualYWhenTop;      // gdy masz jeden sprite – odwróć go w osi Y dla Top
    public Transform rootVisual;             // referencja do obiektu z grafiką (jeśli flipujesz)

    [Header("Ustawienia")]
    public bool useBelkaDolna = true;        // czy w trybie Top dawać bezpieczną belkę pod spodem

    [Header("Kolory")]
    public Color safeTop    = new(0.6f, 1f, 0.6f);   // „góra bezpieczna” (Bottom hazard)
    public Color safeBottom = new(0.6f, 0.8f, 1f);   // „dół bezpieczny” (Top hazard)

    void Awake()
    {
        if (!rb) rb = GetComponent<Rigidbody2D>();
        if (!topEffector) topEffector = GetComponent<PlatformEffector2D>();

        // auto-znajdź i przygotuj górny collider
        if (!topCollider)
        {
            var bc = GetComponent<BoxCollider2D>();
            if (bc) { bc.usedByEffector = true; topCollider = bc; }
        }
        else if (topCollider is BoxCollider2D bc)
        {
            bc.usedByEffector = true;
        }

        if (!sr) sr = GetComponent<SpriteRenderer>();

        // sanity dla triggerów
        if (hazardBottom is Collider2D hb) hb.isTrigger = true;
        if (hazardTop    is Collider2D ht) ht.isTrigger = true;
    }

    void OnEnable()
    {
        jestZatrzymana = false;
        if (!initialized) ApplyHazard(hazardSide);  // ważne: nic nie „flipuje” w trakcie
    }

void FixedUpdate()
{
    if (!jestZatrzymana && rb)
    {
        var delta = Vector2.left * speed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + delta);
        CurrentDelta = delta;                // KLUCZ
    }
    else
    {
        CurrentDelta = Vector2.zero;
    }
}

    /// Wywołaj ZARAZ po Instantiate, żeby ustawić typ platformy.
    public void Init(HazardSide side)
    {
        hazardSide = side;
        initialized = true;
        ApplyHazard(side);
    }

    /// Publiczne – gdybyś chciał ręcznie wymusić po zmianie w edytorze w Play.
    public void ApplyHazard(HazardSide side)
    {
        bool topIsHazard = (side == HazardSide.Top);

        // Stać na górze można tylko, gdy hazard jest na dole:
        if (topCollider)     topCollider.enabled   = !topIsHazard;

        // Triggery obrażeń (włącz jeden, wyłącz drugi)
        if (hazardBottom)    hazardBottom.enabled  = !topIsHazard;
        if (hazardTop)       hazardTop.enabled     =  topIsHazard;

        // Belka „od spodu” tylko gdy hazard jest na górze (opcjonalnie):
        if (belkaDolna)      belkaDolna.SetActive(useBelkaDolna && topIsHazard);

        // Wizual kolców
        if (spikeVisualTop)    spikeVisualTop.SetActive(topIsHazard);
        if (spikeVisualBottom) spikeVisualBottom.SetActive(!topIsHazard);

        if (flipRootVisualYWhenTop && rootVisual)
        {
            var s = rootVisual.localScale;
            s.y = Mathf.Abs(s.y) * (topIsHazard ? -1f : 1f);
            rootVisual.localScale = s;
        }

        // Kolor informacyjny:
        if (sr)              sr.color = topIsHazard ? safeBottom : safeTop;

#if UNITY_EDITOR
        // pomocny warning w Play Mode – jeśli trigger nie ma DamageZone2D
        if (Application.isPlaying)
        {
            if (topIsHazard && hazardTop && !hazardTop.GetComponent<DamageZone2D>())
                Debug.LogWarning($"[{name}] HazardTop włączony, ale brak DamageZone2D.");
            if (!topIsHazard && hazardBottom && !hazardBottom.GetComponent<DamageZone2D>())
                Debug.LogWarning($"[{name}] HazardBottom włączony, ale brak DamageZone2D.");
        }
#endif
    }

    public void ZatrzymajIUtrzymaj()
    {
        jestZatrzymana = true;
        if (rb) rb.velocity = Vector2.zero;
    }

    void OnBecameInvisible()
    {
        if (!jestZatrzymana)
            gameObject.SetActive(false);
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        // auto-wire + sanity, działa też w edytorze
        if (!rb) rb = GetComponent<Rigidbody2D>();
        if (!topEffector) topEffector = GetComponent<PlatformEffector2D>();

        var bc = GetComponent<BoxCollider2D>();
        if (!topCollider && bc) topCollider = bc;
        if (bc) bc.usedByEffector = true;

        if (hazardBottom) hazardBottom.isTrigger = true;
        if (hazardTop)    hazardTop.isTrigger    = true;

        if (!sr) sr = GetComponent<SpriteRenderer>();

        // w edytorze, gdy zmienisz hazardSide, odśwież od razu
        if (Application.isPlaying == false)
            ApplyHazard(hazardSide);
    }
#endif
}
