using UnityEngine;
using System;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using StorylineEditor;
public class Decryptor : MonoBehaviour
{
    // Start is called before the first frame update
    public string Modificate(string unmodificated)
    {
        string result = "";
        string splitBy = "-";
        string[] Units = unmodificated.Split(splitBy.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        foreach (string a in Units)
        {
            char[] temp = a.ToCharArray();
            if (!Char.IsLetter(temp[0]) && !Char.IsLetter(temp[1]))
            {
                int tempInt1 = int.Parse(temp[0].ToString());
                int tempInt2 = int.Parse(temp[1].ToString());
                string tempString = ((tempInt1 * 14) / 4).ToString() + ((tempInt2 * 14) / 4).ToString();
                result = result + tempString + "&";
            }
            else
            {
                result = result + a + "&";
            }
        }
        return result;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
