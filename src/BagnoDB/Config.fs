module BagnoDB.Configuring

    open System

    type Config =
        {
            Host: string
            Port: int
            User: string option
            Password: string option
        }
        member this.GetConnectionString () =
            let isEmpty s = String.IsNullOrWhiteSpace s
            match this.Password, this.User with
            | None, None
            | None, Some _
            | Some _, None ->
                sprintf "mongodb://%s:%d" this.Host this.Port
            | Some pas, Some us when isEmpty pas || isEmpty us ->
                sprintf "mongodb://%s:%d" this.Host this.Port
            | Some pas, Some us ->
                sprintf "mongodb://%s:%s@%s:%d" us pas this.Host this.Port