using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using StorylineEditor;

[RequireComponent(typeof(StrEditorEvents))]
[RequireComponent(typeof(StrEditorGodObject))]
[RequireComponent(typeof(extStrEditorReplacer))]
[RequireComponent(typeof(TaglistReader))]
[ExecuteInEditMode]

public class StrEditorEncryptor : MonoBehaviour
{
    private StrEditorGodObject _StrRootObject;
    private byte[] _fileContent;
    public Boolean GetRequieredComponents()
    {
        StrEditorGodObject tempRootObject = GetComponent<StrEditorGodObject>();
        if (tempRootObject is IStrEditorRoot)
        {
            _StrRootObject = tempRootObject;
        }
        else
        {
            throw new ArgumentException("'StrEditorRoot' must implement the 'IStrEditorRoot' interface");
        }

        return true;
    }
    public void ExportToFile(string storylineName, string composedStoryline)
    {
        string convertedName = ConvertI(storylineName);
        string mod = Modificate(convertedName);
        string convertedExtension = ConvertI(StrExtensions.FinalStr);
        string modExtension = Modificate(convertedExtension);
        Debug.Log(convertedName);
        Debug.Log(mod+"."+modExtension);

        string filePath = _StrRootObject._folders._storylines + "/" + convertedName;
    }
    public string ConvertI(string original)
    {

        byte[] unconverted = Encoding.UTF8.GetBytes(original);
        string tempConverted = BitConverter.ToString(unconverted);
        //  string converted = tempConverted.Replace("-", "_");

        Unconvert(tempConverted);
        return tempConverted;
    }
    public void Unconvert(string toUnconvert)
    {
        byte[] data = FromHex(toUnconvert);
        string s = Encoding.UTF8.GetString(data);
        Debug.Log(s);
    }
    public byte[] FromHex(string hex)
    {
        hex = hex.Replace("-", "");
        byte[] raw = new byte[hex.Length / 2];
        for (int i = 0; i < raw.Length; i++)
        {
            raw[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
        }
        return raw;
    }
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
    public void EncryptContent(string content)
    {
        using (Aes myAes = Aes.Create())
        {
            byte[] encrypted = EncryptStringToBytes_Aes(content, myAes.Key, myAes.IV);
            string roundtrip = DecryptStringFromBytes_Aes(encrypted, myAes.Key, myAes.IV);
            string path = "Assets/Resources/Gamedata/Storylines/str1.str.txt";
            File.WriteAllBytes(path, encrypted);
            Debug.Log(roundtrip);
        }
    }
    static byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV)
    {
        if (plainText == null || plainText.Length <= 0)
            throw new ArgumentNullException("plainText");
        if (Key == null || Key.Length <= 0)
            throw new ArgumentNullException("Key");
        if (IV == null || IV.Length <= 0)
            throw new ArgumentNullException("IV");
        byte[] encrypted;
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Key;
            aesAlg.IV = IV;
            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }
                    encrypted = msEncrypt.ToArray();
                }
            }
        }
        return encrypted;
    }
    static string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key, byte[] IV)
    {
        if (cipherText == null || cipherText.Length <= 0)
            throw new ArgumentNullException("cipherText");
        if (Key == null || Key.Length <= 0)
            throw new ArgumentNullException("Key");
        if (IV == null || IV.Length <= 0)
            throw new ArgumentNullException("IV");
        string plaintext = null;
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Key;
            aesAlg.IV = IV;
            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
            using (MemoryStream msDecrypt = new MemoryStream(cipherText))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        plaintext = srDecrypt.ReadToEnd();
                    }
                }
            }
        }
        return plaintext;
    }
    public void WriteStrFile(string path, byte[] fileContent)
    {
        File.WriteAllBytes(path, fileContent);
    }
}
