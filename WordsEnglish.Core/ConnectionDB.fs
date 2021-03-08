

namespace WordsEnglish.Core
    open Microsoft.Data.Sqlite
    open FSharp.Data.Dapper
    open System.IO
    open System
    module DB =
        let private dbpath =
            let appData = Environment.GetFolderPath Environment.SpecialFolder.LocalApplicationData
            let folder =  Path.Combine(appData, "WordsEnglish")
            match Directory.Exists folder with
            | true -> ()
            | false -> 
                Directory.CreateDirectory folder |> ignore
            let path = Path.Combine(folder, "wordsEnglish.db")
            sprintf "Data Source = %s;" path
        let private mkOnDisk () = new SqliteConnection (dbpath)
        let private connectionF () = SqliteConnection (mkOnDisk())

        
        let querySeqAsync<'R>    = querySeqAsync<'R> (connectionF)
        let querySingleAsync<'R> = querySingleOptionAsync<'R> (connectionF)
        let private restoreDB ()=
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
        restoreDB ()