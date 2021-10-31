using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
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
