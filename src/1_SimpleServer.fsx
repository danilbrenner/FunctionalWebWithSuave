#r "../packages/Suave/lib/net40/Suave.dll"

open Suave

Successful.OK "Hello Suave!"
|> startWebServer defaultConfig