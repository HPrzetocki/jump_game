using UnityEngine;
using UnityEngine.UI;

public class JumpChargeUI : MonoBehaviour
{
    [Header("1. Przypisz obiekty")]
    [SerializeField] private PlayerController2 playerController;
    [SerializeField] private Slider chargeSlider;
    [SerializeField] private Image fillImage;

    [Header("2. Ustawienia")]
    [SerializeField] private Gradient chargeGradient;
    [Tooltip("Wpisz tu dok³adnie tak¹ sam¹ wartoœæ jak w skrypcie gracza (90)")]
    [SerializeField] private float maxJumpForce = 90f;

    void Start()
    {
        if (chargeSlider == null || fillImage == null || playerController == null)
        {
            Debug.LogError("JumpChargeUI: BRAKUJE PRZYPISAÑ W INSPEKTORZE! SprawdŸ pola.");
            return;
        }

        // ZMIANA 1: Na starcie upewniamy siê, ¿e pasek JEST W£¥CZONY
        chargeSlider.gameObject.SetActive(true);

        // Zerujemy go i ustawiamy pocz¹tkowy kolor
        UpdateVisuals(0f);
    }

    void Update()
    {
        if (playerController == null) return;

        float progress = 0f;

        // Jeœli gracz ³aduje skok, obliczamy ile % si³y ma
        if (playerController.preJump)
        {
            float currentForce = playerController.jumpForce;
            progress = Mathf.Clamp01(currentForce / maxJumpForce);
        }
        // ZMIANA 2: Jeœli NIE ³aduje, progress po prostu zostaje 0 (nie wy³¹czamy obiektu)

        UpdateVisuals(progress);
    }

    private void UpdateVisuals(float progress)
    {
        // Ustaw wartoœæ suwaka
        chargeSlider.value = progress;

        // Ustaw kolor (dla 0 bêdzie to pocz¹tek gradientu, np. zielony)
        if (fillImage != null)
        {
            fillImage.color = chargeGradient.Evaluate(progress);
        }
    }
}