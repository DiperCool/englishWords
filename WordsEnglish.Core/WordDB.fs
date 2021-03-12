namespace WordsEnglish.Core
module WordDB =
    
    [<CLIMutable>]
    type Word={
        id: int
        Value : string
        Translate: string
        idListWords : int
        Created: System.DateTime
        guidMedia: string
    }

    let createWord (word: Word)=
        DB.querySingleAsync<int>{
            script "INSERT INTO Word (Value, Translate, idListWords, Created,guidMedia) VALUES (@value, @translate, @idListWords, @created,@guidMedia)"
            parameters (dict[
                ("value", box word.Value ); 
                ("translate", box word.Translate);
                ("idListWords", box word.idListWords); 
                ("created", box word.Created);
                ("guidMedia", box word.guidMedia)
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
    let setNewGuid id guid= 
        DB.querySingleAsync<int>{
            script "UPDATE Word SET guidMedia=@guidMedia WHERE id=@id"
            parameters (dict[
                "@id", box id
                "@guidMedia", box guid
            ])
        }|>Async.RunSynchronously
