using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockScript : EntityManager
{
    //public GameObject platform;
    //float timer;
    //private bool onGround;

    //private void OnTriggerStay2D(Collider2D collision) {
    //    if (collision.transform.CompareTag("Ground")) {
    //        onGround = true;
    //        timer = 0;
    //    } else if (collision.transform.CompareTag("Void")) {
    //        onGround = false;
    //        timer += Time.deltaTime;
    //    }
    //}

    //private void Update() {
    //    if (!onGround && timer > 1f) {
    //        EntityDeath();
    //    }
    //}

    //private void EntityDeath() {
    //    Instantiate(platform, transform.position, transform.rotation);
    //    Destroy(this.gameObject);
    //}

    /*
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class BlockScript : MonoBehaviour
        {
        public Vector2 instantVelocity = Vector2.zero;
        private void FixedUpdate() {
            GetComponent<Rigidbody2D>().MovePosition(instantVelocity);
            instantVelocity *= 1 - Time.deltaTime;
        }
    }

    */
}
