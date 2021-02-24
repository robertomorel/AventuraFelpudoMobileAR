using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class acaoMoveJogador : MonoBehaviour
{

    private float velocidade = 1.0f;
    private float giro = 180.0f;
    private float gravidade = 3.5f;
    private float pulo = 6.0f;
    private CharacterController objetoCharControler;
    private Vector3 vetorDirecao = new Vector3(0, 0, 0);

    public GameObject jogador;
    public Animation animacao;

    void Start()
    {
        objetoCharControler = GetComponent<CharacterController>();
        animacao = jogador.GetComponent<Animation>();
    }

    void Update()
    {

        if (Input.GetKey(KeyCode.LeftShift))
        {
            velocidade = 2.5f;
        }
        else
        {
            velocidade = 5;
        }

        float inputVertical, inputHorizontal;
        bool buttonJump;

#if UNITY_ANDROID
        inputVertical = CrossPlatformInputManager.GetAxis("Vertical");
        inputHorizontal = CrossPlatformInputManager.GetAxis("Horizontal");
        buttonJump = CrossPlatformInputManager.GetButton("Jump");
#elif UNITY_IOS
        Debug.log("It´s running in IOS platform");
#else
        inputVertical = Input.GetAxis("Vertical");
        inputHorizontal = Input.GetAxis("Horizontal");
        buttonJump = Input.GetButton("Jump");
#endif


        Vector3 forward = inputVertical * transform.TransformDirection(Vector3.forward) * velocidade;
        transform.Rotate(new Vector3(0, inputHorizontal * giro * Time.deltaTime, 0));

        objetoCharControler.Move(forward * Time.deltaTime);
        objetoCharControler.SimpleMove(Physics.gravity);

        if (buttonJump)
        {
            if (objetoCharControler.isGrounded == true)
            {
                vetorDirecao.y = pulo;
            }
        }

        if (buttonJump)
        {
            if (objetoCharControler.isGrounded == true)
            {
                vetorDirecao.y = pulo;
                jogador.GetComponent<Animation>().Play("JUMP");
            }
        }
        else
        {
            if (Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
            {
                if (!animacao.IsPlaying("JUMP"))
                {
                    jogador.GetComponent<Animation>().Play("WALK");
                }



            }
            else
            {
                if (objetoCharControler.isGrounded == true)
                {
                    jogador.GetComponent<Animation>().Play("IDLE");
                }
            }
        }

        vetorDirecao.y -= gravidade * Time.deltaTime;
        objetoCharControler.Move(vetorDirecao * Time.deltaTime);
    }
}