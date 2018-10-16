#region Header
/* ============================================ 
 *			    Strix Unity Library
 *		https://github.com/strix13/UnityLibrary
 *	============================================ 	
 *	관련 링크 :
 *	
 *	설계자 : 
 *	작성자 : Strix
 *	
 *	기능 : 
 *	
 *	뼈대가 된 코드
 *	https://gist.github.com/StephenHodgson/a64ea696d0206214d06ca55f01fe2711
   ============================================ */
#endregion Header

using System;
using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TextAsset))]
public class CEditorInspector_MarkdownViewer : Editor
{
    static private GUIStyle _pGUIStyle_Normal;
    static private GUIStyle _pGUIStyle_Header;
    static private GUIStyle _pGUIStyle_URL;

    public override void OnInspectorGUI()
    {
        string strPath = AssetDatabase.GetAssetPath(target);
        if (string.IsNullOrEmpty(strPath)) return;

        if (strPath.EndsWith(".md"))
        {
            GUI.enabled = true;
            MdInspectorGUI(strPath);
        }
        else
        {
            base.OnInspectorGUI();
        }
    }

    private static void MdInspectorGUI(string path)
    {
        try
        {
            using (var pReader = new StreamReader(path))
            {
                _pGUIStyle_Normal = new GUIStyle();
                _pGUIStyle_Normal.richText = true;

                _pGUIStyle_Header = new GUIStyle();
                _pGUIStyle_Header.richText = true;
                _pGUIStyle_Header.fontStyle = FontStyle.Bold;

                _pGUIStyle_URL = new GUIStyle();
                _pGUIStyle_URL.richText = true;
                _pGUIStyle_URL.fontStyle = FontStyle.Italic;

                string strLine;
                do
                {
                    strLine = pReader.ReadLine();

                    if (strLine != null)
                    {
                        strLine = CommonMark.CommonMarkConverter.Convert(strLine);
                        Parsing_HTMLTag(strLine);

                        //EditorGUILayout.BeginHorizontal();
                        //EditorGUILayout.LabelField(strLine, pGUIStyle);
                        //EditorGUILayout.EndHorizontal();
                    }
                }
                while (strLine != null);
                pReader.Close();
            }
        }
        catch (Exception e)
        {
            Debug.LogErrorFormat("{0}\n", e.Message);
        }
    }

    private static void Parsing_HTMLTag(string strLine)
    {
        if (strLine.Contains("<hr />"))
        {
            EditorGUILayout.Separator();
            return;
        }

        bool bIsHeader = strLine.Contains("<h1>") || strLine.Contains("<h2>") || strLine.Contains("<h3>");
        bool bIsContainURL = (strLine.Contains("http://") || strLine.Contains("https://")) ;

        strLine = strLine.Replace("&quot;", "\"");
        strLine = strLine.Replace("&bull;", "•");
        strLine = strLine.Replace("&trade;", "™");
        strLine = strLine.Replace("&copy;", "Ⓒ");
        strLine = strLine.Replace("&sum;", "∑");
        strLine = strLine.Replace("&prod;", "∏");
        strLine = strLine.Replace("&ni;", "∋");
        strLine = strLine.Replace("&notin;", "∉");
        strLine = strLine.Replace("&isin;", "∈");
        strLine = strLine.Replace("&nabla;", "∇");
        strLine = strLine.Replace("&empty;", "∅");
        strLine = strLine.Replace("&exist;", "∃");
        strLine = strLine.Replace("&part;", "∂");
        strLine = strLine.Replace("&forall;", "∀");
        strLine = strLine.Replace("&forall;", "Δ");

        // <h1> </h1> etc...
        strLine = strLine.Replace("<h1>", "<size=18>");
        strLine = strLine.Replace("</h1>", "</size>");

        strLine = strLine.Replace("<h2>", "<size=16>");
        strLine = strLine.Replace("</h2>", "</size>");

        strLine = strLine.Replace("<h3>", "<size=14>");
        strLine = strLine.Replace("</h3>", "</size>");


        strLine = strLine.Replace("<p>", "");
        strLine = strLine.Replace("</p>", "\n");

        strLine = strLine.Replace("<em>", "<i>");
        strLine = strLine.Replace("</em>", "</i>");

        // <strong> </strong>
        strLine = strLine.Replace("<strong>", "<b>");
        strLine = strLine.Replace("</strong>", "</b>");


        // <ul>
        // <li> </li>
        // </ul>
        strLine = strLine.Replace("<ul>", "");
        strLine = strLine.Replace("</ul>", "");
        strLine = strLine.Replace("<li>", "● ");
        strLine = strLine.Replace("</li>", "");

        strLine = strLine.Replace("![](", "Image Link : ");
        strLine = strLine.Replace("<img src=", "Image Link : ");

        // <ol>
        // <li> </li>
        // </ol>

        // <a herf=" ... ">
        strLine = strLine.Replace("● <a href=", "");
        strLine = strLine.Replace("<a href=", "");
        strLine = strLine.Replace("\"", "");


        if (bIsHeader)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("");
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(strLine, _pGUIStyle_Header);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.EndHorizontal();
        }
        else if (bIsContainURL)
        {
            int iIndex = strLine.IndexOf("https://");
            if(iIndex == -1)
                iIndex = strLine.IndexOf("http://");

            string strLink = strLine.Substring(iIndex, strLine.Length - iIndex - 1);
            int iIndexCut = strLink.IndexOf(">");
            if (iIndexCut != -1)
                strLink = strLink.Substring(0, iIndexCut);

            iIndexCut = strLink.IndexOf(")");
            if (iIndexCut != -1)
                strLink = strLink.Substring(0, iIndexCut);
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(strLine.Substring(0, iIndex), _pGUIStyle_Normal);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            
            EditorGUILayout.LabelField("<color=#0000ff>" + strLink + "</color>", _pGUIStyle_URL);
            EditorGUILayout.EndHorizontal();

            Rect pRect_URL_Button = GUILayoutUtility.GetLastRect();
            if (Event.current.type == EventType.MouseUp && pRect_URL_Button.Contains(Event.current.mousePosition))
                Application.OpenURL(strLink);

            string strUnderLine = "<color=#0000ff>";
            for (int i = 0; i < strLink.Length - 5; i++)
                strUnderLine += "_";
            strUnderLine += "</color>";

            GUI.Label(pRect_URL_Button, strUnderLine, _pGUIStyle_Normal);
        }
        else
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(strLine, _pGUIStyle_Normal);
            EditorGUILayout.EndHorizontal();
        }
    }
}