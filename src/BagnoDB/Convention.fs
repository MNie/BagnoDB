namespace BagnoDB

    open MongoDB.Bson.Serialization.Conventions

    type Conventions =
        {
            members: IConvention list
        }

    module Conventions =
        let create = { members = [] }
        let add ``new`` conv =
            { conv with members = ``new``::conv.members }

        let build name conv =
            let pack = ConventionPack()
            conv.members |> List.iter (fun mem -> pack.Add mem)
            ConventionRegistry.Register(name, pack, (fun _ -> true))