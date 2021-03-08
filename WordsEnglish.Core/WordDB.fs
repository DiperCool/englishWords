namespace WordsEnglish.Core
module WordDB =
    
    [<CLIMutable>]
    type Word={
        id: int
        Value : string
        Translate: string
        idListWords : int
        Created: System.DateTime
    }

    let createWord (word: Word)=
        DB.querySingleAsync<int>{
            script "INSERT INTO Word (Value, Translate, idListWords, Created) VALUES (@value, @translate, @idListWords, @created)"
            parameters (dict[
                ("value", box word.Value ); 
                ("translate", box word.Translate);
                ("idListWords", box word.idListWords); 
                ("created", box word.Created) 
            ])
        }|> Async.RunSynchronously
    
    let getWords (idListWords : int)=
        DB.querySeqAsync<Word>{
            script "SELECT * FROM Word WHERE idListWords = @id ORDER BY Created DESC"
            parameters (dict[
                ("id", box idListWords)
            ])
        }|> Async.RunSynchronously |> List.ofSeq
    let countWords id=
        DB.querySingleAsync<int>{
            script "SELECT COUNT(*) FROM Word WHERE idListWords=@id"
            parameters (dict[
                "id", box id
            ])
        }|>Async.RunSynchronously
