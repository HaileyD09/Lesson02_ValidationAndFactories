using System.Text.RegularExpressions;

namespace Toolkit;

/// <summary>
/// A set of dice:  Count dice with Sides sides, plus a flat Modifier.
///
/// This is your Dice from Lesson 1, with the change you made -- the three
/// properties are init-only, so a set of dice is fixed once it exists.
///
/// Which leaves exactly one moment when dice could be wrong:  the moment they
/// are built.  So this version closes that moment too.  The constructor is
/// PRIVATE, and this file is the only place that can reach it.  The one way to
/// make dice is a factory method below, and that is where the rules live.
///
/// One door in.  One place the rules are written.  That is the whole design.
/// </summary>
public record Dice
{
    // Two different forms below, and the difference is on purpose.  The form
    // you choose is a claim about how permanent the number is.

    // MinSides is a `const`, because it is structural.  A random result needs
    // at least two faces to choose between -- in this game, in a board game
    // about trains, in any game anyone will ever write.  That number is
    // permanent, so the compiler may as well bake it in.
    //
    // Worth knowing while you are here:  a const is ALREADY static.  You reach
    // it as Dice.MinSides, straight off the type, and writing
    // `public static const` is a compile error -- CS0504.  If you learned
    // `static final` somewhere else, your fingers will try it exactly once.

    /// <summary>The fewest sides a die may have.  Two faces is the floor for randomness.</summary>
    public const int MinSides = 2;

    // The next three are static properties, because they are house rules
    // as opposed to structure.  They are this table's numbers, and another game
    // would pick different ones and still be playing with dice.
    //
    // Written this way they can be changed, read from a config, or moved out
    // of this type entirely.  Hold that thought; Lesson 3 moves them.

    /// <summary>The most dice one roll may use.</summary>
    public static int MaxCount => 100;

    /// <summary>The most sides a die may have.</summary>
    public static int MaxSides => 1000;

    /// <summary>The largest modifier, in either direction, a roll may carry.</summary>
    public static int MaxModifier => 20;

    // ─────────────────────────────────────────────────────────────────────────
    //  What a set of dice IS
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>How many dice are rolled.  The 2 in "2d6+3".</summary>
    public int Count { get; init; }

    /// <summary>How many sides each die has.  The 6 in "2d6+3".</summary>
    public int Sides { get; init; }

    /// <summary>A flat amount added to the total after rolling.  The +3 in "2d6+3".</summary>
    public int Modifier { get; init; }

