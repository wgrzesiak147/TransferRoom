import { useState } from "react";
import axios from "axios";
import "./App.css";
import type { Squad } from "./models/Squad";
import type { Player } from "./models/Player";

function App() {
  const [teamName, setTeamName] = useState("");
  const [squad, setSquad] = useState<Squad | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  const handleSearch = async () => {
    if (!teamName.trim()) return;

    setLoading(true);
    setError("");
    setSquad(null);

    try {
      const response = await axios.get<Squad>(
        `http://localhost:5231/api/squad?team=${encodeURIComponent(teamName)}`
      );
      setSquad(response.data);
    } catch (err) {
      console.error(err);
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
        {squad?.players.map((player: Player) => (
          <div className="player-card" key={player.playerId}>
            <img
              src={player.photo}
              alt={`${player.firstName} ${player.lastName}`}
            />
            <h3>
              {player.firstName} {player.lastName}
            </h3>
            <p>{player.position}</p>
            <p>
              DOB:{" "}
              {player.dateOfBirth
                ? new Date(player.dateOfBirth).toLocaleDateString()
                : "N/A"}
            </p>
          </div>
        ))}
      </div>
    </div>
  );
}

export default App;
