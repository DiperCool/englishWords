namespace WordsEnglish.Core
    open System
    open FSharp.Json
    open System.Net.Http
    open System.IO
    open System.Threading
    open System.Threading.Tasks
    open System
    open System.Diagnostics
    open System.Runtime.InteropServices
    module TTS=

        let specialFolder= Environment.GetFolderPath Environment.SpecialFolder.LocalApplicationData
        let folder = Path.Combine(specialFolder, "WordsEnglish/media")
        let createDirectoryIfNotExist () =
            match Directory.Exists folder with
            | true -> ()
            | false -> 
                Directory.CreateDirectory folder |> ignore

        let getTranslate (text:string) (guid:string)=
            createDirectoryIfNotExist ()
            let httpClient= new HttpClient()
            let url = new Uri($"http://api.voicerss.org/?key=8ab88e67a0d945269eacc0bb117e3253&hl=en-us&src={text}")
            let array = httpClient.GetByteArrayAsync(url)
            let path = Path.Combine (specialFolder, $"WordsEnglish/media/{guid}.wav")

            File.WriteAllBytes(path, array.Result)
        let run path=
            let program = 
                        if RuntimeInformation.IsOSPlatform(OSPlatform.Linux) then
                            "cvlc"
                        else
                            "vlc.exe"

            let newProcess= new Process()
            let infoProcces= new ProcessStartInfo()
            infoProcces.FileName <- program
            infoProcces.Arguments <-  $"{path} --play-and-exit"
            infoProcces.RedirectStandardOutput <-  true
            infoProcces.UseShellExecute <-  false
            infoProcces.CreateNoWindow <-  true
            newProcess.StartInfo <- infoProcces
            newProcess.Start() |> ignore