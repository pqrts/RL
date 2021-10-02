using UnityEngine;
using UnityEngine.UI;
public class local_character : MonoBehaviour
{
    public string _char_runtime_name;
    public Image _char_body;
    public Image _char_haircut;
    public Image _char_clothes;
    public Image _char_makeup;
    public void reset_param(string char_name, Image body, Image haircut, Image clothes, Image makeup)
    {
        _char_runtime_name = char_name;
        _char_body = body;
        _char_haircut = haircut;
        _char_clothes = clothes;
        _char_makeup = makeup;
    }
}