namespace WordsEnglish.Core
module WordDB =
    open Npgsql.FSharp

    type Word={
        id: int
        Value : string
        Translate: string
        idListWords : int
        Created: System.DateTime
    }

    let createWord (word: Word)=
        DB.getConnectionString
        |> Sql.connect
        |> Sql.query """INSERT INTO "Word" VALUES (DEFAULT,@value, @translate, @idListWords, @created) RETURNING id"""
        |> Sql.parameters [ 
                            ("@value", Sql.text word.Value ); 
                            ("@translate", Sql.text word.Translate);
                            ("@idListWords", Sql.int word.idListWords); 
                            ("@created", Sql.timestamp word.Created)    
        ]
        |> Sql.execute (fun read -> { word with id= read.int "id"}) |> ignore
    
    let getWords (idListWords : int)=
        DB.getConnectionString
        |> Sql.connect
        |> Sql.query """SELECT * FROM "Word" WHERE "idListWords" = @id ORDER BY "Created" DESC """
        |> Sql.parameters[
            ("@id", Sql.int idListWords)
        ]
        |> Sql.execute(fun read -> 
            {
                id = read.int "id";
                Value = read.text "Value";
                Translate = read.text "Translate"
                idListWords = read.int "idListWords"
                Created = (read.timestamp "Created").ToDateTime()
            }
        )
        |> DB.getResult