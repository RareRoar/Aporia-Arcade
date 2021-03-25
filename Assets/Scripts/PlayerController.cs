using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    protected Joystick joystick;
    protected Joybutton joybutton;
    public GameObject fade;
    public float speed;
    private Rigidbody rb_;
    private Vector3 jump_ = new Vector3(0.0f, 4.0f, 0.0f);
    private bool isGrounded_;
    private Vector3 movement_;
    public Vector3 gravity_ = new Vector3(0.0f, -1.0f, 0.0f);

    void Start()
    {
        joystick = FindObjectOfType<Joystick>();
        joybutton = FindObjectOfType<Joybutton>();
        rb_ = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        //float moveHorizontal = Input.GetAxis("Horizontal");
        //float moveVertical = Input.GetAxis("Vertical");
        float moveHorizontal = joystick.Horizontal;
        float moveVertical = joystick.Vertical;
        movement_ = new Vector3(moveHorizontal, 0.0f, moveVertical);
        rb_.AddForce(movement_ * speed);
    }

    void Update()
    {
        if (joybutton.Pressed && isGrounded_)
        {
            rb_.AddForce(jump_, ForceMode.Impulse);
            rb_.AddForce(gravity_, ForceMode.Acceleration);
            isGrounded_ = false;
        }
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded_)
        {
            rb_.AddForce(jump_, ForceMode.Impulse);
            rb_.AddForce(gravity_, ForceMode.Acceleration);
            isGrounded_ = false;
        }
    }

    void OnCollisionStay()
    {
        isGrounded_ = true;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Killbox"))
        {
            fade.SetActive(true);
            CommandInvoker invoker = new CommandInvoker();
            invoker.SetCommand(new WriteRowCommand());
            invoker.RunAll();
            Invoke("Redirect", 2.0f);
        }
        if (!collision.gameObject.CompareTag("Ground"))
        {
            rb_.AddForce(movement_ * (-1), ForceMode.Impulse);

            if (collision.gameObject.CompareTag("Wall"))
            {
                collision.gameObject.SetActive(false);
            }
        }
    }

    void Redirect()
    {
        SceneManager.LoadScene("MainMenu");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Pick Up"))
        {
            other.gameObject.SetActive(false);
            MetaSceneInformation.CollectedSeeds++;
            if (MetaSceneInformation.CollectedSeeds == MetaSceneInformation.SeedCount)
            {
                MetaSceneInformation.SetCollected();
                MetaSceneInformation.Level++;
                /*CommandInvoker invoker = new CommandInvoker();
                invoker.SetCommand(new WriteRowCommand());
                invoker.RunAll();*/
                Invoke("Restart", 1.0f);
            }
        }
        if (other.gameObject.CompareTag("Killbox"))
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }
        if (other.gameObject.CompareTag("Respawn"))
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
    private void Restart()
    {
        Debug.Log(SceneManager.GetActiveScene().name);
        Debug.Log(SceneManager.GetActiveScene().buildIndex);
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
        SceneManager.LoadScene("SampleScene");
    }
}
