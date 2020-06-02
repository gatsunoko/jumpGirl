using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBoundScript : MonoBehaviour {

  GameObject player;
  Rigidbody2D rigid2d;
  float oldVelocityX = 0;

  void Start() {
    this.player = PlayerScript.Instance.gameObject;
    this.rigid2d = this.player.GetComponent<Rigidbody2D>();
  }

  void Update() {
    this.oldVelocityX = this.rigid2d.velocity.x;
  }

  private void OnCollisionEnter2D(Collision2D col) {
    if (col.gameObject.tag == "Ground") {      
      Debug.Log(this.oldVelocityX);
      this.rigid2d.velocity = new Vector2(this.oldVelocityX * -1.0f, this.rigid2d.velocity.y);
      Debug.Log(this.rigid2d.velocity.x);
    }
  }
}
