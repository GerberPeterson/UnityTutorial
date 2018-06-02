using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour {

    // Adjustable parameters
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float bulletSpeed;
    public float playerRunSpeed;
    public float playerStrafeSpeed;
    public float playerTurnSpeed;

    // Camera parameters
    Vector3 cameraPosition = new Vector3(2.5f, 1.5f, -3f);
    Vector3 cameraRotation = new Vector3(10f, 0f, 0f);
    float cameraFOV = 90f;


    public override void OnStartLocalPlayer()
    {
        GetComponent<MeshRenderer>().material.color = Color.blue;

        // Set up the camera on spawn
        if (isLocalPlayer)
        {
            Camera cameraObject = FindObjectOfType<Camera>();
            if (cameraObject != null)
            {
                SetUpCamera(cameraObject);
            }
            else
            {
                Debug.Log("Couldn't find MainCamera in scene.");
            }
        }
    }

    private void SetUpCamera(Camera cameraObject)
    {
        cameraObject.transform.parent = gameObject.transform;
        cameraObject.transform.localPosition = cameraPosition;
        cameraObject.transform.localRotation = Quaternion.Euler(cameraRotation);
        cameraObject.fieldOfView = cameraFOV;
    }

    // Update is called once per frame
    void Update ()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            CmdFire();
        }

        var turn = Input.GetAxis("Turn") * Time.deltaTime * playerTurnSpeed;
        var run = Input.GetAxis("Vertical") * Time.deltaTime * playerRunSpeed;
        var strafe = Input.GetAxis("Horizontal") * Time.deltaTime * playerStrafeSpeed;

        transform.Rotate(0, turn, 0);
        transform.Translate(strafe, 0, run);
	}

    [Command]
    void CmdFire()
    {
        // Create the Bullet from the Bullet Prefab
        var bullet = (GameObject)Instantiate(
            bulletPrefab,
            bulletSpawn.position,
            bulletSpawn.rotation);

        // Add velocity to the bullet
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * bulletSpeed;

        // Spawn the bullet on the Clients
        NetworkServer.Spawn(bullet);

        // Destroy the bullet after 2 seconds
        Destroy(bullet, 2.0f);
    }
}
