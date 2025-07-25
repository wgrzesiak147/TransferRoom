import { useState } from "react";
import "./App.css";

function App() {
  const [teamName, setTeamName] = useState("");
  const [players, setPlayers] = useState<any[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  const handleSearch = async () => {
    setLoading(true);
    setError("");
    setPlayers([]);

    try {
      const response = await fetch(
        `http://localhost:5000/api/squad?team=${encodeURIComponent(teamName)}`
      );
      if (!response.ok) throw new Error("Failed to fetch");
      const data = await response.json();
      setPlayers(data);
    } catch (err) {
      console.log(err);
      setError("Failed to fetch squad. Please check the team name.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="app-container">
      <h1>Premier League Squad Finder</h1>
      <div className="search-container">
        <input
          type="text"
          placeholder="Enter team name or nickname..."
          value={teamName}
          onChange={(e) => setTeamName(e.target.value)}
        />
        <button onClick={handleSearch}>Search</button>
      </div>

      {loading && <p>Loading...</p>}
      {error && <p className="error">{error}</p>}

      <div className="players-grid">
        {players.map((player) => (
          <div className="player-card" key={player.id}>
            <img src={player.photo} alt={player.name} />
            <h3>
              {player.firstName} {player.lastName}
            </h3>
            <p>{player.position}</p>
            <p>DOB: {player.dateOfBirth}</p>
          </div>
        ))}
      </div>
    </div>
  );
}

export default App;
