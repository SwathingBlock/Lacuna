using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
public class CollisionDetector : MonoBehaviour
{
    BoxCollider2D crouchCol;
    BoxCollider2D haroCol;
    BoxCollider2D crawlCol;

    CollisionTracker crouch, normal, crawl;

    public CollisionDetector()
    {
        GameObject geometric_info = GameObject.Find("HaroGeometricData");
        crouchCol = GameObject.Find("CrouchSize").GetComponent<BoxCollider2D>();
        haroCol = GameObject.Find("HaroSize").GetComponent<BoxCollider2D>();
        crawlCol = GameObject.Find("CrawlSize").GetComponent<BoxCollider2D>();

        normal = haroCol.GetComponent<CollisionTracker>();
        crouch = crouchCol.GetComponent<CollisionTracker>();
        crawl = crawlCol.GetComponent<CollisionTracker>();
    }

    // True if new Collision is detected
    public Boolean CrouchToNormal()
    {
        Boolean newCollider = DifferentCollider(crouch.GetCollisions(), normal.GetCollisions());
        return newCollider;
    }

    public Boolean CrawlToNormal()
    {
        Boolean newCollider = DifferentCollider(crawl.GetCollisions(), normal.GetCollisions());
        return newCollider;
    }

    public Boolean CrawlToCrouch()
    {
        Boolean newCollider = DifferentCollider(crawl.GetCollisions(), crouch.GetCollisions());
        return newCollider;
    }

    // Block also CrouchToCrawl transition if needed


    private Boolean DifferentCollider(List<Collider2D> present,List<Collider2D> target)
    {
        Boolean r = false;

        foreach(Collider2D c in target)
        {
            if (!present.Contains(c)) return true; 
        }

        return r;
    }
}