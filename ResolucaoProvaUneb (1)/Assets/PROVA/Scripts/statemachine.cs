using UnityEngine;
using System.Collections;
using TMPro; 

//Script de máquina de estados finitos e controle de player
//By Luna Leal

public class statemachine : MonoBehaviour
{
    public TextMeshProUGUI playerStateTEXT;
    float velocityPlayer = 1.0f;
    public float jumpPlayer;
    Rigidbody2D Player;
    bool isGround = false;
    public bool pulando;
    public Animator anim;
    public bool isCrouching;
    IEnumerator currentAnimationCoroutine;
    public float lateralJumpForce = 2f;
    bool isWalking = false;
    bool isClimbing = false;
    public float climbSpeed = 5.0f;
    public bool des = false;
    public bool par;
    public float maxAngle = 45f;
    SpriteRenderer spr;
    public enum estado_Esquilo{
        paradinho, andaninho, correndinho, agacharparado, agacharandando, correndode4, pulandinho, ataquebaixo, ataquecima, ataquelado, escalandinho, deslizandinho, deslizandonaparede, rolandinho
    }
    public estado_Esquilo monitora;

    void Start(){
        spr = GetComponent<SpriteRenderer>();
        Player = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update(){
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Jump");

        if (isGround){
            if (Input.GetButtonDown("Jump") && !pulando){
                Jump(horizontalInput);
                isCrouching = false;
                isClimbing = false;
            }
            if (Input.GetKeyDown(KeyCode.R)){
                isWalking = false;
                float seila = Input.GetAxis("Horizontal");                
                velocityPlayer = 7.0f;
                if (monitora == estado_Esquilo.rolandinho){
                if (seila < 0f){
                spr.flipX = false;
                transform.position -= new Vector3(1 * velocityPlayer * Time.deltaTime, 0);
                } else if (seila > 0f){
                spr.flipX = true;
                transform.position += new Vector3(1 * velocityPlayer * Time.deltaTime, 0);
            }

            StartCoroutine(ReproduzirAnimacao(estado_Esquilo.rolandinho, estado_Esquilo.paradinho));
        } 

        } else if (Input.GetKeyUp(KeyCode.R)){
            if (monitora == estado_Esquilo.rolandinho && currentAnimationCoroutine == null){
            
            isCrouching = false;
        }
            } else  {
                if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow) || verticalInput < 0){
                    isCrouching = true;
                    if (Mathf.Abs(horizontalInput) > 0.1f){
                        CrouchingAndWalking(horizontalInput);
                    } else {
                        monitora = estado_Esquilo.agacharparado;
                    }
                } else {
                    isCrouching = false;
                    if (Mathf.Abs(horizontalInput) > 0.1f){
                        Walk(horizontalInput);
                    } else {
                        isWalking = false;
                        switch (monitora){
                            case estado_Esquilo.rolandinho:
                            case estado_Esquilo.ataquecima:
                                return;
                            case estado_Esquilo.ataquelado:
                                isWalking = true;
                                return;
                        }

                        if (!isClimbing){
                            if (currentAnimationCoroutine == null){
                                par = true;
                                if (monitora != estado_Esquilo.deslizandinho && monitora != estado_Esquilo.correndinho && monitora != estado_Esquilo.correndode4){
                                    monitora = estado_Esquilo.paradinho;
                                }
                            }
                        } else {
                            par = false;
                        }
                    }
                }
            }

            if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && (Mathf.Abs(horizontalInput) > 0.1f)){
                isWalking = false;
                velocityPlayer = 5.0f;
                if (currentAnimationCoroutine == null){
                    StartCoroutine(ReproduzirAnimacao(estado_Esquilo.correndinho, estado_Esquilo.andaninho));
                }
            }

