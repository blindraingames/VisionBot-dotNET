# VisionBot
This is small library for receiving image descriptions from [VisionBot service](https://visionbot.ru) written in CSharp.
Library sends post request to service and waits for result.
Target dotNET version is .NET Framework 4.8, but it should compiles on later versions.

# Simple example of using
```
var bytes = File.ReadAllBytes("img.jpg");
                var vb = new BlindRainGames.Utils.VisionBot.VisionBot();
                var tsk = vb.GetImageDescriptionAsStringAsync(bytes);
// Wait for result...
string result = tsk.Result;
vb.Dispose();
```

# Dependences
Newtonsoft.Json [https://www.nuget.org/packages/Newtonsoft.Json](https://www.nuget.org/packages/Newtonsoft.Json)