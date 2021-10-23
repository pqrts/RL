using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
public class ext_CharacterSp : MonoBehaviour
{

    public string _char_runtime_name;
    public GameObject _char_GO;
    public Image _char_body;
    public Image _char_haircut;
    public Image _char_clothes;
    public Image _char_makeup;
    private string _s_body;
    private string _s_haircut;
    private string _s_clothes;
    private string _s_makeup;
    ext_StorylineEditor s_StorylineEd;
    public GameObject Spawn(GameObject Canvas, string root, string path_body, string path_haircut, string path_clothes, string path_makeup, string path_characters, string char_name)
    {
        Debug.Log(Canvas);
        try
        {
            
            StreamReader SR = new StreamReader(path_characters, encoding: System.Text.Encoding.GetEncoding("windows-1251"));
            string line = SR.ReadLine();
            _char_runtime_name = line;
            int count = 1;
            while (line != null)
            {
                line = SR.ReadLine();
                switch (count)
                {
                    case 1:
                        _s_body = line;
                        count += 1;
                        break;
                    case 2:
                        _s_haircut = line;
                        count += 1;
                        break;
                    case 3:
                        _s_clothes = line;
                        count += 1;
                        break;
                    case 4:
                        _s_makeup = line;
                        count += 1;
                        break;
                }
            }
            SR.Close();
            string resources_path_body = path_body.Replace(root + "/Resources/", "") + "/" + _s_body;
            string resources_path_haircut = path_haircut.Replace(root + "/Resources/", "") + "/" + _s_haircut;
            string resources_path_clothes = path_clothes.Replace(root + "/Resources/", "") + "/" + _s_clothes;
            string resources_path_makeup = path_makeup.Replace(root + "/Resources/", "") + "/" + _s_makeup;

            if (Create_body(Canvas, resources_path_body, char_name))
            {
                Create_haircut( resources_path_haircut);
                Create_clothes( resources_path_clothes);
                if (Create_makeup( resources_path_makeup))
                {
                    _char_GO.tag = "character";
                    _char_GO.AddComponent<local_character>();
                    _char_GO.GetComponent<local_character>().reset_param(_char_runtime_name, _char_body, _char_haircut, _char_clothes, _char_makeup);
                    _char_GO.name = char_name;
                }

            }

        }
        catch (Exception ex)
        {
            Debug.Log("Error: " + ex.Message);
        }
        return _char_GO;
    }

    Boolean Create_body(GameObject Canvas, string res_p_body, string name)
    {
        try
        {
            GameObject t = new GameObject(_char_runtime_name);
            _char_GO = t;
            t.transform.SetParent(Canvas.transform, false);
            RectTransform rt = t.AddComponent<RectTransform>();
            rt.localScale = Vector3.one;
            rt.anchoredPosition = new Vector2(0f, 0f);
            rt.sizeDelta = new Vector2(720, 1280);
            t.AddComponent<Image>();
            _char_body = t.GetComponent<Image>();
            Texture2D tex;
            tex = Resources.Load(res_p_body) as Texture2D;
            _char_body.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(tex.width / 2, tex.height / 2));
        }
        catch (Exception ex)
        {
            Debug.Log("Error: " + ex.Message);
            return false;
        }
        return true;
    }
    Boolean Create_haircut( string res_p_haircut)
    {
        try
        {
            GameObject t = new GameObject("haircut");
            t.transform.SetParent(_char_body.transform, false);
            RectTransform rt = t.AddComponent<RectTransform>();
            rt.localScale = Vector3.one;
            rt.anchoredPosition = new Vector2(0f, 0f);
            rt.sizeDelta = new Vector2(720, 1280);
            t.AddComponent<Image>();
            _char_haircut = t.GetComponent<Image>();
            Texture2D tex2;
            tex2 = Resources.Load(res_p_haircut) as Texture2D;
            _char_haircut.sprite = Sprite.Create(tex2, new Rect(0, 0, tex2.width, tex2.height), new Vector2(tex2.width / 2, tex2.height / 2));
        }
        catch (Exception ex)
        {
            Debug.Log("Error: " + ex.Message);
            return false;
        }
        return true;
    }
    Boolean Create_clothes( string res_p_clothes)
    {
        try
        {
            GameObject t = new GameObject("clothes");
            t.transform.SetParent(_char_body.transform, false);
            RectTransform rt = t.AddComponent<RectTransform>();
            rt.localScale = Vector3.one;
            rt.anchoredPosition = new Vector2(0f, 0f);
            rt.sizeDelta = new Vector2(720, 1280);
            t.AddComponent<Image>();
            _char_clothes = t.GetComponent<Image>();
            Texture2D tex3;
            tex3 = Resources.Load(res_p_clothes) as Texture2D;

            _char_clothes.sprite = Sprite.Create(tex3, new Rect(0, 0, tex3.width, tex3.height), new Vector2(tex3.width / 2, tex3.height / 2));
        }
        catch (Exception ex)
        {
            Debug.Log("Error: " + ex.Message);
            return false;
        }
        return true;
    }
    Boolean Create_makeup( string res_p_makeup)
    {
        try
        {
            GameObject t = new GameObject("makeup");
            t.transform.SetParent(_char_body.transform, false);
            RectTransform rt = t.AddComponent<RectTransform>();
            rt.localScale = Vector3.one;
            rt.anchoredPosition = new Vector2(0f, 0f);
            rt.sizeDelta = new Vector2(720, 1280);
            t.AddComponent<Image>();
            _char_makeup = t.GetComponent<Image>();
            Texture2D tex4;
            tex4 = Resources.Load(res_p_makeup) as Texture2D;

            _char_makeup.sprite = Sprite.Create(tex4, new Rect(0, 0, tex4.width, tex4.height), new Vector2(tex4.width / 2, tex4.height / 2));
        }
        catch (Exception ex)
        {
            Debug.Log("Error: " + ex.Message);
            return false;
        }
        return true;
    }

}
