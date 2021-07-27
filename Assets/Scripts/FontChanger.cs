using System.Collections.Generic;
using System.Linq;
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
    private void Start()
    {
        systemFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
        curFont = systemFont;
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
        var type = typeof(FontUpdateTracker);
        const BindingFlags flags = BindingFlags.Static | BindingFlags.NonPublic;
        FieldInfo fieldInfo = type.GetField("m_Tracked", flags);
        var obj = fieldInfo?.GetValue(null) as Dictionary<Font, HashSet<Text>>;
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
        Debug.Log($"切换字体：{curFont.name}");
        var fontNames = curFont.fontNames;
    }
}