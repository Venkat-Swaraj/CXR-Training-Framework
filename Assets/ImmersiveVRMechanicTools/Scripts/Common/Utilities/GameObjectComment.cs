using UnityEngine;

public class GameObjectComment : MonoBehaviour
{
    public bool IsEditable = true;
    public string TextInfo = "Add comment and lock when finished";
    public int MessageTypeAsInt = 0;
 
    public void ToggleIsEditable ()
    {
        IsEditable = !IsEditable;
    }
 
    void Start()
    {
        enabled = false;
    }
}
