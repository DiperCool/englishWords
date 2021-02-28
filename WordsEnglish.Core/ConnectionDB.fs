

namespace WordsEnglish.Core
    open Npgsql.FSharp
    module DB =
        let getConnectionString =
                Sql.host "localhost"
                |> Sql.database "EnglishWords"
                |> Sql.username "postgres"
                |> Sql.password ""
                |> Sql.port 5432
                |> Sql.formatConnectionString
        let getResult (result: Result<'T list, exn>)=
            match result with
                | Ok res-> res
                | Error error -> raise error
