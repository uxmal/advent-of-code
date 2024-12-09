var secret = args[0];
for (int i = 1; i < int.MaxValue; ++i)
{
    var input = secret + i.ToString();
    var md5 = CreateMD5(input);
    if (md5.StartsWith("000000"))
    {
        Console.WriteLine("Found: {0} {1}", i, md5);
        break;
    }
}


static string CreateMD5(string input)
{
    // Use input string to calculate MD5 hash
    using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
    {
        byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
        byte[] hashBytes = md5.ComputeHash(inputBytes);

        return Convert.ToHexString(hashBytes);
    }
}
