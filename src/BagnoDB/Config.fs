namespace BagnoDB

    open System

    type Config =
        {
            host: string
            port: int
            user: string option
            password: string option
        }
        member this.GetConnectionString () =
            let isEmpty s = String.IsNullOrWhiteSpace s
            match this.password, this.user with
            | None, None
            | None, Some _
            | Some _, None ->
                sprintf "mongodb://%s:%d" this.host this.port
            | Some pas, Some us when isEmpty pas || isEmpty us ->
                sprintf "mongodb://%s:%d" this.host this.port
            | Some pas, Some us ->
                sprintf "mongodb://%s:%s@%s:%d" us pas this.host this.port