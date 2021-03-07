namespace WordsEnglish.Core
    open System
    open FSharp.Json
    open System.Net.Http
    module TranslateHelper=

        type Response={
            translatedText: string
        }

        type Json2={
            responseData : Response
        }

        let getTranslate (text:string)=
            let httpClient= new HttpClient()
            let rqstMsg= new HttpRequestMessage();
            rqstMsg.Method <- HttpMethod.Get;
            rqstMsg.RequestUri <- new Uri($"https://translated-mymemory---translation-memory.p.rapidapi.com/api/get?langpair=en|ru&q={text}");
            rqstMsg.Headers.Add("x-rapidapi-key", "40e09c54f1msh821cf3df80dab34p1f3534jsnfcbfe63f5cbf")
            rqstMsg.Headers.Add("x-rapidapi-host", "translated-mymemory---translation-memory.p.rapidapi.com")
            async{
                let! response= 
                        httpClient.SendAsync(rqstMsg) |>Async.AwaitTask
                let! body = response.Content.ReadAsStringAsync() |> Async.AwaitTask
                let deserialized = Json.deserialize<Json2> body

                return match response.IsSuccessStatusCode with
                | true -> deserialized.responseData.translatedText
                | false -> failwith body
            }

