module ConfigTests
    open Xunit
    open Swensen.Unquote
    open BagnoDB

    type ConnectionStringTests () =
        [<Fact>]
        let ``when password is empty, built con string should equal mongodb://host:0`` () =
            let config = { host = "host"; port = 0; user = Some "user"; password = None }
            let result = config.GetConnectionString ()

            result =! "mongodb://user:pass@host:0"

        [<Fact>]
        let ``when user is empty, built con string should equal mongodb://host:0`` () =
            let config = { host = "host"; port = 0; user = None; password = Some "pass" }
            let result = config.GetConnectionString ()

            result =! "mongodb://user:pass@host:0"

        [<Fact>]
        let ``when user and pass are empty, built con string should equal mongodb://host:0`` () =
            let config = { host = "host"; port = 0; user = None; password = None }
            let result = config.GetConnectionString ()

            result =! "mongodb://host:0"

        [<Fact>]
        let ``when pass and user are provided, built con string should equal mongodb://user:pass@host:0`` () =
            let config = { host = "host"; port = 0; user = Some "user"; password = Some "pass" }
            let result = config.GetConnectionString ()

            result =! "mongodb://user:pass@host:0"


