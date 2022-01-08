using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    protected Transform _XForm_Camera;
    protected Transform _XForm_Parent;

    GameObject player;

    protected Vector3 _LocalRotation;
    protected float _CameraDistance = 11f;

    public float MouseSensitivity = 4f;
    public float ScrollSensetivity = 2f;
    public float OrbitDampening = 10f;
    public float ScrollDampening = 6f;

    public bool CameraDisabled;

    // Режимы для камеры

    public float cameraDistanceLowerBoundary = 4;
    public float cameraDistanceUpperBoundary = 16f;

    //ForAndroid
    [SerializeField] private FixedJoystick joystick;

    public void StandartMode()
    {
        _CameraDistance = 11f;
        cameraDistanceLowerBoundary = 4;
        cameraDistanceUpperBoundary = 16f;
    }

    public void TutorialMode()
    {
        _CameraDistance = 6f;
        cameraDistanceLowerBoundary = 5f;
        cameraDistanceUpperBoundary = 8f;
    }

    void Start()
    {
        CameraDisabled = true;
        this._XForm_Camera = this.transform;
        this._XForm_Parent = this.transform.parent;

        // Куда повернута камера в начале
        _LocalRotation = new Vector3(0f, 30f, 0f);
        this._XForm_Parent.rotation = Quaternion.Euler(_LocalRotation.y, _LocalRotation.x, _LocalRotation.z);

        player = GameObject.Find("Player");
    }

    void LateUpdate()
    {
        //Отключение камеры
        /*if (Input.GetKeyDown(KeyCode.Mouse1))
            CameraDisabled = false;
        if (Input.GetKeyUp(KeyCode.Mouse1))
            CameraDisabled = true;

        if (!CameraDisabled)
        {
            // Вращение камеры на основе координат мыши
            if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
            {
                _LocalRotation.x += Input.GetAxis("Mouse X") * MouseSensitivity;
                _LocalRotation.y -= Input.GetAxis("Mouse Y") * MouseSensitivity;

                // Установка границ вращения
                _LocalRotation.y = Mathf.Clamp(_LocalRotation.y, 6f, 90f);
            }
        }*/

        if (joystick.Horizontal != 0 || joystick.Vertical != 0)
            CameraDisabled = false;
        if (Input.GetKeyUp(KeyCode.Mouse1))
            CameraDisabled = true;

        if (!CameraDisabled)
        {
            if (joystick.Horizontal != 0 || joystick.Vertical != 0)
            {
                _LocalRotation.x -= joystick.Horizontal * 0.5f * MouseSensitivity;
                _LocalRotation.y -= joystick.Vertical * 0.5f * MouseSensitivity;

                _LocalRotation.y = Mathf.Clamp(_LocalRotation.y, 6f, 90f);
            }
        }

        // Зум
        if (Input.GetAxis("Mouse ScrollWheel") != 0f)
        {
            float ScrollAmount = Input.GetAxis("Mouse ScrollWheel") * ScrollSensetivity;

            // Позволяет камере удаляться быстрей, по мере удаления от цели
            ScrollAmount *= (this._CameraDistance * 0.3f);

            this._CameraDistance += ScrollAmount * -1f;

            // Установка границ приближения-удаления камеры
            this._CameraDistance = Mathf.Clamp(this._CameraDistance, cameraDistanceLowerBoundary, cameraDistanceUpperBoundary);
        }

        Quaternion QT = Quaternion.Euler(_LocalRotation.y, _LocalRotation.x, _LocalRotation.z);
        this._XForm_Parent.rotation = Quaternion.Lerp(this._XForm_Parent.rotation, QT, Time.deltaTime * OrbitDampening);

        _XForm_Parent.transform.position = player.transform.position;

        if (this._XForm_Camera.localPosition.z != this._CameraDistance * -1f)
        {
            this._XForm_Camera.localPosition = new Vector3(0f, 0f, Mathf.Lerp(this._XForm_Camera.localPosition.z, this._CameraDistance * -1f, Time.deltaTime * ScrollDampening));
        }
    }
}