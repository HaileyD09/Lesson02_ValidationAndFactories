# Lesson 2 - Data Validation and Factory Methods

```
dotnet run
```

`Program.cs` is a report card on your `Dice` type.  Run it now.  Run it again
after every change.  The output should get less embarrassing.

| File | What it is |
|---|---|
| `Dice.cs` | Your dice from Lesson 1 -- now with a private constructor and one factory method. |
| `Program.cs` | Two scenes.  **Leave these alone**, apart from the one line Scene 2 asks you to uncomment. |

## Where we left off

Lesson 1 ended with dice that are fixed the moment they are built.  That closed off every way a set of
dice could go wrong, save one.

If a value can only ever be set while the object is being built, then the moment
it is built is the *only* moment it can be wrong.  That's a much better problem
than the one you started with, and it's today's problem.

## One door in

Open `Dice.cs` and look at the constructor.  It's **private**.

```csharp
private Dice(int count, int sides, int modifier)
{
    Count = count; Sides = sides; Modifier = modifier;
}
```

It puts the three numbers where they go and trusts them completely.  It can
afford that, because this file is the only place that can reach it.  Try writing
`new Dice(0, 0)` in `Program.cs` -- the compiler rejects it.

One thing *can* call it, and that is the **factory method** on `Dice` itself:

```csharp
Dice.Of(2, 6, 3)        // the only door -- this is where the rules live
```

That is the whole public surface for making dice, and it is the point.  Scene 2
has three callers who work in three different files, and every one of them ends
up at `Of()` whether they meant to or not.

> A constructor gets one name and has to accept whatever it's given.
> A factory gets to refuse.

## Where the rule lives

A rule written once stays in sync with itself for free.  A rule written twice is
one rule plus a future bug, because somebody will change one copy.

That is the argument for the private constructor, and it is worth being precise
about what it buys:  every caller, including the ones written next year by
somebody who has only ever seen the call site, goes through `Of()`.  Politeness
and convention would leave that optional.  The compiler makes it a fact.

## Normalize, or reject?

A value can be wrong in two different ways, and they want different answers.

- **Formatting is noise.**  A name typed as `" Goblin "`, `"goblin"` and
  `"GOBLIN"` is one claim typed by three people.  Clean it up quietly;
  everything meaningful survives.
- **Meaning is a claim.**  `Dice.Of(0, 6)` says something false.  Refuse it.

`Dice` is all claims today -- three numbers, three rules, three refusals.  The
noise half turns up the moment a value comes from text, and that gets a lesson
of its own.  Keep the distinction in your head now;  you will need it.

The line between them can be genuinely hard to find, and *you* have to put it
somewhere.

## Syntax you may be meeting for the first time

```csharp
throw new ArgumentOutOfRangeException(nameof(value), value, "message");
nameof(value)                     // the identifier's name, as text
public const int MinSides = 2;    // fixed at compile time
public static int MaxCount => 100;// computed every time it's asked for
```

### Why `Dice` uses two different forms for its limits

Look at the top of `Dice.cs`.  `MinSides` is a `const`.  The other three are
`static` properties.  That is a claim about how permanent each number is.

| | Form | Because |
|---|---|---|
| `MinSides` | `public const int MinSides = 2;` | **Structural.**  A random result needs at least two faces to choose between, in this game or any other.  That number is permanent. |
| `MaxCount`, `MaxSides`, `MaxModifier` | `public static int MaxCount => 100;` | **House rules.**  This table's numbers.  Another game picks different ones and is still playing with dice. |

The form you pick tells the next reader which kind of number they're looking at.
That's worth more than the two characters it costs you.

### Three things worth knowing about `const`

1. **A `const` is already `static`.** You reach it as `Dice.MinSides`, straight
   off the type.  If you learned `static final` somewhere else, your fingers
   will try `public static const` exactly once -- that's **CS0504**.
2. **`public static int MaxCount => 100;` is a property.**  It's computed each
   time it's asked for, which is why it *needs* the `static` keyword that a
   `const` carries on its own.
3. **Only a `const` can appear in a pattern.**  The guards in `Of()` could have
   been written as patterns -- `count is >= 1 and <= MaxCount` -- which is
   idiomatic modern C#. A pattern requires a `const`, and `MaxCount` is a
   property.  Try it and you'll get **CS9135**, *"a constant value is expected."*

Go break number 3 on purpose and read the error.  Knowing which compiler errors
are real design rules and which are just syntax is worth five minutes.

## Today's steps

1. Run it.  Read Scene 1.  Which exhibit worries you most, and why?  For each
   `ROLLED` line, finish:  *"Six months from now this is a bug report that
   says ______."*
2. In `Of()`, `sides` is wide open.  Give it a rule, next to the `count` guard that's
   already written.  Use `MinSides` and `MaxSides`.  Re-run.
3. `modifier` is wide open too.  Give it a rule, using `MaxModifier`.  Decide for
   yourself whether a negative modifier is legal -- it is, but *how* negative?
4. Scene 2.  Uncomment the `new Dice(0, 0, 0)` line, read the error, put the
   comment back.  **CS0122** is the reason the rest of that scene is true.
5. Count what you touched.  Read Scene 2's three callers and count how many of
   them mention a limit.  Then answer:  *MaxCount changes to 50 tomorrow -- how
   many lines do you edit?*
6. Cards -- the challenge.  Your own `Card.cs`, carried from Lesson 1.

Between steps 2 and 5, notice how little you had to touch.  You wrote two rules
in one method, and all three of Scene 2's callers got them while staying silent
about the limits.

## Two arguments worth having

Both are genuinely open.  Pick a side and be able to defend it.

- **Is `1d1` a broken die or a useful one?**  It always returns 1.  Step 2 asks
  you to refuse it, and `MinSides = 2` is the reason given.  Somebody will point
  out that a die that always rolls 1 is a perfectly good way to say "one damage,
  every time."  Decide whether that's a die or a number wearing a costume.
- **Is `1d20-50` valid?**  It rolls.  Its best possible result is -30.
  Is that the `Dice` type's problem, or the game's?

## Done for today?

> Point at one rule you wrote.  Say whether it protects against a bug in your
> code or bad data from outside it -- and show me where it lives *because* of that.
