using Toolkit;

// ─────────────────────────────────────────────────────────────────────────────
//  Lesson 2 -- Data Validation and Factory Methods
//
//  Lesson 1 settled who is allowed to change a value once it exists, and the
//  answer was that the door is shut.  Which leaves exactly one moment when a
//  set of dice can be wrong:  the moment it is built.  Today we close that too.
//
//  Notice something before you start:  the compiler rejects `new Dice(...)`
//  anywhere in this file.  Try it.  The constructor is private, so the way to
//  get dice is to ask Dice for them -- through a method somebody wrote on
//  purpose, with rules in it.
//
//  This program is a report card on your Dice type.  Run it now, then run it
//  again after every change.  The output should get less embarrassing.
// ─────────────────────────────────────────────────────────────────────────────

Scene1_TheMuseumOfBadDice();
Scene2_OneDoor();


/// <summary>
/// Scene 1 -- eight sets of dice, several of which the game should have refused.
/// </summary>
static void Scene1_TheMuseumOfBadDice()
{
    Section("1.  The museum of bad dice");

    Exhibit("an ordinary roll", () => Dice.Of(2, 6));
    Exhibit("a roll of zero dice", () => Dice.Of(0, 6));
    Exhibit("negative dice", () => Dice.Of(-3, 6));
    Exhibit("two hundred dice", () => Dice.Of(200, 6));
    Exhibit("a one-sided die", () => Dice.Of(1, 1));
    Exhibit("a zero-sided die", () => Dice.Of(1, 0));
    Exhibit("a die with -4 sides", () => Dice.Of(1, -4));
    Exhibit("a modifier from another game", () => Dice.Of(1, 20, 500));

    Console.WriteLine();
    Console.WriteLine("Every exhibit marked ROLLED is dice your game now has to live with.");
    Console.WriteLine("Of() let them through.  Of() is the only thing that could have stopped them.");
}

/// <summary>
/// Tries to build the dice, then tries to roll them, and reports which of the
/// two it got to.
///
/// Those are deliberately separate.  The question today is whether a bad value
/// gets stopped where the mistake was made, or sails on and detonates
/// somewhere else, later, in a message written by a stranger.
/// </summary>
/// <param name="label">What to call this exhibit.</param>
/// <param name="make">How to build it.</param>
static void Exhibit(string label, Func<Dice> make)
{
    Dice dice;
    try
    {
        dice = make();
    }
    catch (ArgumentException ex)
    {
        Console.WriteLine($"  refused   {label,-30} {ex.Message.Split('(')[0].Trim()}");
        return;
    }

    try
    {
        var rng = new Random(7);
        var rolls = new int[5];
        for (var i = 0; i < 5; i++)
            rolls[i] = dice.Roll(rng);
        Console.WriteLine($"  ROLLED    {label,-30} {dice,-10} -> {string.Join(", ", rolls)}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"  ACCEPTED, THEN CRASHED   {label,-19} {ex.GetType().Name}: {ex.Message.Split('(')[0].Trim()}");
    }
}

/// <summary>
/// Scene 2 -- three callers in three different files, and the one thing they share.
/// </summary>
static void Scene2_OneDoor()
{
    Section("2.  One door");

    // Start with the door that is shut.  Uncomment this line and read the
    // error, then put the comment back:
    //
    //     var sneaky = new Dice(0, 0, 0);
    //
    // CS0122:  'Dice.Dice(int, int, int)' is inaccessible due to its
    // protection level.  The constructor is private, so Dice.cs is the only
    // file that can reach it.  That is what makes the rest of this scene true.

    Console.WriteLine("Three callers, written by three people, in three different files.");
    Console.WriteLine();

    // Caller one:  a config table, the kind a designer edits.
    var fromConfig = new[] { (count: 3, sides: 8), (count: 0, sides: 6), (count: 2, sides: 20) };
    foreach (var (count, sides) in fromConfig)
        Report($"config row {count}d{sides}", () => Dice.Of(count, sides));

    // Caller two:  somebody's helper method, three files away.
    Report("a helper somebody wrote", () => RollForDamage(sides: 6));

    // Caller three:  a loop that builds a whole set at once.
    Report("the fourth die in a loop", () =>
    {
        Dice made = Dice.Of();
        for (var i = 1; i <= 4; i++)
            made = Dice.Of(i, 6);
        return made;
    });

    Console.WriteLine();
    Console.WriteLine("Each of them wrote different code.  Each of them got the same rules,");
    Console.WriteLine("and the one who typed a zero got the same refusal.  All three were");
    Console.WriteLine("spared remembering a limit, because remembering is the part that fails.");
    Console.WriteLine();
    Console.WriteLine("Count the places a rule about dice is written in this program:  one.");
    Console.WriteLine("Change your mind about MaxCount and there is one line to edit.");
}

/// <summary>
/// A helper of the kind that turns up three files from where you are looking.
/// It knows what it wants, and it leaves every limit to Of().
/// </summary>
/// <param name="sides">How many sides the damage die has.</param>
static Dice RollForDamage(int sides) => Dice.Of(2, sides, 1);

/// <summary>Builds it, prints what came back, and says so when it was refused.</summary>
/// <param name="label">What to call this caller.</param>
/// <param name="make">How it builds its dice.</param>
static void Report(string label, Func<Dice> make)
{
    try
    {
        Console.WriteLine($"  built     {label,-28} {make()}");
    }
    catch (ArgumentException ex)
    {
        Console.WriteLine($"  refused   {label,-28} {ex.Message.Split('(')[0].Trim()}");
    }
}

/// <summary>Prints a section heading, padded out to a fixed width.</summary>
static void Section(string title)
{
    Console.WriteLine();
    Console.WriteLine($"── {title} {new string('─', Math.Max(0, 68 - title.Length))}");
}
