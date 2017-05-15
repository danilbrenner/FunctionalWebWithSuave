#r "../packages/Suave/lib/net40/Suave.dll"

open Suave
open Suave.Http

open System.Text

//type WebPart = HttpContext -> Async<HttpContext option>

let myWebPart (ctx:HttpContext) = async {
    let helloString = "Hello WebPart"
    let bytes = Encoding.UTF8.GetBytes helloString
    let response = { 
        ctx.response with 
            status = HttpCode.HTTP_200.status; 
            content = HttpContent.Bytes bytes
    }
    return Some { ctx with response = response}
}

startWebServer defaultConfig myWebPart