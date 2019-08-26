using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerController : MonoBehaviour {
    
    public static PlayerController instance;
    public Vector2 speed;
    public Vector2 wallJumpSpeed;
    public float wallJumpTime;
    private float wallJumpTimer;
    public AnimationCurve wallJumpCurve;
    public float fallMultiplier, lowJumpMultiplier, fastFallMultiplier;

    public bool grounded, onWall, hasWallJump;
    public Transform groundCheck, wallCheck;
    public LayerMask groundLayer;
    private Rigidbody2D rb;

    public SpawnPoint currentSpawn;
    public ParticleSystem deathParticles;
    private SpriteRenderer sp;

    public CinemachineVirtualCamera activeCamera;

    public Script deathScript;

    public bool facingRight;

    public bool allowWallJump = true;
    public bool wallFaceRight = true;

    public float wallStickTimer, wallStickTime;

    public GameObject hitbox;

    void Awake() {
        rb = GetComponent<Rigidbody2D>();
        sp = GetComponent<SpriteRenderer>();
        instance = this;
        wallJumpTimer = wallJumpTime;
    }

    void Update() {
        float xVel = Input.GetAxis("Horizontal") * speed.x;
        float yVel = rb.velocity.y;

        grounded = Physics2D.OverlapCircle(groundCheck.position, .01f, groundLayer);
        if (!hasWallJump && grounded) hasWallJump = true;
        if (xVel != 0) {
            facingRight = xVel > 0;
            transform.localScale = facingRight ? new Vector3(1,1,1) : new Vector3(-1,1,1);
        }
        onWall = Physics2D.OverlapCircle(wallCheck.position, .01f, groundLayer);

        

        if (onWall && wallJumpTimer > 0.3f) {
            wallJumpTimer = wallJumpTime;
            wallStickTimer = wallStickTime;
            wallFaceRight = facingRight;
        }

        if (wallStickTimer > 0) {
            wallStickTimer -= Time.deltaTime;
            xVel = 0f;
        }

        if (grounded && Input.GetAxisRaw("Vertical") > 0) {
            yVel = speed.y;
        } else if (allowWallJump && hasWallJump && (onWall || wallStickTimer > 0) && Input.GetButtonDown("Jump")) {
            //hasWallJump = false;
            onWall = false;
            wallStickTimer = 0;
            yVel = wallJumpSpeed.y;
            wallJumpTimer = 0;
        }

        if (wallJumpTimer < wallJumpTime) {
            wallJumpTimer += Time.deltaTime;
            float newVelX = wallJumpSpeed.x * (!wallFaceRight ? 1f : -1f);
            float i = wallJumpCurve.Evaluate(wallJumpTimer / wallJumpTime);
            xVel = i * newVelX + (1f - i) * xVel;
        }

        if (yVel < 0) {
            if (Input.GetAxisRaw("Vertical") < 0) {
                yVel += Physics2D.gravity.y * (fastFallMultiplier - 1) * Time.deltaTime;
            } else {
                yVel += Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
            }
        } else if (yVel > 0 && Input.GetAxisRaw("Vertical") <= 0) {
            yVel += Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }

        rb.velocity = new Vector2(xVel, yVel);
    }

    public void SetSpawn(string spawnName) {
        currentSpawn = SpawnPoint.GetSpawnPoint(spawnName);
    }

    public void Respawn() {
        hitbox.SetActive(true);
        activeCamera.enabled = false;
        transform.position = currentSpawn.transform.position;
        rb.velocity = Vector2.zero;
        wallJumpTimer = wallJumpTime;
        wallStickTimer = 0;
        sp.enabled = true;
        StartCoroutine("CameraCut");
        // inactiveCamera.transform.position = currentSpawn.transform.position;
        // CinemachineVirtualCamera temp = activeCamera;
        // inactiveCamera.m_Priority = 5;
        // inactiveCamera.m_Follow = transform;
        // activeCamera.m_Priority = 0;
        // activeCamera = inactiveCamera; 
        // inactiveCamera = temp;
         
    }

    IEnumerator CameraCut() {
        yield return null;
        activeCamera.enabled = true;
    }

    public void Death() {
        if (hitbox.activeInHierarchy) {
            hitbox.SetActive(false);
            sp.enabled = false;
            deathParticles.transform.position = transform.position;
            deathParticles.Play();
            StartCoroutine("DeathCo");
        }
        
    }

    private IEnumerator DeathCo() {
        yield return new WaitForSecondsRealtime(0.2f);
        UIController.instance.LoadScript(deathScript);
    }

    
}
