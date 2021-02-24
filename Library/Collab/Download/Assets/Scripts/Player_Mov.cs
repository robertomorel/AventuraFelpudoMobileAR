using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Mov : MonoBehaviour
{

    private CharacterController _characterController;

    [SerializeField]
    private float _spin, _speed, _jump;

    private Vector3 _vectorDirection;

    [SerializeField]
    private GameObject _player;

    [SerializeField]
    private Animation _playerAnimation;

    // Use this for initialization
    void Start()
    {

        _characterController = GetComponent<CharacterController>();
        _playerAnimation = _player.GetComponent<Animation>();

        _spin = 300.0f;
        _speed = 5.0f;
        _jump = 5.0f;
        
    }

    // Update is called once per frame
    void Update()
    {

        // -- Time.deltaTime é utilizado basicamente para normalizar o movimento

        // -- Input.GetAxis é um float que vai de 0 - 1
        // -- Horizontal: cima, baixo; Vertical: frente, trás

        // -- Indicar quanto o player irá se movimentar para frente ou para trás
        Vector3 forward = Input.GetAxis("Vertical") * transform.TransformDirection(Vector3.forward) * _speed;

        // -- Indicar quanto o player irá rotacionar quando as teclas de direita ou esquerda forem clicadas
        transform.Rotate(new Vector3(0, Input.GetAxis("Horizontal") * _spin * Time.deltaTime));

        // -- Move player horizontalmente
        _characterController.Move(forward * Time.deltaTime);

        // -- Empurra o player para baixo para representar gravidade
        _characterController.SimpleMove(Physics.gravity);

        // -- "Jump" por padrão da Unity é a tecla espaço
        if (Input.GetButton("Jump"))
        {
            // -- Se o player estiver no chão...
            if (_characterController.isGrounded == true)
            {
                // -- Calculo inicial do pulo
                _vectorDirection.y = _jump;
                //_player.GetComponent<Animation>().Play("JUMP");
                _playerAnimation.Play("JUMP");
            }
        }
        else
        {
            if (Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
            {
                if (!_playerAnimation.IsPlaying("JUMP"))
                {
                    //_player.GetComponent<Animation>().Play("WALK");
                    _playerAnimation.Play("WALK");
                }
            }
            else
            {
                if (_characterController.isGrounded == true)
                {
                    //_player.GetComponent<Animation>().Play("IDLE");
                    _playerAnimation.Play("IDLE");
                }
            }

        }

        // -- Empurrar o player para baixo
        _vectorDirection.y -= 3.0f * Time.deltaTime;

        // -- Executar o movimento
        _characterController.Move(_vectorDirection * Time.deltaTime);

    }
}
