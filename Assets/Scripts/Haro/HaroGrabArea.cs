using System;
using System.Collections;
using UnityEngine;


public class HaroGrabArea : MonoBehaviour
{

    ArrayList activeColliders = new ArrayList();

    public int counter = 0;

    void OnTriggerEnter2D(Collider2D other)
        {
        // ignore non grab zones
        if (other.CompareTag("Ledge"))
        {
            activeColliders.Add(other);
            SendMessageUpwards("OnGrabLedge", other.transform.position);
        }
        counter = activeColliders.Count;
        }

  void OnTriggerExit2D(Collider2D other)
    {
        // remove grab zones
        if (other.CompareTag("Ledge")) activeColliders.Remove(other);
        counter = activeColliders.Count;
    }

    public bool OnGrabArea() { return activeColliders.Count > 0; }
}
