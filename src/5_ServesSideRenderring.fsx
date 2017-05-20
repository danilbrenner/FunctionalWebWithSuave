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

let getModel text = 
  { title = text }

setTemplatesDir "./templates"

page "index.liquid" (getModel "Functional Meetup")
|> startWebServer defaultConfig 