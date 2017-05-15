#r "../packages/Suave/lib/net40/Suave.dll"
#r "../packages/Newtonsoft.Json/lib/net45/Newtonsoft.Json.dll"

#load "OpenWeatherMapData.fsx"

open OpenWeatherMapData

open System

open Suave
open Suave.Operators
open Suave.Filters

open System.Text
open Newtonsoft.Json

type ForecastItem = { 
    date: DateTime
    temperature: decimal }

let mapItem (itm:OpenWeatherMapData.WeatherForecastData.List) =
    let date = 
        DateTime(1970,1,1,0,0,0,0,System.DateTimeKind.Utc)
        |> (fun i -> i.AddSeconds(float(itm.Dt)))
    let temperature = itm.Temp.Max - 273.15M //Unit Default: Kelvin
    { date = date; temperature = temperature }

let toJson<'T> (o: 'T) =
    JsonConvert.SerializeObject o
    |> Encoding.Default.GetBytes

let toResp (ctx: HttpContext) result = 
    let response = { 
        ctx.response with 
            status = HttpCode.HTTP_200.status; 
            content = HttpContent.Bytes (toJson result)
    }
    Some { ctx with response = response}

let mapJson<'T> (o : Async<'T>) (ctx: HttpContext) = async {
    let! ob = o
    return (toResp ctx ob)
}

let getForecast city = async {
    let! weatherFk = OpenWeatherMapData.getData city
    return
        weatherFk.List
        |> Array.map mapItem
}

let app = 
    choose [
        GET >=> choose [
            path "/" >=> (Successful.OK "Hi there! I'm a weather sample")
            pathScan "/forecast/%s" (mapJson << getForecast) ]]

startWebServer defaultConfig app