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

let getForecast city = async {
    let! weatherFk = OpenWeatherMapData.getData city
    return
        weatherFk.List
        |> Array.map mapItem
}

let mapJson<'T> (o : Async<'T>) (ctx: HttpContext) = async {
    let! ob = o
    let bytes = 
        JsonConvert.SerializeObject ob
        |> Encoding.Default.GetBytes
    
    let response = {
        ctx.response with
            status = HttpCode.HTTP_200.status;
            content = HttpContent.Bytes bytes
    }
    return Some { ctx with response = response}
}

let app =
    choose [
        GET >=> choose [
            path "/" >=> (Successful.OK "Hi there! I'm a weather sample")
            pathScan "/forecast/%s" (mapJson << getForecast) ]]

startWebServer defaultConfig app