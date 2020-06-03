using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : SingletonMonoBehaviourFast<PlayerScript> {
  Rigidbody2D rigid2d;
  Animator animator;
  public GameObject groundedCollider;
  public GameObject jumpCollider;
  //public GameObject boundAfterCollider;
  public float walkSpeedMax = 2.0f;
  public Vector2 velocityMin = new Vector2(-15.0f, -15.0f);
  public Vector2 velocityMax = new Vector2(+15.0f, +15.0f);
  public float walkSpeed = 3.0f;
  bool leftTap = false;
  bool rightTap = false;
  bool jumpTap = false;
  bool oldJumpTap = false;
  bool jumping = false;
  bool grounded_result = false;
  bool[] grounded = new bool[3] { false, false, false };
  public LayerMask groundLayer;
  public float jumpForceX = 5f;
  public float jumpForceY = 20.0f;
  float jumpTime = 0; //ジャンプ貯め時間を保存する変数
  float jumpDelay = 0.4f;
  float jumpDelayCount = 3.0f;
  Image hpBarImage;
  public LayerMask slopeLayer;
  bool staySlope = false;

  void Start() {
    this.rigid2d = GetComponent<Rigidbody2D>();
    this.animator = GetComponent<Animator>();
    GameObject hpBar = GameObject.Find("HPBar");
    this.hpBarImage = hpBar.GetComponent<Image>();
  }

  private void FixedUpdate() {
    //歩行処理
    float key = 0;
    if (this.leftTap) {
      key = -1.0f;
    }
    else if (this.rightTap) {
      key = 1.0f;
    }
    //進行方向に向かって画像を反転させたり
    if (key != 0) {
      transform.localScale = new Vector3(key, 1, 1);
    }
    //移動していたら歩きアニメーション
    if (Mathf.Abs(this.rigid2d.velocity.x) > 0.5f) {
      this.animator.SetBool("walk", true);
    }
    else {
      this.animator.SetBool("walk", false);
    }
    //地面に接地していたら歩行処理
    if ((this.grounded_result) && (this.jumpDelay < this.jumpDelayCount) && (key != 0)) {
      //歩行maxスピード以下なら速度を足
      if (Mathf.Abs(this.rigid2d.velocity.x) < this.walkSpeedMax) {
        this.rigid2d.AddForce(new Vector2(this.walkSpeed * key, 0));
      }
    }

    //ジャンプ処理
    if (this.jumping) {
      this.jumping = false;
      if (this.jumpTime > 1.0f) {
        this.jumpTime = 1.0f;
      }

      float jumpPower = 0.2f + JumpHeightCalculation(0.2f, 1.0f);

      this.rigid2d.velocity = new Vector2(this.jumpForceX * transform.localScale.x,
                                                           this.jumpForceY * jumpPower);
      this.jumpTime = 0;
      this.jumpDelayCount = 0;
    }

    //スピード制限
    float vx = Mathf.Clamp(this.rigid2d.velocity.x, this.velocityMin.x, this.velocityMax.x);
    float vy = Mathf.Clamp(this.rigid2d.velocity.y, this.velocityMin.y, this.velocityMax.y);
    //速度反映
    this.rigid2d.velocity = new Vector2(vx, vy);
  }

  void Update() {
    //ジャンプできるかどうかの接地判定
    Vector2 centerPosition;
    Vector2 linePos = transform.position;
    float myLocalScaleX = transform.localScale.x;
    centerPosition = new Vector2(transform.position.x + 0.0967f * myLocalScaleX, transform.position.y + 0.1938f);
    linePos = new Vector2(centerPosition.x + 0.09f * myLocalScaleX, centerPosition.y - 0.1f);
    grounded[0] = Physics2D.Linecast(centerPosition, linePos, groundLayer);
    Debug.DrawLine(centerPosition, linePos, Color.red);
    linePos = new Vector2(centerPosition.x, centerPosition.y - 0.1f);
    grounded[1] = Physics2D.Linecast(centerPosition, linePos, groundLayer);
    Vector2 slopeLine = new Vector2(centerPosition.x, centerPosition.y - 0.13f);//坂道に接しているかもついでに判定
    this.staySlope = Physics2D.Linecast(centerPosition, slopeLine, slopeLayer); //坂道に接しているかもついでに判定
    Debug.DrawLine(centerPosition, slopeLine, Color.blue);
    Debug.DrawLine(centerPosition, linePos, Color.red);
    linePos = new Vector2(centerPosition.x - 0.12f * myLocalScaleX, centerPosition.y - 0.12f);
    grounded[2] = Physics2D.Linecast(centerPosition, linePos, groundLayer);
    Debug.DrawLine(centerPosition, linePos, Color.red);

    //接地判定をして結果をgourended_result変数に入れる
    if ((grounded[0]) || (grounded[1]) || (grounded[2])) {
      grounded_result = true;
    }
    else {
      grounded_result = false;
      this.jumpTime = 0;
    }

    if (this.staySlope) {
      this.animator.SetBool("Slope", true);
    }
    else {
      this.animator.SetBool("Slope", false);
    }

    if (grounded_result) {
      this.groundedCollider.SetActive(true);
      this.jumpCollider.SetActive(false);
      this.animator.SetBool("Grounded", true);
    }
    else {
      this.groundedCollider.SetActive(false);
      this.jumpCollider.SetActive(true);
      this.animator.SetBool("Grounded", false);
    }

    //--------------キー入力--------------
    //左右キー
    if (!Input.GetKey(KeyCode.Space)) {
      if (Input.GetKey(KeyCode.LeftArrow)) {
        this.leftTap = true;
      }
      if (Input.GetKey(KeyCode.RightArrow)) {
        this.rightTap = true;
      }
    }
    else {
      this.leftTap = false;
      this.rightTap = false;
    }
    if (Input.GetKeyUp(KeyCode.LeftArrow)) {
      this.leftTap = false;
    }
    if (Input.GetKeyUp(KeyCode.RightArrow)) {
      this.rightTap = false;
    }
 
    //ジャンプキー
    if (!this.jumping && this.grounded_result) {
      if (Input.GetKeyDown(KeyCode.Space)) {
        this.jumpTap = true;
        this.animator.SetBool("JumpTame", true);
      }
    }
    if (Input.GetKeyUp(KeyCode.Space)) {
      this.jumpTap = false;
      this.animator.SetBool("JumpTame", false);
    }
    //--------------キー入力ここまで--------------

    //ジャンプキーが押されている時間をはかる
    if (this.jumpTap) {
      this.jumpTime += Time.deltaTime;
    }

    //前回のフレームでジャンプが押されていて今のフレームで離されていたらtrue OR ジャンプボタンが1秒以上押されたら
    if (((!this.jumpTap && this.oldJumpTap) || (this.jumpTime > 1.0f)) && this.grounded_result) {
      this.jumping = true;
      this.jumpTap = false;
      this.animator.SetBool("JumpTame", false);
      this.jumpDelayCount = 0;
    }
    //ジャンプを連続できないようにするディレイタイムを加算する
    if (this.jumpDelayCount < 2.0f) {
      this.jumpDelayCount += Time.deltaTime;
    }
    //空中だったらjumpDelayCountをjumpDelay以上に
    if (!this.grounded_result) {
      this.jumpDelayCount = 2.0f;
    }
    //Y速度をanimationのSpeedYに代入
    this.animator.SetFloat("SpeedY", this.rigid2d.velocity.y);
    //フレームの最後の処理
    this.oldJumpTap = this.jumpTap;
    this.hpBarImage.fillAmount = this.jumpTime / 1.0f; //貯めをゲージに反映
  }

  //ジャンプの高さを計算する関数
  private float JumpHeightCalculation(float minPower, float maxPower) {
    float nowTime = this.jumpTime / 1.0f;
    float powerWidth = maxPower - minPower;
    float powerResult = powerWidth * nowTime;
    return powerResult;
  }

  private void OnCollisionEnter2D(Collision2D col) {
    //動く床に乗った時動く要素を親オブジェクトにする
    if (col.gameObject.tag == "MoveFloor" || col.gameObject.name == "MoveFloor") {
      transform.SetParent(col.transform);
    }
  }

  private void OnTriggerExit2D(Collider2D col) {
    //動く床から離れたら床との親子関係を解消する
    if (col.gameObject.tag == "MoveFloor" || col.gameObject.name == "MoveFloor") {
      transform.SetParent(null);
      //親オブジェクトが回転していた場合自分の角度も狂っているので、０にする
      transform.rotation = Quaternion.Euler(0, 0, 0);
    }
  }
}
