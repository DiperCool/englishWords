namespace WordsEnglish.Core
[<RequireQualifiedAccess>]
module ListWordsDB =
    open Npgsql.FSharp

    type ListWords={
        id: int;
        Name : string;
        TimeToRepiting: System.DateTime
        Created: System.DateTime
    }
    let private map = 
        fun (read: RowReader) ->
            {
                id = read.int "id";
                Name = read.text "Name";
                TimeToRepiting = (read.timestamp "TimeToRepiting").ToDateTime()
                Created = (read.timestamp "Created").ToDateTime()
            }
    let createListWords (listWords: ListWords)=
        DB.getConnectionString
        |> Sql.connect
        |> Sql.query """INSERT INTO "ListWords" VALUES (DEFAULT,@name, @timeToRepeat, @created) RETURNING id"""
        |> Sql.parameters [ 
                            ("@name", Sql.text listWords.Name ); 
                            ("@timeToRepeat", Sql.timestamp listWords.TimeToRepiting);
                            ("@created", Sql.timestamp listWords.Created); ]
        |> Sql.execute (fun read -> { listWords with id= read.int "id"}) |> ignore
    let getListWords page=
        let pagesize = 5;
        let offset = (page - 1) * pagesize
        let res= DB.getConnectionString
                |> Sql.connect
                |> Sql.query """SELECT * FROM "ListWords" ORDER BY "Created" DESC LIMIT @pagesize OFFSET @offset """
                |> Sql.parameters[
                            ("@pagesize", Sql.int pagesize);
                            ("@offset", Sql.int offset)
                        ]
                |> Sql.execute map
        DB.getResult res
       