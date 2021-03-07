namespace WordsEnglish.Core
[<RequireQualifiedAccess>]
module ListWordsDB =
    open Microsoft.Data.Sqlite
    open FSharp.Data.Dapper
    [<CLIMutable>]
    type ListWords={
        id: int;
        Name : string;
        TimeToRepiting: System.DateTime
        Created: System.DateTime
        Level : string
        AmountRepetition : int

    }

    type ListWords2={
        id: int;
        Name : int;
        TimeToRepiting: int
        Created: int
        Level : int
        AmountRepetition : int

    }
   

    let createListWords (listWords: ListWords)=
        DB.querySingleAsync<int>{
            script "INSERT INTO ListWords (Name,TimeToRepiting, Created, Level, AmountRepetition) VALUES (@name, @timeToRepeat, @created, @level,@amountRepetition)"
            parameters (dict 
                    [
                        "name", box listWords.Name; 
                        "timeToRepeat", box listWords.TimeToRepiting;
                        "created", box listWords.Created;
                        "level", box listWords.Level;
                        "amountRepetition", box listWords.AmountRepetition
                    ])
        }|> Async.RunSynchronously
    let getListWords page=
        let pagesize = 5;
        let offset = (page - 1) * pagesize
        DB.querySeqAsync<ListWords>{
            script "SELECT * FROM ListWords ORDER BY Created DESC LIMIT @pagesize OFFSET @offset  "
            parameters (dict[
                "pagesize", box pagesize;
                "offset", box offset;
            ])

        }|> Async.RunSynchronously
    let updateLevel id level time amountRepetition=
       DB.querySingleAsync<int>{
            script " UPDATE ListWords SET Level=@level , TimeToRepiting=@timeToRepeat, AmountRepetition=@amountRepetition WHERE id=@id "
            parameters (dict[
                ("@id", box id);
                ("@level", box level)
                ("@timeToRepeat", box time)
                ("@amountRepetition", box amountRepetition)
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

       