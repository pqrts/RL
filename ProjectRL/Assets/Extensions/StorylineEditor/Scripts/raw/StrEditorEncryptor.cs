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
[RequireComponent(typeof(StrEditorReplacer))]
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
        string convertedName = ConvertString(storylineName);
        string modificatedName = Modificate(convertedName);
        string convertedFinalExtension = ConvertString(StrExtensions.FinalStr);
        string modificatedFinalExtension = Modificate(convertedFinalExtension);
        string convertedKeyExtension = ConvertString(StrExtensions.Key);
        string modificatedKeyExtension = Modificate(convertedKeyExtension);
        string convertedIVExtension = ConvertString(StrExtensions.IV);
        string modificatedIVExtension = Modificate(convertedIVExtension);
        string modificatedFinalFilePath = _StrRootObject._folders._storylines + "/" + modificatedName + "." + modificatedFinalExtension;
        string modificatedKeyFilePath = _StrRootObject._folders._storylines + "/" + modificatedName + "." + modificatedKeyExtension;
        string modificatedIVFilePath = _StrRootObject._folders._storylines + "/" + modificatedName + "." + modificatedIVExtension;
        EncryptContent(composedStoryline, modificatedFinalFilePath, modificatedKeyFilePath, modificatedIVFilePath);
    }
    public string ConvertString(string original)
    {
        byte[] unconverted = Encoding.UTF8.GetBytes(original);
        string tempConverted = BitConverter.ToString(unconverted);
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
        byte[] fromHex = new byte[hex.Length / 2];
        for (int i = 0; i < fromHex.Length; i++)
        {
            fromHex[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
        }
        return fromHex;
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
    public void EncryptContent(string fileContent, string finalFilePath, string keyFilePath, string ivFilePath)
    {
        using (Aes myAes = Aes.Create())
        {
            byte[] encrypted = EncryptString(fileContent, myAes.Key, myAes.IV);
            string roundtrip = DecryptString(encrypted, myAes.Key, myAes.IV);
            File.WriteAllBytes(finalFilePath, encrypted);
            File.WriteAllBytes(keyFilePath, myAes.Key);
            File.WriteAllBytes(ivFilePath, myAes.IV);
        }
    }
    static byte[] EncryptString(string plainText, byte[] Key, byte[] IV)
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
    static string DecryptString(byte[] cipherText, byte[] Key, byte[] IV)
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
}
