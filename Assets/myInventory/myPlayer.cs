using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class myPlayer : MonoBehaviour
{
    private Vector3 Velocity;
    private bool grounded;
    private float Speed = 5.0f;
    private float jump = 1.0f;
    private float gravity = -9.81f;
    [SerializeField] float mouseSense;
    [SerializeField] Transform playerArms;
    private CharacterController controller;
    private GameObject takenItem;
    private Inventory inventory;
    public GameObject takeIcon;
    private bool fixLooking = false;

    public UnityEvent folding, removing;



    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        TakeItem();


        Movement();
        if(!fixLooking) MouseLook();
    }

    void TakeItem()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if(inventory)
            {
                if (takenItem)
                {
                    inventory.Put(takenItem.GetComponent<Item>());
                    takenItem = null;
                    takeIcon.SetActive(false);

                    return;
                }
                else
                {
                    Cursor.lockState = CursorLockMode.Locked;

                    fixLooking = false;
                }

                inventory = null;
            }
        }

        if (Input.GetMouseButton(0))
        {
            if (takenItem)
            {
                takenItem.GetComponent<Rigidbody>().isKinematic = true;
                takenItem.GetComponent<Collider>().enabled = false;
                takenItem.transform.parent = playerArms;
                takenItem.transform.position = playerArms.position + playerArms.forward * 2;

                Ray rayInventory = Camera.main.ScreenPointToRay(Input.mousePosition);
                Debug.DrawRay(rayInventory.origin, rayInventory.direction * 10, Color.green);
                RaycastHit hitInventory;

                if (Physics.Raycast(rayInventory, out hitInventory))
                {
                    if (hitInventory.transform.CompareTag("Inventory"))
                    {
                        if(!inventory) inventory = hitInventory.transform.GetComponent<Inventory>();

                        takeIcon.SetActive(true);
                        return;
                    }
                }
                inventory = null;
                takeIcon.SetActive(false);
                return;
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction * 10, Color.red);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.CompareTag("Item"))
                {
                    takenItem = hit.transform.gameObject;
                }
                else if (hit.transform.CompareTag("Inventory"))
                {
                    if (!inventory)
                    {
                        inventory = hit.transform.GetComponent<Inventory>();
                        inventory.UIpanel.SetActive(true);
                        Cursor.lockState = CursorLockMode.None;

                        fixLooking = true;
                    }
                }
            }
        }
        else if (takenItem)
        {
            takeIcon.SetActive(false);

            takenItem.transform.parent = null;
            takenItem.GetComponent<Rigidbody>().isKinematic = false;
            takenItem.GetComponent<Collider>().enabled = true;

            takenItem = null;
        }
    }

    void Movement() 
    {
        grounded = controller.isGrounded;
        if (grounded && Velocity.y < 0)
        {
            Velocity.y = 0f;
        }

        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        move = transform.TransformDirection(move);
        controller.Move(move * Time.deltaTime * Speed);

        if (Input.GetButtonDown("Jump") && grounded)
        {
            Velocity.y += Mathf.Sqrt(jump * -3.0f * gravity);
        }

        Velocity.y += gravity * Time.deltaTime;
        controller.Move(Velocity * Time.deltaTime);
    }
    void MouseLook()
    {
        float rotateX = Input.GetAxis("Mouse X") * mouseSense;
        float rotateY = Input.GetAxis("Mouse Y") * mouseSense;

        Vector3 rotPlayerArms = playerArms.rotation.eulerAngles;
        Vector3 rotPlayer = transform.rotation.eulerAngles;

        rotPlayerArms.x -= rotateY;
        rotPlayerArms.z = 0;
        rotPlayer.y += rotateX;

        playerArms.rotation = Quaternion.Euler(rotPlayerArms);
        transform.rotation = Quaternion.Euler(rotPlayer);
    }

    public IEnumerator POST()
    {
        var Data = new WWWForm();
        Data.AddField("BMeHG5xqJeB4qCjpuJCTQLsqNGaqkfB6", "Текст 1");
        
        var Query = new WWW("https://dev3r02.elysium.today/inventory/status", Data);
        yield return Query;
        if (Query.error != null)
        {
            Debug.Log("Server does not respond : " + Query.error);
        }
        else
        {
            if (Query.text == "response")
            {
                Debug.Log("Server responded correctly");
            }
            else
            {
                Debug.Log("Server responded : " + Query.text);
            }
        }
        Query.Dispose();
    }
}