            if (Input.GetKeyDown(KeyCode.Z)){
                if (!des){
                    par = false;
                    des = true;
                    isCrouching = false;
            if (currentAnimationCoroutine == null){
                StartCoroutine(ReproduzirAnimacao(estado_Esquilo.paradinho, estado_Esquilo.deslizandinho));
        }
    }
        } else if (Input.GetKeyUp(KeyCode.Z)){
            if (des){
                par = true;
                des = false;
                isCrouching = false;
            if (currentAnimationCoroutine == null){
                StartCoroutine(ReproduzirAnimacao(estado_Esquilo.deslizandinho, estado_Esquilo.paradinho));
            }
        }
    }

    if (Input.GetKeyDown(KeyCode.G)){
    isWalking = false;
    float mds = Input.GetAxis("Horizontal");
    monitora = estado_Esquilo.correndode4;
    velocityPlayer = 7.0f;
    if (monitora == estado_Esquilo.correndode4){
        if (mds < 0f){
            spr.flipX = false;
            transform.position -= new Vector3(1 * velocityPlayer * Time.deltaTime, 0);
        }
        else if (mds > 0f){
            spr.flipX = true;
            transform.position += new Vector3(1 * velocityPlayer * Time.deltaTime, 0);
            }
        }
    } else if (Input.GetKeyUp(KeyCode.G)){
    if (monitora == estado_Esquilo.correndode4 && currentAnimationCoroutine == null){
        StartCoroutine(ReproduzirAnimacao(estado_Esquilo.correndode4, estado_Esquilo.paradinho));
        }

    }

        } else if (monitora != estado_Esquilo.ataquebaixo && !isClimbing){
            monitora = estado_Esquilo.pulandinho;
        }

            if (Input.GetKeyDown(KeyCode.J) && pulando && isWalking == false){
                isCrouching = false;
                StartCoroutine(ataques(estado_Esquilo.ataquebaixo, estado_Esquilo.pulandinho));
            } else if (Input.GetKeyDown(KeyCode.J) && !pulando && isWalking == false){
                isCrouching = false;
                StartCoroutine(ataques(estado_Esquilo.ataquecima, estado_Esquilo.pulandinho));
            } else if (Input.GetKeyDown(KeyCode.J) && isWalking){
            isCrouching = false;
            StartCoroutine(ataques(estado_Esquilo.ataquelado, estado_Esquilo.andaninho));
            }

        playerStateTEXT.text = monitora.ToString();
        UpdateAnimationState();
    }

    IEnumerator ReproduzirAnimacao(estado_Esquilo estadoInicial, estado_Esquilo estadoFinal){
        monitora = estadoInicial;
        UpdateAnimationState();

        float animDuration = anim.GetCurrentAnimatorClipInfo(0).Length;

        yield return new WaitForSeconds(0.1f);

        monitora = estadoFinal;
        UpdateAnimationState();
    }

    IEnumerator ataques(estado_Esquilo estadoInicial, estado_Esquilo estadoFinal){
        monitora = estadoInicial;
        UpdateAnimationState();

        float animDuration = anim.GetCurrentAnimatorClipInfo(0).Length;

        yield return new WaitForSeconds(animDuration);

        monitora = estadoFinal;
        UpdateAnimationState();
    }   

    void UpdateAnimationState(){
        if (isCrouching){
            anim.Play(monitora == estado_Esquilo.agacharandando ? "agacharandando" : "agacharparado");
        } else {
            switch (monitora){
                case estado_Esquilo.paradinho:
                    anim.Play("paradinho");
                    break;
                case estado_Esquilo.andaninho:
                    if (isWalking){
                        anim.Play("andaninho");
                        isWalking = true;
                    }
                    break;
                case estado_Esquilo.correndinho:
                    anim.Play("correndinho");
                    break;
                case estado_Esquilo.correndode4:
                    anim.Play("correndode4");
                    break;
                case estado_Esquilo.pulandinho:
                    anim.Play("pulandinho");
                    break;
                case estado_Esquilo.ataquebaixo:
                    anim.Play("ataquebaixo");
                    break;
                case estado_Esquilo.ataquecima:
                    anim.Play("ataquecima");
                    break;
                case estado_Esquilo.ataquelado:
                    anim.Play("ataquelado");
                    break;
                case estado_Esquilo.escalandinho:
                    anim.Play("escalandinho");
                    break;
                case estado_Esquilo.deslizandinho:
                    anim.Play("deslizandinho");
                    break;
                case estado_Esquilo.deslizandonaparede:
                    anim.Play("deslizandonaparede");
                    break;
                case estado_Esquilo.rolandinho:
                    anim.Play("rolandinho");
                    break;
            }
        }
    }

    void Walk(float horizontalInput){

    float currentVelocity = velocityPlayer;  

    if (monitora == estado_Esquilo.correndode4){
        currentVelocity = 7.0f;
    }
    
    if (horizontalInput < 0f){
        spr.flipX = false;
        transform.position -= new Vector3(1 * currentVelocity * Time.deltaTime, 0);
        isWalking = true;
        if (monitora != estado_Esquilo.correndode4) {
            monitora = estado_Esquilo.andaninho;
        }
    } else if (horizontalInput > 0f){
        spr.flipX = true;
        transform.position += new Vector3(1 * currentVelocity * Time.deltaTime, 0);
        isWalking = true;
        if (monitora != estado_Esquilo.correndode4) {
            monitora = estado_Esquilo.andaninho;
            }
        }
    }


    void Jump(float horizontalInput){
        float horizontalVelocity = Player.velocity.x;
        Player.velocity = new Vector2(horizontalVelocity, jumpPlayer);

        float lateralForce = horizontalInput * lateralJumpForce;
        Player.AddForce(new Vector2(lateralForce, 0), ForceMode2D.Impulse);

        isGround = false;
        isClimbing = false;
        pulando = true;
    }

   void OnCollisionEnter2D(Collision2D collision){
        if (collision.gameObject.tag == "chao"){
            isGround = true;
            pulando = false;
        }

        if (collision.gameObject.CompareTag("parede")){
            pulando = false;
            isClimbing = true;
        }

        if (collision.gameObject.CompareTag("chao2")){
        Player.bodyType = RigidbodyType2D.Static;
        }
    }

    void OnCollisionExit2D(Collision2D collision){
        if (collision.gameObject.CompareTag("chao")){
            isClimbing = false;
        }
        
    }

