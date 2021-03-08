

namespace WordsEnglish.Core
    open Microsoft.Data.Sqlite
    open FSharp.Data.Dapper
    open System.IO
    module DB =
    
        let private mkOnDisk () = new SqliteConnection ("Data Source = ./WordsEnglish.Core/wordsEnglish.db;")
        let private connectionF () = SqliteConnection (mkOnDisk())

        
        let querySeqAsync<'R>    = querySeqAsync<'R> (connectionF)
        let querySingleAsync<'R> = querySingleOptionAsync<'R> (connectionF)
        let private restoreDB=
            querySingleAsync<int>{
                script 
                        "CREATE TABLE IF NOT EXISTS ListWords (
                            id               INTEGER PRIMARY KEY AUTOINCREMENT
                                                     NOT NULL
                                                     UNIQUE,
                            Name             STRING,
                            TimeToRepiting   DATE,
                            Created          DATE,
                            Level            STRING,
                            AmountRepetition INTEGER
                        );

                        CREATE TABLE IF NOT EXISTS Word (
                            id          INTEGER  PRIMARY KEY
                                                 UNIQUE
                                                 NOT NULL,
                            Value       TEXT,
                            Translate   TEXT,
                            idListWords INTEGER  REFERENCES ListWords (id),
                            Created     DATETIME
                        );
                        "
                
            }|> Async.RunSynchronously |> ignore
        let checkDatabase () =
            match File.Exists("./WordsEnglish.Core/wordsEnglish.db") with
            | false ->
                connectionF() |> ignore
                restoreDB
            | _ -> ()
        