namespace BagnoDB

    open System

    type Config =
        {
            host: string
            port: int
            user: string option
            password: string option
            authDb: string option
        }
        member this.GetConnectionString () =
            let isEmpty s = String.IsNullOrWhiteSpace s
            match this.password, this.user, this.authDb with
            | None, None, _
            | None, Some _, _
            | Some _, None, _ ->
                sprintf "mongodb://%s:%d" this.host this.port
            | Some pas, Some us, _ when isEmpty pas || isEmpty us ->
                sprintf "mongodb://%s:%d" this.host this.port
            | Some pas, Some us, None ->
                sprintf "mongodb://%s:%s@%s:%d" us pas this.host this.port
            | Some pas, Some us, Some authDb when isEmpty authDb ->
                sprintf "mongodb://%s:%s@%s:%d" us pas this.host this.port
            | Some pas, Some us, Some authDB ->
                sprintf "mongodb://%s:%s@%s:%d/%s" us pas this.host this.port authDB