using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine.Events;

public class Inventory : MonoBehaviour
{
    public GraphicRaycaster m_Raycaster;
    PointerEventData m_PointerEventData;
    public EventSystem m_EventSystem;


    public List<Item> list;

    public Transform[] itemTypesPoints;
    public GameObject UIpanel;
    public Transform blowoutPoint;

    public UnityEvent folding, removing;


    void Start()
    {
        list = new List<Item>();
    }

    private void UpdatePanel()
    {
        for(int i = 0; i < UIpanel.transform.childCount; i++)
        {
            Destroy(UIpanel.transform.GetChild(i).gameObject);
        }

        foreach (var cur in list)
        {
            if (cur)
            {
                GameObject item = Instantiate(cur.UIobject);
                item.transform.parent = UIpanel.transform;
                item.transform.name = cur.name;
                item.SetActive(true);
            }
        }
    }

    private void Blowout(string name)
    {
        int index = list.FindIndex(i => i.name == name);

        StartCoroutine(POST(list[index].id, "removing"));
        removing.Invoke();

        StartCoroutine(BlowoutMove(list[index].transform, blowoutPoint));

        list.RemoveAt(index);
        //list.RemoveAll(i => i.name == name);
        UpdatePanel();
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            m_PointerEventData = new PointerEventData(m_EventSystem);
            m_PointerEventData.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            m_Raycaster.Raycast(m_PointerEventData, results);
            foreach (RaycastResult result in results)
            {
                Debug.Log("Hit " + result.gameObject.name);
                Destroy(result.gameObject);
                Blowout(result.gameObject.name);
            }

            UIpanel.SetActive(false);
        }
    }

    public void Put(Item item)
    {

        list.Add(item);
        UpdatePanel();
        StartCoroutine(Move(item.transform, itemTypesPoints[(int)item._type]));

        StartCoroutine(POST(item.id, "folding"));
        folding.Invoke();
    }

    private IEnumerator Move(Transform a, Transform b)
    {
        a.parent = null;

        while ((a.position - b.position).sqrMagnitude > 0.05f)
        {
            a.position += (b.position - a.position) * 5 * Time.deltaTime;
            a.rotation = Quaternion.Lerp(a.rotation, b.rotation, .1f);

            yield return null;
        }

        a.position = b.position;
        a.rotation = b.rotation;
        a.parent = b;
    }

    private IEnumerator BlowoutMove(Transform a, Transform b)
    {
        a.parent = null;

        while ((a.position - b.position).sqrMagnitude > 0.05f)
        {
            a.position += (b.position - a.position) * 10 * Time.deltaTime;
            a.rotation = Quaternion.Lerp(a.rotation, b.rotation, .3f);

            yield return null;
        }

        a.parent = null;
        a.GetComponent<Rigidbody>().isKinematic = false;
        a.GetComponent<Collider>().enabled = true;
    }

    public IEnumerator POST(int id_item, string act)
    {
        var Data = new WWWForm();
        Data.AddField("BMeHG5xqJeB4qCjpuJCTQLsqNGaqkfB6", 0);
        Data.AddField("ID", id_item);  
        Data.AddField("ACT", act);	

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