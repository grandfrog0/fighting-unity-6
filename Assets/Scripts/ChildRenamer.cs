using UnityEngine;

public class ChildRenamer : MonoBehaviour
{
    private void Start()
    {
        RenameChilds(transform);
    }

    private void RenameChilds(Transform parent)
    {
        int index = parent.name.IndexOf(":") - 1;

        if (index > -1 && char.IsDigit(parent.name[index]))
            parent.name = parent.name.Remove(index, 1);

        for (int i = 0; i < parent.childCount; i++)
        {
            RenameChilds(parent.GetChild(i));
        }
    }
}