    /// <summary>
    /// Private, and that is the point.
    ///
    /// It puts the three numbers where they go and trusts them completely.  It
    /// can afford that, because this file is the only place that can reach it.
    /// The one thing that CAN call it is the factory method twenty lines below,
    /// and that is where the checking happens.
    /// </summary>
    private Dice(int count, int sides, int modifier)
    {
        Count = count;
        Sides = sides;
        Modifier = modifier;
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  The factory method -- the only way in
    // ─────────────────────────────────────────────────────────────────────────
    public static readonly int[] _validSides =
        [2, 4, 6, 8, 10, 12, 20, 100];

    /// <summary>
    /// Builds a set of dice, or refuses to.  The only door;  every set of dice
    /// in the entire program comes through here.
    /// </summary>
    /// <param name="count">How many dice.</param>
    /// <param name="sides">How many sides on each.</param>
    /// <param name="modifier">A flat amount added to the total.</param>
    public static Dice Of(int count = 1, int sides = 6, int modifier = 0)
    {
        // A guard clause:  check it, complain immediately, get out.  The
        // alternative is to accept it, carry on, and fail somewhere else an
        // hour later, with the origin of the bad value long since lost.
        if (count < 1 || count > MaxCount)
            throw new ArgumentOutOfRangeException(
                nameof(count), count, $"A roll uses between 1 and {MaxCount} dice.");

        if (!_validSides.Contains(sides))
        {
            throw new ArgumentOutOfRangeException(
                nameof(sides), sides, $"You can't make {sides} sided dice :O");
        }

        if (Math.Abs(modifier) > 100)
        {
            throw new ArgumentOutOfRangeException(
                nameof(modifier), modifier, $"{modifier} must be less than 100 and greater than -100.");
        }

    // TODO (Step 2): `sides` is wide open.  One side makes a token, zero
    //                sides makes a paradox, and -4 sides makes
    //                Random.Next() throw from deep inside the standard
    //                library, which is a genuinely miserable way to find
    //                out.  Use MinSides and MaxSides.

    // TODO (Step 3): `modifier` is wide open too.  Use MaxModifier.
    //                Decide for yourself whether a negative modifier is
    //                legal -- it is, but how negative?

    return new Dice(count, sides, modifier);
    }
    //hw 9/18
    public static Dice Damage() => Of(1, 6, 0);
    public static Dice CheckYourLuck() => Of(1, 20, 0);
    public static Dice Movement() => Of(2, 8, 1);
    // ─────────────────────────────────────────────────────────────────────────
    //  What a set of dice DOES
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Rolls the dice.  Sums Count rolls of a Sides-sided die, then adds Modifier.
    /// </summary>
    /// <param name="rng">Where the randomness comes from.  Seed it to repeat it.</param>
    public int Roll(Random rng)
    {
        var sum = Modifier;
        for (var i = 0; i < Count; i++)
            sum += rng.Next(1, Sides + 1);
        return sum;
    }

    /// <summary>The smallest total these dice can produce.</summary>
    public int Minimum => Count + Modifier;

    /// <summary>The largest total these dice can produce.</summary>
    public int Maximum => Count * Sides + Modifier;

    /// <summary>"2d6+3", the way a player would write it.</summary>
    public override string ToString()
    {
        var mod = Modifier switch
        {
            > 0 => $"+{Modifier}",
            < 0 => $"{Modifier}",
            _ => ""
        };
        return $"{Count}d{Sides}{mod}";
    }
    /*
    using System.Text.RegularExpressions;
using System.Xml;

namespace Toolkit;

//A Uno designer would want Suit to be { get; set; } so a wild card can change color.
// Fixed for good: number
// Can change: suit
// What breaks if reversed: a card's rank changing mid-game would make scores unpredictable



public record Suit(string Name = "Clubs", string Color = "Black")
{
    public static Suit Clubs => new Suit("Clubs", "Black");
    public static Suit Hearts => new Suit("Hearts", "Red");
    public static Suit Diamonds => new Suit("Diamonds", "Red");
    public static Suit Spades => new Suit("Spades", "Black");
}

public record Card
{
    public int Value { get; init; }
    public Suit Suit { get; init; }
    public bool IsFaceUp { get; init; } 
    
    private Card(int value, Suit suit, bool isFaceUp)
    {
        Value = value;
        Suit = suit;
        IsFaceUp = isFaceUp;
    }

    //hw 9/17
    public static Card Of(int value, Suit suit, bool isFaceUp = true)
    {
        if (value < 1 || value > 14)
            throw new ArgumentOutOfRangeException(
                nameof(value), value, $"A card has a number between 1 and 14.");

        string[] validSuits = ["Clubs", "Hearts", "Diamonds", "Spades"];
        if (!validSuits.Contains(suit.Name))
            throw new ArgumentOutOfRangeException(
                nameof(suit), suit, $"A suit can only be diamonds, hearts, clubs, or spades.");

        return new Card(value, suit, isFaceUp);

    }
    
    public static List<Card> FullDeck()
    {
        Suit[] suits = [Suit.Clubs, Suit.Hearts, Suit.Diamonds, Suit.Spades];
        List<Card> deck = new();

        for (int i = 1; i <= 13; i++)
        {
            foreach (Suit suit in suits)
            {
                deck.Add(Card.Of(i, suit));
            }
        }

        Console.WriteLine();
        return deck;
    }
    
    public static bool TryParse(string input, out Dice? result)
    {
        result = null;
        var match = Regex.Match(input.Trim(), @"^(\d+)?d(\d+)([+-]\d+)?$");
    
        if (!match.Success)
            return false;
    
        int count = match.Groups[1].Success ? int.Parse(match.Groups[1].Value) : 1;
        int sides = int.Parse(match.Groups[2].Value);
        int modifier = match.Groups[3].Success ? int.Parse(match.Groups[3].Value) : 0;
    
        try
        {
            result = Of(count, sides, modifier);
            return true;
        }
        catch (ArgumentOutOfRangeException)
        {
            result = null;
            return false;
        }
    }

}
*/
    public static bool TryParse(string input, out Dice? result)
    {
        result = null;
        var match = Regex.Match(input.Trim(), @"^(\d+)?d(\d+)([+-]\d+)?$");

        if (!match.Success)
            return false;

        int count = match.Groups[1].Success ? int.Parse(match.Groups[1].Value) : 1;
        int sides = int.Parse(match.Groups[2].Value);
        int modifier = match.Groups[3].Success ? int.Parse(match.Groups[3].Value) : 0;

        try
        {
            result = Of(count, sides, modifier);
            return true;
        }
        catch (ArgumentOutOfRangeException)
        {
            result = null;
            return false;
        }
    }
} // closes record Dice





