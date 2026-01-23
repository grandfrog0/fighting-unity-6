using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSelector : MonoBehaviour
{
    [SerializeField] Transform selection;
    [SerializeField] TMP_Text title, description;
    public ItemView SelectedItem;

    private void Start()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            ItemView child = transform.GetChild(i).GetComponent<ItemView>();
            child.Button.onClick.AddListener(() => OnChildClicked(child));

            if (i == 0)
                SelectedItem = child;
        }
    }

    private void OnChildClicked(ItemView item)
    {
        SelectedItem = item;
        selection.transform.position = SelectedItem.transform.position;
        title.text = item.Item.Name;
        description.text = item.Item.Description;
    }
}
