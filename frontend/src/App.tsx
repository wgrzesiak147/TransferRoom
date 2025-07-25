import { useState } from "react";
import "./App.css";
import SquadViewer from "./components/SquadViewer";
import { fetchSquad } from "./services/squadService";
import type { Squad } from "./models/Squad";

const seasons = [2020, 2021, 2022, 2023, 2024, 2025];
const paidSeasons = [2024, 2025]; //just workaround

function App() {
  const [teamName, setTeamName] = useState("");
  const [squad, setSquad] = useState<Squad | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [season, setSeason] = useState(2023); //2023 is free in api-football

  const handleSearch = async () => {
    if (!teamName.trim()) return;

    setLoading(true);
    setError("");
    setSquad(null);

    try {
      const squadData = await fetchSquad(teamName, season);
      setSquad(squadData);
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
        <select
          value={season}
          onChange={(e) => setSeason(parseInt(e.target.value))}
        >
          {seasons.map((s) => (
            <option key={s} value={s}>
              {s}/{(s + 1).toString().slice(-2)}
              {paidSeasons.includes(s) ? " (Paid)" : ""}
            </option>
          ))}
        </select>

        <button onClick={handleSearch}>Search</button>
      </div>

      {loading && <p>Loading...</p>}
      {error && <p className="error">{error}</p>}

      {!loading && squad && <SquadViewer squad={squad} />}
    </div>
  );
}

export default App;
