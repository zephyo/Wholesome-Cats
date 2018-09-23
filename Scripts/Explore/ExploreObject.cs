using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExploreObject : MonoBehaviour {

    protected Rigidbody2D rigidBody;


    public Rigidbody2D getRigidBody2D() {
        if (rigidBody != null)
        {
            return rigidBody;
        }
        rigidBody = this.GetComponent<Rigidbody2D>();
        return rigidBody;
    }
}
