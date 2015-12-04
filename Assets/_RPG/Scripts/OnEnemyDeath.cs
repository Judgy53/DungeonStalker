using UnityEngine;
using System.Collections;

public class OnEnemyDeath : MonoBehaviour
{
    private Container container = null;

    void OnEnable()
    {
        container = GetComponent<Container>();

        StartCoroutine("TryDestroy");
    }

    private IEnumerator TryDestroy()
    {
        yield return null; // wait one frame to let the container fill

        while (container != null && container.Items.Length > 0)
            yield return null; // check each frame

        Destroy(gameObject, 5.0f);
    }
}