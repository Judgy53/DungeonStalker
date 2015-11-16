using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DynamicObstacle : MonoBehaviour
{
    public float staticActualizeTimer = 0.5f;

    private bool hasChanged = false;
    private float timeSinceLastChange = 0.0f;

    private Node[] affectedNodes = new Node[0];

    private Matrix4x4 oldMat = new Matrix4x4();
    private bool processing = false;

    void Start()
    {
    }

    void Update()
    {
        if (hasChanged)
        {
            if (oldMat == transform.worldToLocalMatrix)
                timeSinceLastChange += Time.unscaledDeltaTime;
            else
                timeSinceLastChange = 0.0f;

            if (timeSinceLastChange > staticActualizeTimer && !processing)
            {
                Bounds bounds = new Bounds(transform.position, Vector3.zero);
                foreach (var collider in GetComponentsInChildren<Collider>())
                    bounds.Encapsulate(collider.bounds);

                processing = Grid.EnqueueRecomputeObstacle(bounds, affectedNodes, OnFinishedProcess);
            }
        }
        else
        {
            hasChanged = oldMat != transform.worldToLocalMatrix;
            timeSinceLastChange = 0.0f;
        }

        oldMat = transform.worldToLocalMatrix;
    }

    private void OnFinishedProcess(Node[] affectedNodes)
    {
        //Debug.Log("Finished processing !");
        this.affectedNodes = affectedNodes;
        processing = false;
        hasChanged = false;
    }
}
