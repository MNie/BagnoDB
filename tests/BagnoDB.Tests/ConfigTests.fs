module ConfigTests
    open Xunit
    open Swensen.Unquote
    open BagnoDB.Configuring

    type ConnectionStringTests () =
        [<Fact>]
        let ``when password is empty, built con string should equal mongodb://host:0`` () =
            let config = { Host = "host"; Port = 0; User = Some "user"; Password = None }
            let result = config.GetConnectionString ()

            result =! "mongodb://user:pass@host:0"

        [<Fact>]
        let ``when user is empty, built con string should equal mongodb://host:0`` () =
            let config = { Host = "host"; Port = 0; User = None; Password = Some "pass" }
            let result = config.GetConnectionString ()

            result =! "mongodb://user:pass@host:0"

        [<Fact>]
        let ``when user and pass are empty, built con string should equal mongodb://host:0`` () =
            let config = { Host = "host"; Port = 0; User = None; Password = None }
            let result = config.GetConnectionString ()

            result =! "mongodb://host:0"

        [<Fact>]
        let ``when pass and user are provided, built con string should equal mongodb://user:pass@host:0`` () =
            let config = { Host = "host"; Port = 0; User = Some "user"; Password = Some "pass" }
            let result = config.GetConnectionString ()

            result =! "mongodb://user:pass@host:0"


