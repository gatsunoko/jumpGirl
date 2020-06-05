using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameControllerScript : MonoBehaviour {

  GameObject player;

  private void Start() {
    this.player = PlayerScript.Instance.gameObject;
  }

  void Update() {
    if (Input.GetKeyDown(KeyCode.R)) {
      SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    if (Input.GetKeyDown(KeyCode.Q)) {
      if (PlayerPrefs.HasKey("ResbornX")) {
        this.player.transform.position = new Vector2(PlayerPrefs.GetFloat("ResbornX"),
                                                                           PlayerPrefs.GetFloat("ResbornY"));
      }
    }
  }
}
