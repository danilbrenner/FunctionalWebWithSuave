#r "../packages/Suave/lib/net40/Suave.dll"
#r "System.Runtime.Serialization.dll"

#load "OpenWeatherMapData.fsx"

open OpenWeatherMapData

open System

open Suave
open Suave.Operators
open Suave.Filters

open Suave.Json
open System.Runtime.Serialization

[<DataContract>]
type ForecastItem = { 
    [<field: DataMember(Name = "date")>]
    date: string
    [<field: DataMember(Name = "temp")>]
    temperature: decimal }

let mapItem (itm:OpenWeatherMapData.WeatherForecastData.List) =
    let dateStr = 
        DateTime(1970,1,1,0,0,0,0,System.DateTimeKind.Utc)
        |> (fun i -> i.AddSeconds(float(itm.Dt)))
        |> (fun i -> i.ToString("o")) 
        //Suave uses the default BCL JSON serializer DataContractJsonSerializer. So I'm using ToString(o) to get iso 8601 date . For more elegant way see next sample. 
    let temperature = itm.Temp.Max - 273.15M //Kelvin -> Celsius
    { date = dateStr; temperature = temperature }

let toResp (ctx:HttpContext) result = 
    let response = { 
        ctx.response with 
            status = HttpCode.HTTP_200.status; 
            content = HttpContent.Bytes (toJson result)
    }
    Some { ctx with response = response}

let getForecast (city:string) (ctx:HttpContext) = async {
    let! weatherFk = OpenWeatherMapData.getData city
    return
        weatherFk.List
        |> Array.map mapItem
        |> toResp ctx
}

let app = 
    GET >=> choose [
        path "/" >=> (Successful.OK "Hi there! I'm a weather sample")
        pathScan "/forecast/%s" getForecast]

startWebServer defaultConfig app