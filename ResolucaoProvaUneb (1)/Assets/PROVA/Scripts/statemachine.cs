using System.Collections;
using UnityEngine;
using TMPro;

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

    bool isWalking = false;

    public enum estado_Esquilo{
        paradinho, andaninho, correndinho, agacharparado, agacharandando, correndode4, pulandinho, ataquebaixo, ataquecima, ataquelado, escalandinho, deslizandinho, deslizandonaparede, rolandinho
    }

    SpriteRenderer spr;
    public estado_Esquilo monitora = estado_Esquilo.paradinho;

    void Start(){
        spr = GetComponent<SpriteRenderer>();
        Player = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update(){
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        if (isGround){            
            if (Input.GetButtonDown("Jump") /*|| Input.GetButtonDown("JoystickButton0")*/ && !pulando){
                Jump();
                isCrouching = false;       
            } else if(Input.GetKeyDown(KeyCode.R /*|| (Input.GetAxis("RightTrigger") > 0 && Input.GetAxis("Vertical") < 0)*/)){
                StartCoroutine(ReproduzirAnimacao(estado_Esquilo.rolandinho, estado_Esquilo.paradinho));
                isCrouching = false;
            } else {
                if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow) || verticalInput < 0){
                    isCrouching = true;
                    if (Mathf.Abs(horizontalInput) > 0.1f){
                        CrouchingAndWalking(horizontalInput);
                    } else{
                        monitora = estado_Esquilo.agacharparado;
                    }
                } else {
                    isCrouching = false;
                    if (Mathf.Abs(horizontalInput) > 0.1f){
                        Walk(horizontalInput);                        
                    } else {
                        isWalking = false;
                        switch(monitora){
                            case estado_Esquilo.rolandinho:
                            case estado_Esquilo.ataquecima:
                                return;
                            case estado_Esquilo.ataquelado:
                                isWalking = true;
                                return;                                
                        }

                        monitora = estado_Esquilo.paradinho;

                    }
                }
            }

            if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && (Input.GetAxis("Horizontal") > 0 || Input.GetAxis("Horizontal") < 0)){
                if(currentAnimationCoroutine == null){
                    StartCoroutine(ReproduzirAnimacao(estado_Esquilo.correndinho, estado_Esquilo.andaninho));
                } else if (!pulando && !isCrouching && currentAnimationCoroutine == null){
                    monitora = estado_Esquilo.andaninho;
                }            
            }

        } else if(monitora != estado_Esquilo.ataquebaixo){
            monitora = estado_Esquilo.pulandinho;
        }        
        
        if (Input.GetKeyDown(KeyCode.J) /*|| Input.GetButtonDown("ButtonX")*/ && pulando && isWalking == false){
            Debug.Log("atacoubaixo");
            isCrouching = false;            
            StartCoroutine(ReproduzirAnimacao(estado_Esquilo.ataquebaixo, estado_Esquilo.pulandinho));
        } else if(Input.GetKeyDown(KeyCode.J) /*|| Input.GetButtonDown("ButtonX")*/ && !pulando && isWalking == false){
            Debug.Log("atacoucima");
            isCrouching = false;
            StartCoroutine(ReproduzirAnimacao(estado_Esquilo.ataquecima, estado_Esquilo.pulandinho));
        } else if(Input.GetKeyDown(KeyCode.J) /*|| Input.GetButtonDown("ButtonX")*/ && isWalking){
            Debug.Log("atacoulado");
            isCrouching = false;
            StartCoroutine(ReproduzirAnimacao(estado_Esquilo.ataquelado, estado_Esquilo.andaninho));
        }

        playerStateTEXT.text = monitora.ToString();
        UpdateAnimationState();
    }

    IEnumerator ReproduzirAnimacao(estado_Esquilo estadoInicial, estado_Esquilo estadoFinal){
    monitora = estadoInicial;
    UpdateAnimationState();
    yield return new WaitForSeconds(anim.GetCurrentAnimatorClipInfo(0).Length);
    monitora = estadoFinal;
    UpdateAnimationState();
    currentAnimationCoroutine = null;
    }

    void UpdateAnimationState(){
        if (isCrouching){
            anim.Play(monitora == estado_Esquilo.agacharandando ? "agacharandando" : "agacharparado");  
        } else {
            anim.Play(monitora.ToString());
        }
    }

    void Walk(float horizontalInput){        
        if (horizontalInput < 0f){
            spr.flipX = false;
            transform.position -= new Vector3(1 * velocityPlayer * Time.deltaTime, 0);
        } else if (horizontalInput > 0f){
            spr.flipX = true;
            transform.position += new Vector3(1 * velocityPlayer * Time.deltaTime, 0);
        }

        isWalking = true;

        if(monitora == estado_Esquilo.ataquelado) return;        
        monitora = estado_Esquilo.andaninho; 

        if (!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift)){
        monitora = estado_Esquilo.andaninho;
        }       
    }

    void Jump(){
        Player.velocity = new Vector2(Player.velocity.x, jumpPlayer);
        isGround = false;
        pulando = true;
    }

    void OnCollisionEnter2D(Collision2D collision){
        if (collision.gameObject.tag == "chao"){
            isGround = true;
            pulando = false;
        }
    }

    void CrouchingAndWalking(float horizontalInput){
        if (horizontalInput < 0f){
            transform.localScale = new Vector3(1, 1);
            transform.position -= new Vector3(1 * velocityPlayer * Time.deltaTime, 0);
        } else if (horizontalInput > 0f){
            transform.localScale = new Vector3(-1, 1);
            transform.position += new Vector3(1 * velocityPlayer * Time.deltaTime, 0);
        }
        
        monitora = estado_Esquilo.agacharandando;
    }
}