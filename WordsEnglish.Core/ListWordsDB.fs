namespace WordsEnglish.Core
[<RequireQualifiedAccess>]
module ListWordsDB =
    open Npgsql.FSharp

    type ListWords={
        id: int;
        Name : string;
        TimeToRepiting: System.DateTime
        Created: System.DateTime
        Level : string

    }
    let private map = 
        fun (read: RowReader) ->
            {
                id = read.int "id";
                Name = read.text "Name";
                TimeToRepiting = (read.timestamp "TimeToRepiting").ToDateTime()
                Created = (read.timestamp "Created").ToDateTime()
                Level = read.string "Level"
            }
    let createListWords (listWords: ListWords)=
        DB.getConnectionString
        |> Sql.connect
        |> Sql.query """INSERT INTO "ListWords" VALUES (DEFAULT,@name, @timeToRepeat, @created, @level) RETURNING id"""
        |> Sql.parameters [ 
                            ("@name", Sql.text listWords.Name ); 
                            ("@timeToRepeat", Sql.timestamp listWords.TimeToRepiting);
                            ("@created", Sql.timestamp listWords.Created);
                            ("@level", Sql.text listWords.Level)
                        ]
        |> Sql.execute (fun read -> { listWords with id= read.int "id"}) |> ignore
    let getListWords page=
        let pagesize = 5;
        let offset = (page - 1) * pagesize
        DB.getConnectionString
                |> Sql.connect
                |> Sql.query """SELECT * FROM "ListWords" ORDER BY "Created" DESC LIMIT @pagesize OFFSET @offset """
                |> Sql.parameters[
                            ("@pagesize", Sql.int pagesize);
                            ("@offset", Sql.int offset)
                        ]
                |> Sql.execute map
                |> DB.getResult
    let updateLevel id level time=
        DB.getConnectionString
                |> Sql.connect
                |> Sql.query """ UPDATE "ListWords" SET "Level"=@level , "TimeToRepiting"=@timeToRepeat WHERE "id"=@id """
                |> Sql.parameters[
                            ("@id", Sql.int id);
                            ("@level", Sql.text level)
                            ("@timeToRepeat", Sql.timestamp time)
                        ]
                |> Sql.execute map
                |> DB.getResult

       