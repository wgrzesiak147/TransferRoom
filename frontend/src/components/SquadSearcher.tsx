import { useState } from "react";
import styles from "./SquadSearcher.module.css";
import type { Squad } from "../models/Squad";
import { fetchSquad } from "../services/squadService";
import SquadViewer from "./SquadViewer";
import axios from "axios";

const seasons = [2020, 2021, 2022, 2023, 2024, 2025];
const paidSeasons = [2024, 2025]; //just workaround, api-football required payed subscription for these seasons

const SquadSearcher = () => {
  const [teamName, setTeamName] = useState("");
  const [squad, setSquad] = useState<Squad | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [season, setSeason] = useState(2023);
  const handleSearch = async () => {
    if (!teamName.trim()) return;

    setLoading(true);
    setError("");
    setSquad(null);

    try {
      const squadData = await fetchSquad(teamName, season);
      setSquad(squadData);
    } catch (error) {
      if (axios.isAxiosError(error)) {
        const status = error.response?.status;

        if (status === 401) {
          setError("Unauthorized – API key issue or restricted season.");
        } else if (status === 404) {
          setError("Team not found. Please check the name.");
        } else {
          setError("An unexpected error occurred.");
        }
      } else {
        setError("Something went wrong. Please try again.");
      }

      console.error(error);
    } finally {
      setLoading(false);
    }
  };
  return (
    <>
      <h1>Premier League Squad Finder</h1>

      <div className={styles.searchContainer}>
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
              {paidSeasons.includes(s) ? " (Paid subscription)" : ""}
            </option>
          ))}
        </select>

        <button onClick={handleSearch}>Search</button>
      </div>

      {loading && <p>Loading...</p>}
      {error && <p className={styles.error}>{error}</p>}

      {!loading && squad && <SquadViewer squad={squad} />}
    </>
  );
};

export default SquadSearcher;
