using UnityEngine;
using UnityEditor;
using Framework.Player;

namespace Framework.E{
    [CustomEditor(typeof(PlayerState), true)]
    public class StateInspector : Editor{
        public override void OnInspectorGUI(){
            if (((PlayerState)target).isPassiveState){
                var style = new GUIStyle(GUI.skin.label) {alignment = TextAnchor.MiddleCenter};
                GUILayout.Label("!! Passive State !!", style);
                EditorGUILayout.Separator();
            }

            base.OnInspectorGUI();
        }
    }
}