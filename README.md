# FIFA Tournament Simulation (C#)

A console-based football (soccer) tournament simulation built in C# that models
repeated tournaments using Elo-style skill ratings and performance-based match
outcomes. The project focuses on simulation design, algorithmic decision-making,
and deterministic tournament structure rather than user interface development.

## Overview

This application simulates a tournament consisting of 64 teams divided into
four initial groups. Each team is assigned a base skill rating (Elo), which
represents overall team strength. During each match, an effective performance
rating is calculated to reflect short-term form while remaining bounded by
the team’s base skill.

Group-stage results determine which teams advance to a knockout-style final
tournament. The full tournament process is executed multiple times to evaluate
overall performance consistency and determine a final winner.

## Tournament Structure

- 64 total teams
- Teams divided into 4 initial groups
- Group-stage simulations determine rankings
- Top teams from each group advance to a knockout tournament
- Final tournament produces a winner
- Simulation is repeated multiple times to evaluate outcomes across runs

## Match Simulation Logic

- Each team has a predefined base Elo rating representing long-term strength
- A performance bias and small controlled variance are applied per match to
  derive an effective Elo
- Effective Elo values are intentionally bounded by design through normalized
  inputs and limited swing
- Win probability is calculated using the standard Elo expected-score formula
- Higher-rated teams are favored, while upsets remain possible

This approach balances realism with determinism, ensuring stable simulations
without relying on pure randomness.

## Key Features

- Skill-based match outcomes using Elo-style ratings
- Group-stage and knockout tournament simulation
- Repeatable multi-run tournament execution
- Deterministic structure with controlled randomness
- Console-based output for clarity and debugging

## Architecture Highlights

- Object-oriented modeling of teams, matches, and tournaments
- Separation of simulation logic from output formatting
- Reusable tournament and matchup components
- Rating-driven outcome calculation

## Tech Stack

- C#
- .NET
- Console application

## My Contributions

- Designed the tournament simulation architecture
- Implemented Elo-based skill modeling and performance adjustment logic
- Built group-stage ranking and knockout progression systems
- Created multi-run simulations to evaluate outcome stability
- Structured the project using object-oriented programming principles

## Running the Simulation

This project runs as a console application.
Execute the compiled binary or run the project from Visual Studio to observe
simulated tournament results printed to the console.

The project is intended to demonstrate simulation design, algorithmic logic,
and state management rather than user interface development.
