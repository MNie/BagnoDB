module BagnoDB.Filtering

    open System
    open MongoDB.Driver
    open System.Linq.Expressions

    type Operation =
        | Eq
        | Greater
        | GreaterOrEqual
        | Less
        | LessOrEqual
        | Not

    type internal InternalFilter<'TModel, 'TField> =
        {
            operation: Operation
            field: Expression<Func<'TModel, 'TField>>
            value: 'TField
        }

    type Filter<'TModel> =
        {
            definition: FilterDefinition<'TModel>
        }
        static member (&&&) (first, second) =
            { definition = first.definition &&& second.definition }

        static member (|||) (first, second) =
            { definition = first.definition ||| second.definition }

    type Expression() =
        static member Map<'TItem, 'TField>(e: Expression<Func<'TItem, 'TField>>) = e

    module Filter =
        let internal create<'TModel, 'TField> fOp =
            let filter = FilterDefinitionBuilder<'TModel>()

            let translated =
                match fOp.operation with
                | Eq -> filter.Eq(fOp.field, fOp.value)
                | Greater -> filter.Gt<'TField>(fOp.field, fOp.value)
                | GreaterOrEqual -> filter.Gte(fOp.field, fOp.value)
                | Less -> filter.Lt(fOp.field, fOp.value)
                | LessOrEqual -> filter.Lte(fOp.field, fOp.value)
                | Not -> filter.Not(filter.Eq(fOp.field, fOp.value))

            { definition = translated }
        let eq f v =
            { operation = Eq; field = f; value = v }
            |> create
        let gt f v =
            { operation = Greater; field = f; value = v }
            |> create
        let gte f v =
            { operation = GreaterOrEqual; field = f; value = v }
            |> create
        let lt f v =
            { operation = Less; field = f; value = v }
            |> create
        let lte f v =
            { operation = LessOrEqual; field = f; value = v }
            |> create
        let not f v =
            { operation = Not; field = f; value = v }
            |> create

        let ``and`` ``new`` previous =
            { previous with definition = previous.definition &&& ``new``.definition }

        let ``or`` ``new`` previous =
            { previous with definition = previous.definition ||| ``new``.definition }

        let empty<'TModel> = { definition = FilterDefinitionBuilder<'TModel>().Empty }