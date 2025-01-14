using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameObjectComment))]
public class GameObjectCommentEditor : Editor
{
    private static readonly int[] ChooseMessageTypeIndexes = {0, 1, 2, 3};
    private static readonly string[] ChooseMessageTypeStrings = {"Box Text ", "Box Info", "Box Warning", "Box Error"};
    
    public override void OnInspectorGUI()
    {
        var gameObjectComment = (GameObjectComment) target;

        if (!gameObjectComment.IsEditable)
        {
            if (GUILayout.Button(gameObjectComment.IsEditable ? "Lock" : "Unlock")) {
                    gameObjectComment.ToggleIsEditable();
            }
            EditorGUILayout.HelpBox(gameObjectComment.TextInfo, (MessageType)gameObjectComment.MessageTypeAsInt);
        }
        else
        {
            if (GUILayout.Button(gameObjectComment.IsEditable ? "Lock" : "Unlock"))
            {
                gameObjectComment.ToggleIsEditable();
            }
            
            gameObjectComment.TextInfo = EditorGUILayout.TextArea(gameObjectComment.TextInfo);

            gameObjectComment.MessageTypeAsInt = EditorGUILayout.IntPopup("Message Type:", gameObjectComment.MessageTypeAsInt, ChooseMessageTypeStrings, ChooseMessageTypeIndexes);
            EditorGUILayout.HelpBox(" Press LOCK at the top when finished", MessageType.Warning);
        }
    }
}