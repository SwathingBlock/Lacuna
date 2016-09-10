using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CollisionTracker : MonoBehaviour {

    // Active collisions
    private List<Collider2D> collisions = new List<Collider2D>();

    public int ActiveCollisions = 0;// Only visual information, do not alter 

    public List<Collider2D> GetCollisions() { return collisions; }

    // Active collisions management
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.isTrigger) return;
        ActiveCollisions++;
        collisions.Add(other);
    }
    void OnTriggerExit2D (Collider2D other){
        if (other.isTrigger) return;

        ActiveCollisions--;
        collisions.Remove(other);		
	}
	
}
