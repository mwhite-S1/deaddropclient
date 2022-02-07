using System;
using System.Text;
using System.Net;
using Microsoft.ClearScript.V8;
using Newtonsoft.Json;

string baseUrl = "https://dead-drop.me";

// Use Chrome's V8 javascript engine 
using (V8ScriptEngine engine = new V8ScriptEngine("jscript"))
{
    StringBuilder str = new StringBuilder();

    // merge the javascripts together to create a deaddrop environment
    // merseen generator, sjcl and two functions from deaddrop.js

    // https://gist.github.com/banksean/300494
    str.Append(File.ReadAllText("scripts/merseen.js"));

    // https://github.com/bitwiseshiftleft/sjcl
    str.Append(File.ReadAllText("scripts/sjcl.js"));

    // https://github.com/BillKeenan/dead-drop-python
    str.Append(File.ReadAllText("scripts/deaddrop.js"));
    
    engine.Execute(str.ToString());

    // throw some somewhat random stuff at it
    Random rand = new Random(DateTime.Now.Millisecond);
    engine.Script.sjcl.random.addEntropy(DateTime.Now.Ticks, rand.NextInt64() , Guid.NewGuid().ToByteArray());

    // call js function to make the passwd
    var passwd = engine.Script.makePwd();

    // call js function to do the encryption using the passwd
    // Put your message to encrypt and drop to the second parameter string
    var encrypted = engine.Script.symmetricEncrypt(passwd, "Random text here");

    Console.WriteLine("Passwd: " + passwd);
    Console.WriteLine("Encrypted: " + encrypted);

    // Test decryption.  We hope it's the same :-)
    var decrypted = engine.Script.symmetricDecrypt(passwd, encrypted);
    Console.WriteLine("Decrypted: " + decrypted);

    // send the drop to the web site
    using (HttpClient client = new HttpClient())
    {
        string url = baseUrl + "/drop";
        string body = "data=" + Uri.EscapeDataString(encrypted);

        var data = new StringContent(body, Encoding.UTF8, "application/x-www-form-urlencoded");

        // Get the response back
        var response = await client.PostAsync(url, data);
        string result = response.Content.ReadAsStringAsync().Result;

        Console.WriteLine("Result: " + result);

        dynamic val = JsonConvert.DeserializeObject(result);
        string id = val.id;

        // Show the pickup URL and password that can be used to retrieve the dead drop
        Console.WriteLine(String.Format("{0}/pickup/{1}", baseUrl, id));
        Console.WriteLine("Password: " + passwd);
    }
}
