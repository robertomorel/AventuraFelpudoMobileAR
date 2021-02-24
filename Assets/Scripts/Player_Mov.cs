using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.CrossPlatformInput;

public class Player_Mov : MonoBehaviour
{

    private CharacterController _characterController;

    //[SerializeField]
    private float _spin, _speed, _jump, _forward;

    private Vector3 _vectorDirection;

    private Vector3 _moveCameraForward, _moveMove, normalZeroGround;

    private Transform _transformCamera;

    [SerializeField]
    private GameObject _player;

    [SerializeField]
    private Animation _playerAnimation;

    [SerializeField]
    private GameObject _particulaOvo, _particulaPena, _particulaEstrela, _objetoParticulaFogo;

    private float _contaPisca = 0;

    private bool _canTakeStar = false;

    private int _sceneObjectsCount = 0;

    [SerializeField]
    private AudioClip _somOvo, _somPena, _somEstrela, _somHit, _somWin, _somLose, _somApareceStar, _somFelpudoVoa;

    // Use this for initialization
    void Start()
    {

        _characterController = GetComponent<CharacterController>();
        _playerAnimation = _player.GetComponent<Animation>();

        _spin = 100.0f;
        _speed = 5.0f;
        _jump = 5.0f;
        _forward = 3.0f;

        normalZeroGround = new Vector3(0, 0, 0);

        _transformCamera = Camera.main.transform;

    }

    // Update is called once per frame
    void Update()
    {

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

        // -- Time.deltaTime é utilizado basicamente para normalizar o movimento

        // -- Input.GetAxis é um float que vai de 0 - 1
        // -- Horizontal: cima, baixo; Vertical: frente, trás

        _moveCameraForward = Vector3.Scale(transform.forward, new Vector3(1, 0, 1)).normalized;

        // -- Indicar quanto o player irá se movimentar para frente ou para trás
        _moveMove = (inputVertical * _moveCameraForward) + (inputHorizontal * _transformCamera.right);

        // -- Empurrar o player para baixo
        _vectorDirection.y -= 3.0f * Time.deltaTime;

        // -- Executar o movimento
        _characterController.Move(_vectorDirection * Time.deltaTime);
        _characterController.Move(_moveMove * _speed * Time.deltaTime);

        if (_moveMove.magnitude > 1.0f)
        {
            _moveMove.Normalize();
        }

        _moveMove = transform.InverseTransformDirection(_moveMove);

        _moveMove = Vector3.ProjectOnPlane(_moveMove, normalZeroGround);
        _spin = Mathf.Atan2(_moveMove.x, _moveMove.z);
        _forward = _moveMove.z;

        // -- Empurra o player para baixo para representar gravidade
        _characterController.SimpleMove(Physics.gravity);
        applyRotation();

        // -- "Jump" por padrão da Unity é a tecla espaço
        if (buttonJump)
        {
            // -- Se o player estiver no chão...
            if (_characterController.isGrounded == true)
            {
                // -- Calculo inicial do pulo
                _vectorDirection.y = _jump;
                _player.GetComponent<Animation>().Play("JUMP");
                //_playerAnimation.Play("JUMP");
                GetComponent<AudioSource>().PlayOneShot(_somFelpudoVoa, 0.7f);
            }
        }
        else
        {

            bool condition;

#if UNITY_ANDROID
            condition = (inputHorizontal != 0.0f) || (inputVertical != 0.0f);
#elif UNITY_IOS
            Debug.log("It´s running in IOS platform");
#else
            condition = Input.GetButton("Horizontal") || Input.GetButton("Vertical");
#endif

            if (condition)
            {
                if (!_playerAnimation.IsPlaying("JUMP"))
                {
                    _player.GetComponent<Animation>().Play("WALK");
                    //_playerAnimation.Play("WALK");
                }
            }
            else
            {
                if (_characterController.isGrounded == true)
                {
                    _player.GetComponent<Animation>().Play("IDLE");
                    //_playerAnimation.Play("IDLE");
                }
            }

        }
    }

    void applyRotation()
    {
        float turnSpeed = Mathf.Lerp(180, 360, _forward);
        transform.Rotate(0, _spin * turnSpeed * Time.deltaTime, 0);
    }

    private void OnTriggerEnter(Collider other)
    {

        string tag = other.gameObject.tag;
        Vector3 position = other.gameObject.transform.position;
        bool eraseGameObject = true;

        Debug.Log("Tag touched: " + tag);

        if (tag == "Egg")
        {
            Instantiate(_particulaOvo, position, Quaternion.identity);
            _sceneObjectsCount++;
            checkPickedObjects();
            GetComponent<AudioSource>().PlayOneShot(_somOvo, 0.7f);
        }
        else if (tag == "Feather")
        {
            _sceneObjectsCount++;
            checkPickedObjects();
            Instantiate(_particulaPena, position, Quaternion.identity);
            GetComponent<AudioSource>().PlayOneShot(_somPena, 0.7f);
        }
        else if (tag == "Star")
        {
            if (_canTakeStar)
            {
                Instantiate(_particulaEstrela, position, Quaternion.identity);
                GetComponent<AudioSource>().PlayOneShot(_somEstrela, 0.7f);
                Invoke("reloadScene", 3.0f);
                GetComponent<AudioSource>().PlayOneShot(_somWin, 1.0f);
            }
        }
        else if (tag == "Fire")
        {
            /*
             public void InvokeRepeating(string methodName, float time, float repeatRate);
             Invokes the method methodName in time seconds, then repeatedly every repeatRate seconds.
             Irá chamar o método para mudar o estado do Felpudo a partir de 0s e irá se repetir a cada 0.1s
             */
            InvokeRepeating("FelpudoChangeState", 0, 0.1f);
            // -- Volta o player 3.0f paa trás
            _characterController.Move(transform.TransformDirection(Vector3.back) * 3);
            GetComponent<AudioSource>().PlayOneShot(_somHit, 0.7f);
            eraseGameObject = false;
        }
        else if (tag == "Hole")
        {
            Invoke("reloadScene", 1.5f);
            GetComponent<AudioSource>().PlayOneShot(_somLose, 0.7f);
        }
        else
        {
            Debug.Log("Touched in an unknown object");
        }

        if (eraseGameObject)
        {
            other.gameObject.SetActive(false);
        }

    }

    void FelpudoChangeState()
    {
        _contaPisca++;
        /*
         Lógica para fazer o player piscar após ser atingido.
         Vai apagando e mostrando repetidamente até 7 vezes
         */
        _player.SetActive(!_player.activeInHierarchy);
        if (_contaPisca > 7)
        {
            _contaPisca = 0;
            _player.SetActive(true);
            CancelInvoke();
        }

    }

    void checkPickedObjects()
    {
        if (_sceneObjectsCount >= 22)
        {
            Debug.Log("You can now destroy the fire!");
            _canTakeStar = true;
            Destroy(_objetoParticulaFogo);
            GetComponent<AudioSource>().PlayOneShot(_somApareceStar, 0.7f);
        }
    }

    void reloadScene()
    {
        SceneManager.LoadScene("CenaFelpudo");
    }

}