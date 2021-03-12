namespace WordsEnglish.Core
[<RequireQualifiedAccess>]
module ListWordsDB =
    open Microsoft.Data.Sqlite
    open FSharp.Data.Dapper
    open System.Threading.Tasks
    [<CLIMutable>]
    type ListWords={
        id: int;
        Name : string;
        TimeToRepiting: System.DateTime
        Created: System.DateTime
        Level : string
        AmountRepetition : int
        IsNotificated: string
    }

   

    let createListWords (listWords: ListWords)=
        (DB.querySingleAsync<int>{
            script "INSERT INTO ListWords (Name,TimeToRepiting, Created, Level, AmountRepetition,IsNotificated) VALUES (@name, @timeToRepeat, @created, @level,@amountRepetition,@isNotificated)"
            parameters (dict 
                    [
                        "name", box listWords.Name; 
                        "timeToRepeat", box listWords.TimeToRepiting;
                        "created", box listWords.Created;
                        "level", box listWords.Level;
                        "amountRepetition", box listWords.AmountRepetition
                        "isNotificated", box listWords.IsNotificated
                    ])
        }|>  Async.StartAsTask).Result
    let getListWords page=
        let pagesize = 5;
        let offset = (page - 1) * pagesize
        (DB.querySeqAsync<ListWords>{
            script "SELECT * FROM ListWords ORDER BY Created DESC LIMIT @pagesize OFFSET @offset  "
            parameters (dict[
                "pagesize", box pagesize;
                "offset", box offset;
            ])

        }|> Async.StartAsTask).Result

    let updateLevel id level time amountRepetition=
       DB.querySingleAsync<int>{
            script " UPDATE ListWords SET Level=@level , TimeToRepiting=@timeToRepeat, AmountRepetition=@amountRepetition, IsNotificated= @status WHERE id=@id "
            parameters (dict[
                ("@id", box id);
                ("@level", box level)
                ("@timeToRepeat", box time)
                ("@amountRepetition", box amountRepetition)
                ("@status", box "false")
            ])
        } |> Async.RunSynchronously
       
    let updateAmountRepetition id amoutRepetition=
        DB.querySingleAsync<int>{
            script " UPDATE ListWords SET AmountRepetition=@amountRepetition WHERE id=@id "
            parameters (dict[
                ("@id", box id);
                ("@amountRepetition", box amoutRepetition)
            ])
        }|> Async.RunSynchronously                        
    let getListsWordsWhichReadyToLearn ()=
        let res=(DB.querySeqAsync<ListWords>{
            script "SELECT * FROM ListWords WHERE TimeToRepiting< @now AND IsNotificated = @status"
            parameters (dict[
                ("@now", box System.DateTime.Now)
                ("@status", box "false")
            ])
            
        }|> Async.RunSynchronously)
        res
    let setNotificateStatus (id: int) (bool : string)=
        let res=(DB.querySingleAsync<int>{
            script "UPDATE ListWords SET IsNotificated= @status WHERE id=@id"
            parameters (dict[
                ("@status", box bool);
                ("@id", box id)
            ])
            
        }|> Async.RunSynchronously)
        res
       