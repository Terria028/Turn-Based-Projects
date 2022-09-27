namespace TurnBasedFSharp
open System.Threading
open System

type States =
    | IdleState = 0
    | TurnAState = 1
    | TurnBState = 2
    | FinalState = 3

type Arena(contestA: Entity, contestB: Entity) =
    let contestA = contestA
    let contestB = contestB
    let mutable states = States.IdleState

    member this.Fight() =
        let randomized = Utils.GenerateRandom 2
        states <- 
            match randomized with 
                | 1 -> States.TurnAState 
                | _ -> States.TurnBState
        let mutable turn = 1
        while states <> States.FinalState do
            let (aDamage, bDamage) = (contestA.AssignAttackDamage 50, contestB.AssignAttackDamage 50) 
            match states with
                | States.TurnAState ->
                    contestA.DealDamageTo(contestB, aDamage)
                    this.DisplayHealth()
                    Console.WriteLine("============Turn {0}===============", turn)
                    if contestB.Hp > 0 then
                        Console.WriteLine("{0} Turn!", contestB.Name)
                    states <- States.TurnBState
                | States.TurnBState ->
                    contestB.DealDamageTo(contestA, bDamage)
                    this.DisplayHealth()
                    Console.WriteLine("============Turn {0}===============", turn)
                    if contestA.Hp > 0 then
                        Console.WriteLine("{0} Turn!", contestA.Name)
                    states <- States.TurnAState
                | _ -> printfn "Done!" 
            
            if contestA.Hp < 1 then
                states <- States.FinalState
                Console.WriteLine("{0} Wins!", contestB.Name)
            elif contestB.Hp < 1 then
                states <- States.FinalState
                Console.WriteLine("{0} Wins!", contestA.Name)
            else
                turn <- turn + 1
                Thread.Sleep(1000);
        this.Prompt()

                    
    
    member _.DisplayHealth(): unit =
        Console.WriteLine("{0} has {1} health", contestA.Name, contestA.Hp)
        Console.WriteLine("{0} has {1} health", contestB.Name, contestB.Hp)

    member this.Prompt() =
        printfn "Type 0 or above to continue the game, type -1 to exit!"
        let parsed = Console.ReadLine()
        let (success, result) = Int32.TryParse (parsed)
        if success then
            if result > -1 then
                contestA.Hp <- 100
                contestB.Hp <- 100
                this.Fight()
        else
            printfn "Not a valid number!, exiting the program"        