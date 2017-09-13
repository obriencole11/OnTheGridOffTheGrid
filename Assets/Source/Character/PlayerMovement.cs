using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : GridMovement {

    Rigidbody rb;

    private Vector3 lookDirection = new Vector3(1, 0, 0);

    private MarkerObject currentMarker;

    private Collider collider;

    void Awake()
    {
        // Grab a reference to the GridObject
        gridObject = gameObject.GetComponent<GridObject>();
        gridObject.canKill = false;

        rb = gameObject.GetComponent<Rigidbody>();
        collider = gameObject.GetComponent<Collider>();
    }

	void Update () {

        detectMarker();

        if (currentMarker != null)
        {
            if (Input.GetKeyDown("space"))
            {
                if (GameManager.Instance.onGrid)
                {
                    collider.enabled = true;
                    rb.isKinematic = false;
                    transform.position = new Vector3(currentMarker.transform.position.x, 0.0f, currentMarker.transform.position.z);

                    GameManager.Instance.ExitGrid();

                } else
                {
                    collider.enabled = false;
                    rb.isKinematic = true;
                    transform.position = new Vector3(currentMarker.transform.position.x, 0.0f, currentMarker.transform.position.z);

                    GameManager.Instance.EnterGrid();
                }
            }
        }

        float h = Input.GetAxis("Horizontal") * GameManager.Instance.settings.offGridSpeed;
        float v = Input.GetAxis("Vertical") * GameManager.Instance.settings.offGridSpeed;

        if (GameManager.Instance.gameActive)
        {
            if (GameManager.Instance.onGrid)
            {
                if (Input.GetKeyDown("right"))
                {
                    move(Direction.RIGHT);
                }

                if (Input.GetKeyDown("left"))
                {
                    move(Direction.LEFT);
                }

                if (Input.GetKeyDown("up"))
                {
                    move(Direction.UP);
                }

                if (Input.GetKeyDown("down"))
                {
                    move(Direction.DOWN);
                }
            } else
            {
                

                // force = acceleration
                // acceleration = vDesired - vCurrent
                Vector3 force = new Vector3((h - rb.velocity.x), 0.0f, (v - rb.velocity.z));

                // Move the player
                rb.AddForce(force, ForceMode.Impulse);

                
            }

            if (Mathf.Abs(h) > Mathf.Abs(v))
            {
                if (h != 0) lookDirection = new Vector3(Mathf.Sign(h) * 1, 0, 0);
            }
            else
            {
                if (v != 0) lookDirection = new Vector3(0, 0, Mathf.Sign(v) * 1);
            }

        }
    }

    private void detectMarker()
    {
        // Look for colliders
        int layerMask = 1 << 8;
        Collider[] directionColliders = Physics.OverlapSphere(transform.position + lookDirection + new Vector3(0,0.5f,0), 0.5f, layerMask);
        Collider[] baseColliders = Physics.OverlapSphere(transform.position + new Vector3(0, 0.5f, 0), 1.25f, layerMask);

        if (directionColliders.Length > 0)
        {
            MarkerObject marker = closestCollider(directionColliders).transform.gameObject.GetComponent<MarkerObject>();
            if (marker != null) {
                
                if (currentMarker != null) currentMarker.deactivate();
                currentMarker = marker;
                currentMarker.activate();
            }
        } else if (baseColliders.Length > 0)
        {
            MarkerObject marker = closestCollider(baseColliders).transform.gameObject.GetComponent<MarkerObject>();
            if (marker != null)
            {
                
                currentMarker.deactivate();
                currentMarker = marker;
                currentMarker.activate();
            }
        } else
        {
            if (currentMarker != null) currentMarker.deactivate();
        }


    }

    private Collider closestCollider(Collider[] colliders)
    {
        Collider closest = null;

        foreach (Collider col in colliders)
        {
            if (closest == null)
            {
                closest = col;
            } else
            {
                float currentDistance = Vector3.Distance(col.transform.position, transform.position);
                float closestDistance = Vector3.Distance(closest.transform.position, transform.position);

                if (currentDistance < closestDistance)
                {
                    closest = col;
                }
            }
        }

        return closest;
    }

    private void move(Direction nextDirection)
    {
        if (GameManager.Instance.gridManager.exists(GridTools.directionPosition(gridObject, nextDirection)))
        {
            gridObject.setNextDirection(nextDirection);
            GameManager.Instance.advanceTurn();
        }
    }

    public void setDestroyed(bool destroyed)
    {

    }

    void OnDrawGizmos()
    {
        //Gizmos.color = Color.red;
        //Gizmos.DrawSphere(transform.position + lookDirection + new Vector3(0, 0.5f, 0), 0.5f);
        //Gizmos.color = Color.yellow;
        //Gizmos.DrawSphere(transform.position + new Vector3(0, 0.5f, 0), 1.25f);
    }

}
