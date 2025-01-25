# RockPaperScissors API

## Features
- **Create Game**: Player can create a game by entering their name and invite another player using a unique game ID that is being generated.
- **Join Game**: Player can join a game by entering a valid game ID and their name.
- **Make Move**: Players can make their move by entering their name and chosen move (rock, scissors, paper).
- **Get Game**: To check the game state you can use Get Game by entering the game ID.
- **In-memory storage**: Games are stored in memory, ensuring no external database dependencies.
- **Custom exception handling**: Error handling with middleware and custom exceptions.
- **Unit tests**: Unit tests covering happy paths and failure scenarios.

---

## Setting Up

1. **Unzip the Project**: Extract the provided ZIP file to your local machine.
2. **Open the Project**: Launch the solution file (`RockPaperScissors.sln`) in Rider or Visual Studio.
3. **Install .NET 8 SDK**: Ensure you have the latest [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) installed.


### Running the Application
1. Open a terminal and navigate to the API project directory.
2. Build and run the application:
   ```bash
   dotnet run
3. If the swagger doesn't automaticly open, open your browser and navigate to http://localhost:5097/swagger/index.html

## API Endpoints
Use the Swagger UI to test the API endpoints and play the game following these steps:

1. **Create Game endpoint**: Enter your name and share the generated ID with a friend.
2. **Join Game endpoint**: Player no 2 enters the Game with the ID that was shared and a name.
3. **Make Move endpoint**: Ensure both player makes a move by entering their chosen name and a valid move (rock, paper, scissor). 
4. **Get Game endpoint**: To check the game state use this endpoint to see who won!

### Create a Game
**POST** `/api/game`

**Request**:
```json
{
  "name": "Player1"
}
```
**Response**:
```json
{
  "id": "unique-game-id",
  "instructions": "Share this game ID with Player 2 to start the game.",
  "message": "Game created successfully."
}
```

### Join a Game
**POST** `/api/game/{id}/join`

**Request**:
```json
{
  "name": "Player2"
}
```
**Response**:
```json
{
 "message": "Player 2 joined successfully."
}
```

### Make Move
**POST** `/api/game/{id}/move`

**Request**:
```json
{
  "name": "Player1",
  "move": "Rock"
}
```
**Response**:
```json
{
  "message": "Move registered successfully."
}
```

### Get Game State
**GET** `/api/game/{id}`

**Response**:
```json
{
 "id": "unique-game-id",
  "player1": "Player1",
  "player2": "Player2",
  "player1Move": "Rock",
  "player2Move": "Scissors",
  "winner": "Player1",
  "isComplete": true
}
```


## Running Tests
From the terminal, navigate to the test project directory

```bash
dotnet test
```

**Test coverage includes:**
- Happy paths
- Failure scenarios
- In case of Tie

**Frameworks used:**
- NUnit
- FakeItEasy
- FluentAssertions

## Improvements

- **Making the Player its own class** would encapsulate player-related data (like name, move) into a single entity, reducing some hardcoded values. It would improve the projects readability and flexibility a bit more. 
- **Set Rock, Paper, Scissors to Enums** would ensure that only valid moves are being used and would also reduce more of the hardcoded values.
- **Add logging** Integrating structured logging (using ILogger for example) would make debugging easier.