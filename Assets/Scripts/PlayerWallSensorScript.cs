using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallSensorScript : MonoBehaviour {

  PlayerScript playerScript;

  void Start() {
    this.playerScript = PlayerScript.Instance;
  }

  void Update() {

  }

  private void OnTriggerEnter2D(Collider2D col) {
    if (col.gameObject.tag == "Ground") {
      //this.playerScript.rigid2d.velocity = new Vector2(this.playerScript.rigid2d.velocity.x * -1.0f, this.playerScript.rigid2d.velocity.y);
    }
  }
}
