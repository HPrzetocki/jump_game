using UnityEngine;

public class CameraAspectRatio : MonoBehaviour
{
    void Start()
    {
        // 1. Ustawiamy sztywno cel: 16:9
        float targetAspect = 16.0f / 9.0f;

        // 2. Pobieramy aktualne wymiary okna gry
        float windowAspect = (float)Screen.width / (float)Screen.height;

        // 3. Obliczamy ró¿nicê
        float scaleHeight = windowAspect / targetAspect;

        Camera camera = GetComponent<Camera>();

        // Sytuacja A: Monitor jest szerszy ni¿ 16:9 (np. Ultrawide)
        // Musimy dodaæ pasy po bokach (PILLARBOX)
        if (scaleHeight > 1.0f)
        {
            float scaleWidth = 1.0f / scaleHeight;

            Rect rect = camera.rect;
            rect.width = scaleWidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scaleWidth) / 2.0f; // Centrowanie w poziomie
            rect.y = 0;

            camera.rect = rect;
        }
        // Sytuacja B: Monitor jest wê¿szy/wy¿szy ni¿ 16:9 (np. 16:10, stare monitory, "dziwne okno")
        // Musimy dodaæ pasy góra/dó³ (LETTERBOX), ¿eby nie rozci¹gn¹æ obrazu
        else
        {
            Rect rect = camera.rect;
            rect.width = 1.0f;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1.0f - scaleHeight) / 2.0f; // Centrowanie w pionie

            camera.rect = rect;
        }
    }
}