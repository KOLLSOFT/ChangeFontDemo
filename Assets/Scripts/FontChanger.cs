using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class FontChanger : MonoBehaviour
{
    public Font systemFont;
    public Font thridFont;
    public Text targetText;
    private Font curFont = null;
    public Text textinfo;
    private Button btn;
    private void Start()
    {
        // systemFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
        curFont = systemFont;
        systemFont = targetText.font;

        SetTrackedOnTextTrackAction();

    }

    [Button("获取系统安装的字体")]
    public void GetOSInstalledFonts()
    {
        var names = Font.GetOSInstalledFontNames();
        foreach (var fontName in names)
        {
            Debug.Log($"FontName: {fontName}");
        }
    }


    private Dictionary<Font, HashSet<Text>> FontUpdateTracker_TrackedDic;
    Dictionary<Font, HashSet<Text>> GetTrackedTexts()
    {
        if (null != FontUpdateTracker_TrackedDic)
        {
            return FontUpdateTracker_TrackedDic;
        }
        var type = typeof(FontUpdateTracker);
        const BindingFlags flags = BindingFlags.Static | BindingFlags.NonPublic;
        FieldInfo fieldInfo = type.GetField("m_Tracked", flags);
        FontUpdateTracker_TrackedDic = fieldInfo?.GetValue(null) as Dictionary<Font, HashSet<Text>>;
        return FontUpdateTracker_TrackedDic;
    }
    
    private Action<Text> FontUpdateTracker_OnTextTrackAction;
    void SetTrackedOnTextTrackAction()
    {
        FontUpdateTracker.onTrackText += text =>
        {
            if (null == text)
            {
                return;
            }
            text.font = curFont;
            Debug.Log($"Change Text Font: {text.name}");
        };
    }
    
    [Button("切换字体")]
    public void SwitchFont()
    {
        if (null == targetText)
        {
            return;
        }
            
        //切换当前已有Text的字体
        curFont = curFont == systemFont ? thridFont : systemFont;
        var obj = GetTrackedTexts();
        if (null == obj)
        {
            return;
        }

        var textSet = obj.Values.ToArray();

        foreach (var set in textSet)
        {
            var textArr = set.ToArray();
            foreach (var text in textArr)
            {
                text.font = curFont;
            }
        }
        
        //TODO 新实例化的Text更换为新字体
        // targetText.font = curFont;
        var info = $"当前字体：{curFont.name}";
        Debug.Log(info);
        textinfo.text = info;
        var fontNames = curFont.fontNames;
    }

    private List<GameObject> addedTextList = new List<GameObject>();
    public void AddTextProto()
    {
        var textProto = Instantiate(Resources.Load<GameObject>("TextProto"), transform, true);
        textProto.transform.localScale = Vector3.one;
        addedTextList.Add(textProto);
    }

    public void ClearAddedText()
    {
        for (int i = 0; i < addedTextList.Count; i++)
        {
            var textGo = addedTextList[i];
            Destroy(textGo);
        }
        addedTextList.Clear();
    }
}