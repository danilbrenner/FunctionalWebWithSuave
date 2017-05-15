#r "../packages/DotLiquid/lib/net451/DotLiquid.dll"
#r "../packages/Suave/lib/net40/Suave.dll"
#r "../packages/Suave.DotLiquid/lib/net40/Suave.DotLiquid.dll"

open Suave
open Suave.Operators
open Suave.Filters
open Suave.DotLiquid
open DotLiquid

type Model =
  { title : string }

setTemplatesDir "./templates"

let o = { title = "Functional Meetup" }

let app =
  choose
    [ GET >=> choose
        [ path "/" >=> page "index.liquid" o ]]

startWebServer defaultConfig app