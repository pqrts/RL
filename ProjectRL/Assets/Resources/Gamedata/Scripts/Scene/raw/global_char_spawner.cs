using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;
using System;
public class global_char_spawner : MonoBehaviour
{

    public GameObject _Canvas;
    public string _char_runtime_name;
    private GameObject _char_GO;
    public Image _char_body;
    public Image _char_haircut;
    public Image _char_clothes;
    public Image _char_makeup;
    private string _s_body;
    private string _s_haircut;
    private string _s_clothes;
    private string _s_makeup;
    public GameObject Spawn(GameObject Canvas, string root, string p_body, string p_haircut, string p_clothes, string p_makeup, string p_characters, string char_name)
    {
        try
        {
            string path = p_characters + "/" + char_name + ".char";
            StreamReader SR = new StreamReader(path, encoding: System.Text.Encoding.GetEncoding("windows-1251"));
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
                        Debug.Log(line);
                        count += 1;
                        break;
                    case 2:
                        _s_haircut = line;
                        Debug.Log(line);
                        count += 1;
                        break;
                    case 3:
                        _s_clothes = line;
                        Debug.Log(line);
                        count += 1;
                        break;
                    case 4:
                        _s_makeup = line;
                        count += 1;
                        Debug.Log(line);
                        break;
                }
            }
            SR.Close();
            string res_p_body = p_body.Replace(root + "/Resources/", "") + "/" + _s_body;
            string res_p_haircut = p_haircut.Replace(root + "/Resources/", "") + "/" + _s_haircut;
            string res_p_clothes = p_clothes.Replace(root + "/Resources/", "") + "/" + _s_clothes;
            string res_p_makeup = p_makeup.Replace(root + "/Resources/", "") + "/" + _s_makeup;

            if (Create_body(Canvas, res_p_body))
            {
                Create_haircut(res_p_haircut);
                Create_clothes(res_p_clothes);
                if (Create_makeup(res_p_makeup))
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

    Boolean Create_body(GameObject Canvas, string res_p_body)
    {
        try
        {
            GameObject t = new GameObject(_char_runtime_name);
            _char_GO = t;
            t.transform.SetParent(Canvas.transform, false);
            RectTransform rt = t.AddComponent<RectTransform>();
            rt.localScale = new  Vector3(1.8f,1.8f,1.8f);
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