void OnCollisionStay2D(Collision2D collision){
    if (collision.gameObject.CompareTag("chao2")){
        Vector2 normal = collision.GetContact(0).normal;
        float angle = Vector2.Angle(Vector2.up, normal);

        if (angle <= maxAngle){
            Vector2 point = collision.GetContact(0).point;
            Vector2 playerPosition = transform.position;
            playerPosition.y = point.y + Mathf.Sin(angle * Mathf.Deg2Rad) * 0.5f; 
            transform.position = playerPosition;
        }
    }
}

    void FixedUpdate(){

        if (isClimbing){
            Player.bodyType = RigidbodyType2D.Kinematic;
            Player.velocity = Vector2.zero;
            float inferno = Input.GetKey(KeyCode.E) ? 1 : 0;

            if (inferno > 0){
                pulando = false;
                monitora = estado_Esquilo.escalandinho;
                isGround = false;
                float verticalVelocity = inferno * climbSpeed;
                Player.velocity = new Vector2(Player.velocity.x, verticalVelocity);
            } else if (inferno <= 0 && isClimbing){
                Player.bodyType = RigidbodyType2D.Dynamic;
                monitora = estado_Esquilo.deslizandonaparede;
            }
        } else {
            Player.bodyType = RigidbodyType2D.Dynamic;
        } if (!pulando){
            UpdateAnimationState();
        }
    }

    void CrouchingAndWalking(float horizontalInput){
        if (horizontalInput < 0f){
            spr.flipX = false;
            transform.position -= new Vector3(1 * velocityPlayer * Time.deltaTime, 0);
        } else if (horizontalInput > 0f){
            spr.flipX = true;
            transform.position += new Vector3(1 * velocityPlayer * Time.deltaTime, 0);
        }

        monitora = estado_Esquilo.agacharandando;
    }
}