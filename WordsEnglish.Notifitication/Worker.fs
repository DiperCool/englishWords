namespace WordsEnglish.Notifitication

open System
open System.Collections.Generic
open System.Linq
open System.Threading
open System.Threading.Tasks
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open WordsEnglish.Core
open System
open System.Diagnostics
open System.Threading.Tasks

module Notifitication=
    let send text=
        let newProcess= new Process()
        let command = $"notify-send {text} -u critical -t 60"
        let infoProcces= new ProcessStartInfo()
        infoProcces.FileName <- "/bin/bash"
        infoProcces.Arguments <-  $"-c \"{command}\""
        infoProcces.RedirectStandardOutput <-  true
        infoProcces.UseShellExecute <-  false
        infoProcces.CreateNoWindow <-  true
        newProcess.StartInfo <- infoProcces
        newProcess.Start() |> ignore
        let result = newProcess.StandardOutput.ReadToEnd();
        newProcess.WaitForExit |> ignore
        0

module Worker =
    type CommandResult = { 
      ExitCode: int; 
      StandardOutput: string;
      StandardError: string 
    }

    type Worker(logger: ILogger<Worker>) =
        inherit BackgroundService()

        override _.ExecuteAsync(ct: CancellationToken) =
            async {
              while not ct.IsCancellationRequested do

                let res = ListWordsDB.getListsWordsWhichReadyToLearn ()
                for (item: ListWordsDB.ListWords) in res do
                    (ListWordsDB.setNotificateStatus item.id "true") |> ignore
                    Notifitication.send $"'Go learn {item.Name} list'" |> ignore
                    logger.LogInformation($"Notified: {item.Name}")
                do! Async.Sleep(5000)
            }
            |> Async.StartAsTask
            :> Task // need to convert into the parameter-less task
