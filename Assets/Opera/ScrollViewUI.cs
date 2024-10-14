using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollViewUI : MonoBehaviour
{

    public Transform parent;
    public GameObject itemPrefab;
    public List<GameObject> createdItems = new List<GameObject>();

    public delegate  void MyDelegate(int num);
    public MyDelegate myDelegate;

    private void Awake()
    {
        createdItems = new List<GameObject>();
    }

    void Start()
    {
       
    }

    void Update()
    {
        
    }

    public void ClearItems()
    {
        for(int i = createdItems.Count - 1; i >= 0; i--)
        {
            Destroy(createdItems[i].gameObject);
            createdItems.RemoveAt(i);
        }
        createdItems = new List<GameObject>();
    }

    public void AddOneItem(string text)
    {
        GameObject newGA = Instantiate<GameObject>(itemPrefab);
        createdItems.Add(newGA);
        Debug.Log("createdItems.Add(newGA);");
        newGA.GetComponent<RectTransform>().SetParent(parent);
        newGA.GetComponentInChildren<Text>().text = text;
        ScrollViewUI_button button = newGA.GetComponent<ScrollViewUI_button>();
        button.buttonNumber = 0;
        button.scrollViewUI = this;
    }

    public void OnOneButtonPressed(ScrollViewUI_button button, int number)
    {
        if(number == 0)
        {
            Debug.Log(createdItems.Count);
            int num = createdItems.IndexOf(button.gameObject);
            myDelegate(num);
        }
    }

}
