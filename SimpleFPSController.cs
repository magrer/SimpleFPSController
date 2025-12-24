using UnityEngine;

public class SimpleFPSController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float jumpForce = 5f;
    public float gravity = -9.81f;

    [Header("Mouse Look Settings")]
    public float mouseSensitivity = 2f;
    public float maxLookAngle = 80f; // Чтобы не перевернуться вверх ногами

    private CharacterController characterController;
    private Camera playerCamera;
    private Vector3 playerVelocity;
    private bool isGrounded;
    private float xRotation = 0f; // Поворот камеры по вертикали

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();

        // Закрепляем и скрываем курсор
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleMouseLook();
        HandleMovement();
    }

    void HandleMouseLook()
    {
        // Ввод мыши
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Поворачиваем тело игрока по горизонтали (влево-вправо)
        transform.Rotate(Vector3.up * mouseX);

        // Поворачиваем камеру по вертикали (вверх-вниз)
        xRotation -= mouseY; // Вычитаем, чтобы инвертировать ось (как в стандартных шутерах)
        xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle); // Ограничиваем угол
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    void HandleMovement()
    {
        // Проверка на земле ли персонаж
        isGrounded = characterController.isGrounded;
        if (isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f; // Небольшая сила прижимающая к земле
        }

        // Получаем ввод с клавиатуры (WASD)
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 moveDirection = (transform.right * horizontal + transform.forward * vertical).normalized;

        // Выбираем скорость (бег или ходьба)
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;

        // Двигаем CharacterController
        characterController.Move(moveDirection * currentSpeed * Time.deltaTime);

        // Прыжок
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            playerVelocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }

        // Применяем гравитацию
        playerVelocity.y += gravity * Time.deltaTime;
        characterController.Move(playerVelocity * Time.deltaTime);
    }
}