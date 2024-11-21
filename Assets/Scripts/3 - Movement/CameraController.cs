using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public float moveSpeed = 10.0f;
    public float maxMoveSpeed = 50.0f;
    public float rotationSpeed = 5.0f;

    public Terrain terrain;


    private void Start()
    {
        CenterCameraOnTerrain();
    }

    // Update is called once per frame
    void Update()
    {
        MoveIfClicking();
        HandleMovement();
        SetCameraSpeed();
        KeepCameraAboveTerrain();
        KeepCameraBelowCeiling();

    }

    private void MoveIfClicking()
    {
        // if right click, set speed/rotation
        if (Input.GetMouseButton(1))
        {
            float h = rotationSpeed * Input.GetAxis("Mouse X");
            float v = rotationSpeed * Input.GetAxis("Mouse Y");

            transform.Rotate(Vector3.up, h, Space.World);
            transform.Rotate(Vector3.right, -v);
        }

        // move
        float moveX = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        float moveZ = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;

        transform.Translate(moveX, 0, moveZ);
    }

    private void HandleMovement()
    {
        // if holding w
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime, Space.Self);
        }

        // if holding s
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(Vector3.back * moveSpeed * Time.deltaTime, Space.Self);
        }

        // if holding d
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(Vector3.right * moveSpeed * Time.deltaTime, Space.Self);
        }

        // if holding a
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(Vector3.left * moveSpeed * Time.deltaTime, Space.Self);
        }

        // if holding spacebar
        if (Input.GetKey(KeyCode.Space))
        {
            transform.Translate(Vector3.up * moveSpeed * Time.deltaTime, Space.Self);
        }

        // if holding shift
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            transform.Translate(Vector3.down * moveSpeed * Time.deltaTime, Space.Self);
        }
    }

    private void SetCameraSpeed()
    {
        // if scrolling
        if (Input.GetAxisRaw("Mouse ScrollWheel") > 0)
        {
            moveSpeed += 1;
            moveSpeed = Mathf.Clamp(moveSpeed, 5, maxMoveSpeed);

        }

        if (Input.GetAxisRaw("Mouse ScrollWheel") < 0)
        {
            moveSpeed -= 1;
            moveSpeed = Mathf.Clamp(moveSpeed, 5, maxMoveSpeed);

        }
    }

    private void KeepCameraAboveTerrain()
    {
        // Ensure Camera does not go below terrain
        Vector3 position = transform.position;
        position.y = Mathf.Max(position.y, 3);
        transform.position = position;
    }

    private void KeepCameraBelowCeiling()
    {
        Vector3 position = transform.position;
        position.y = Mathf.Min(position.y, 50);
        transform.position = position;
    }

    private void CenterCameraOnTerrain()
    {
        if (terrain != null)
        {
            float centerX = terrain.terrainData.size.x / 2;
            float centerZ = terrain.terrainData.size.z / 2;

            transform.position = new Vector3(centerX, 25, centerZ);
            transform.rotation = Quaternion.Euler(30f, 0f, 0f);

        } else
        {
            Debug.Log("Terrain not assigned");
        }
    }
}
