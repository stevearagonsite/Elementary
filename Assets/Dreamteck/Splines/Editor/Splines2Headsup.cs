namespace Dreamteck.Splines
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;

    public class Splines2Headsup : SplineEditorWindow
    {
        Texture2D headerTex = null;
        GUIStyle titleStyle, textStyle, textStyleBold;
        bool init = false;
        float timeOpened = 0f;

        protected override string GetTitle()
        {
            return "Dreamteck Splines 2";
        }

        void InitGUI()
        {
            titleStyle = new GUIStyle(GUI.skin.label);
            titleStyle.fontSize = 18;
            titleStyle.fontStyle = FontStyle.Bold;
            Color col = titleStyle.normal.textColor;
            col.a = 0.85f;
            titleStyle.normal.textColor = col;
            titleStyle.alignment = TextAnchor.MiddleCenter;
            textStyle = new GUIStyle(GUI.skin.label);
            textStyle.wordWrap = true;
            textStyle.padding = new RectOffset(10,10, 10, 10);
            textStyleBold = new GUIStyle(textStyle);
            textStyleBold.fontStyle = FontStyle.Bold;
            headerTex = ImageDB.GetImage("splines2header.png", "Splines/Editor/Icons");
            maxSize = minSize = new Vector2(headerTex.width, headerTex.height * 2.5f);
            timeOpened = Time.realtimeSinceStartup;
            init = true;
        }

        

        private void OnDestroy()
        {
            if(Time.realtimeSinceStartup - timeOpened < 10)
            {
                if(EditorUtility.DisplayDialog("Did you read it?", "We apologize for the inconvenience but this text is quite important. Did you have time to read it fully? We want to prevent future progress loss.", "I'll read", "Please, skip"))
                {
                    var newWin = Instantiate<Splines2Headsup>(this);
                    newWin.Show();
                }
            }
        }

        private void OnGUI()
        {
            if (!init) InitGUI();
            GUI.DrawTexture(new Rect(0, 0, headerTex.width, headerTex.height), headerTex, ScaleMode.ScaleToFit);
            GUILayout.BeginArea(new Rect(0, headerTex.height, position.width, position.height - headerTex.height));
            EditorGUILayout.Space();
            GUILayout.Label("No, this is not an update. This is an entirely new thing!", titleStyle, GUILayout.Width(position.width));
            string text = "After years of supporting Dreamteck Splines and observing how everyone has been putting it to great use, we took a big step forward and re-designed the tool." +
                "Dreamteck Splines 2 takes the idea of its predecessor and expands upon it with a completely new archtecture approach.";
            GUILayout.Label(text, textStyle);
            EditorGUILayout.Space();
            text = "New editor UI, new functionality, major performance improvements, improved junction workflows, completely refactored code and much much more is coming this June." +
                "We want to take your time for a second to let everyone know that this new version of Dreamteck Splines is NOT compatible with any previous versions. If you upgrade to Dreamteck Splines 2 and your project" +
                " is making heavy use of splines, a lot of functinality is bound to break. If your project is far into development with Dreamteck Splines 1, our advice is not to upgrade to 2 as a lot of progress might get lost." +
                "We have planned for that and this is why we are also releasing Dreamteck Splines 1.1 Legacy on the Asset Store. All of our current customers will get this asset for free automatically and we will continue to support version 1.1 for the following year.";
            GUILayout.Label(text, textStyle);
            EditorGUILayout.Space();
            text = "Attention: We are currently making a trailer video for Dreamteck Splines 2 and would like to include footage from games made with Dreamteck Splines (providing credit along the way)." +
                "If you would like to showcase your project in our new trailer, send us a link to a 1920x1080 video at team@dreamteck.io We would love to see what you are working on." +
                "You can also always count on our team for support if you decide to upgrade your project from version 1 to 2. Just hit us up at support@dreamteck.io!";
            GUILayout.Label(text, textStyleBold);
            GUILayout.EndArea();
        }
    }
}