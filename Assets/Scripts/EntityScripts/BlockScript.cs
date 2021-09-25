using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockScript : EntityManager
{
    List<Rigidbody2D> collidedObjects = new List<Rigidbody2D>();
    private void OnCollisionEnter2D(Collision2D collision) {
        transform.TryGetComponent(out Rigidbody2D rb);
        bool b1 = collision.transform.TryGetComponent(out BlockScript _);
        bool b2 = collision.transform.TryGetComponent(out Rigidbody2D otherRB);
        if (b1 && b2) {
            collidedObjects.Add(otherRB);
            rb.isKinematic = true;
            rb.useFullKinematicContacts = true;
            rb.velocity = Vector2.zero;
            Vector3 posDifference = transform.position - collision.transform.position;
            rb.MovePosition(transform.position - (posDifference * 0.05f));
        }
    }

    private void OnCollisionExit2D(Collision2D collision) {
        transform.TryGetComponent(out Rigidbody2D rb);
        if (collision.transform.TryGetComponent(out Rigidbody2D otherRB)) {
            if (collidedObjects.Contains(otherRB)) {
                collidedObjects.Remove(otherRB);
                if(collidedObjects.Count == 0) {
                    rb.isKinematic = false;
                }
            }
        }
    }

    private void Start() {
        AddProperty(EnumsAndDictionaries.Properties.Indestructable);
    }
}
