using UnityEngine;

public class DropOneNow : MonoBehaviour
{
    public DebrisPiece debrisPrefab;  // ← przeciągnij tu swój prefab DebrisPiece

    void Start()
    {
        if (!debrisPrefab) { Debug.LogError("[DropOneNow] Brak debrisPrefab"); return; }

        var cam = Camera.main;
        if (!cam) { Debug.LogError("[DropOneNow] Brak Camera.main"); return; }

        // pozycja tuż pod górną krawędzią ekranu, środek X
        var pos = cam.ViewportToWorldPoint(new Vector3(0.5f, 0.95f, Mathf.Abs(cam.transform.position.z)));
        pos.z = 0f;

        var d = Instantiate(debrisPrefab, pos, Quaternion.identity);
        d.Spawn(pos, 0f);
        Debug.Log("[DropOneNow] Spawned at " + pos);
    }
}
