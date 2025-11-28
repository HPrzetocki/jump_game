using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Analytics;
using System.Threading.Tasks;

public class AnalyticsInitializer : MonoBehaviour
{
    async void Awake()
    {
        try
        {
            // 1. Inicjalizacja wszystkich Unity Gaming Services
            await UnityServices.InitializeAsync();

            // 2. Tutaj normalnie powinieneś ogarnąć zgodę gracza (RODO itp.)
            // Dla testów przyjmijmy, że gracz się zgodził:
            AnalyticsService.Instance.StartDataCollection(); // dla SDK <= 6.0 :contentReference[oaicite:3]{index=3}

            Debug.Log("Analytics: initialized + data collection started");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Analytics initialization failed: " + e);
        }

        DontDestroyOnLoad(gameObject); // żeby nie ginął między scenami
    }
}
