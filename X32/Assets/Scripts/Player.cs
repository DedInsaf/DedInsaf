using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//void - это ключевое слово, которое обозначает объявление ф-ии, которая ничего не возвращает
//Ф-ия - это часть кода, которая выполняет ф-ию
//Метод - это ф-ия, объявленная внутри класса

public class Player : Unit
{
    private int tryCount;

    [SerializeField]
    public int lives = 5;
    public int level = 1;
    int sceneIndex = 1;

    public Joystick joystick;

    public int Lives
    {
        get { return lives; }
        set
        {
            if (value < 5) lives = value;
            livesBar.Refresh();
        }
    }

    private LivesBar livesBar;

    [SerializeField] CameraController adTransition;

    [SerializeField] //Чтобы отображалось в Юнити
    private float speed = 3.0F;
    [SerializeField]
    private float jumpForce = 15F;

    private bool isGrounded = false;

    private Bullet bullet;

    private PlayerState State
    {
        get { return (PlayerState)animator.GetInteger("State"); }
        set { animator.SetInteger("State", (int) value); }
    }

    new private Rigidbody2D rigidbody;
    private Animator animator;
    private SpriteRenderer sprite;

    private void Awake()
    {
        tryCount = PlayerPrefs.GetInt("tryCount");
        livesBar = FindObjectOfType<LivesBar>();
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();

        bullet = Resources.Load<Bullet>("Bullet");
    }

    private void FixedUpdate()
    {
        CheckGround();
    }

    private void Update()
    {
        if (isGrounded) State = PlayerState.Idle;

        if (lives <= 0) 
        {
            DieP();
        }

        
        if (joystick.Horizontal != 0) Run();
        if (isGrounded && joystick.Vertical > 0.5F) Jump(); //Если в момент нажатия нажали, то вызвать метод Jump()
    }

    private void Run()
    {
        Vector3 direction = transform.right * joystick.Horizontal;

        transform.position = Vector3.MoveTowards(transform.position, transform.position + direction, speed * Time.deltaTime);

        sprite.flipX = direction.x < 0.0F;

        if (isGrounded) State = PlayerState.Run;
    }

    private void Jump()
    {
        rigidbody.velocity = Vector2.up * jumpForce;
    }

    public void Shoot()
    {
        Vector3 position = transform.position; position.y += 0.8F;
        Bullet newBullet = Instantiate(bullet, position, bullet.transform.rotation) as Bullet;

        newBullet.Parent = gameObject;
        newBullet.Direction = newBullet.transform.right * (sprite.flipX ? -1.0F : 1.0F);
    }

    public override void ReceiveDamage()
    {
        Lives--;

        rigidbody.velocity = Vector3.zero;
        rigidbody.AddForce(transform.up * 15.0F, ForceMode2D.Impulse);

        Debug.Log(lives);
    }

    private void CheckGround()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.3F);

        isGrounded = colliders.Length > 1;

        if (!isGrounded) State = PlayerState.Jump;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {

        Bullet bullet = collider.gameObject.GetComponent<Bullet>();
        if (bullet && bullet.Parent != gameObject)
        {
            ReceiveDamage();
        }
    }

    public void SavePlayer()
    {
        SaveSystem.SavePlayer(this);
    }
    
    public void LoadPlayer()
    {
        PlayerData data = SaveSystem.LoadPlayer();
        level = data.level;
        lives = data.lives;

        Vector3 position;
        position.x = data.position[0];
        position.y = data.position[1];
        position.z = data.position[2];
        transform.position = position;
    }

    public void Start()
    {
        sceneIndex = SceneManager.GetActiveScene().buildIndex;
    }

    public void DieP()
    {
        tryCount++;
        PlayerPrefs.SetInt("tryCount", tryCount);
        if (tryCount % 2 == 0)
        {
            adTransition.ShowAd();
        }
        SceneManager.LoadScene(sceneIndex);
    }
}

public enum PlayerState //enum - тип данных, в ктором задаем дискретно задаем опредлённые состояния
{
     Idle,      
     Run,
     Jump
}