using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIButtonSound : MonoBehaviour
{
    [SerializeField] private AudioClip clickSound;
    [SerializeField] private float volume = 1f;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        Debug.Log($"[UIButtonSound] Awake na obiekcie: {name}, button != null: {button != null}");

        if (button != null)
        {
            button.onClick.AddListener(PlayClickSound);
        }
        else
        {
            Debug.LogError("[UIButtonSound] Brak komponentu Button na tym obiekcie!");
        }
    }

    public void PlayClickSound()
    {
        Debug.Log("[UIButtonSound] PlayClickSound wywołane");

        if (clickSound == null)
        {
            Debug.LogWarning("[UIButtonSound] clickSound NIE jest przypisany!");
            return;
        }

        if (SoundFXManager.instance == null)
        {
            Debug.LogError("[UIButtonSound] SoundFXManager.instance == null! Brak SoundFXManager w scenie?");
            return;
        }

        SoundFXManager.instance.PlaySoundFXClip(clickSound, transform, volume);
    }
}
