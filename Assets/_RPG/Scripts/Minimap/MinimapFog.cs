using UnityEngine;
using System.Collections.Generic;

public class MinimapFog : MonoBehaviour {

    [SerializeField]
    private GameObject player;

    private Texture2D texture;

    [SerializeField]
    private float holeSize = 10f;

	// Use this for initialization
	void Start () {
        texture = new Texture2D((int)(transform.localScale.x * 10f), (int)(transform.localScale.z * 10f));
        GetComponent<Renderer>().material.mainTexture = texture;

        for (int x = 0; x < texture.width; x++)
            for (int y = 0; y < texture.height; y++)
                texture.SetPixel(x, y, Color.black);

        texture.Apply();
	}

    void FixedUpdate()
    {
        if (player == null)
            return;

        Vector2 point = new Vector2(player.transform.position.x, player.transform.position.z) + new Vector2(texture.width /2f, texture.height /2f);

        for (int x = (int)(point.x - holeSize / 2f); x < point.x + holeSize / 2f; x++)
            for (int y = (int)(point.y - holeSize / 2f); y < point.y + holeSize / 2f; y++)
                if (Vector2.Distance(point, new Vector2(x, y)) < holeSize / 2f)
                    texture.SetPixel(-x, -y, new Color(0f, 0f, 0f, 0f));

        texture.Apply();
    }
}
