using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MarkerObject : MonoBehaviour {

    protected bool activated = false;
    protected Renderer r;
    protected Collider col;
    protected bool enabled = true;

    void Awake()
    {
        r = GetComponent<Renderer>();
        col = GetComponent<Collider>();
        r.enabled = false;
    }

    protected abstract bool clear();

    protected void disable()
    {
        col.enabled = false;
        r.enabled = false;
        enabled = false;
    }

    protected void enable()
    {
        if (clear())
        {
            col.enabled = true;
            enabled = true;
        }
    }

    public void activate()
    {
        activated = true;
        r.enabled = activated;
    }

    public void deactivate()
    {
        activated = false;
        r.enabled = activated;
    }

}
