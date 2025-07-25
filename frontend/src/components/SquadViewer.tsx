import React from "react";
import type { Squad } from "../models/Squad";
import type { Player } from "../models/Player";
import "./SquadViewer.css";

interface SquadViewerProps {
  squad: Squad;
}

const SquadViewer: React.FC<SquadViewerProps> = ({ squad }) => {
  return (
    <div className="players-grid">
      {squad.players.map((player: Player) => (
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
  );
};

export default SquadViewer;
