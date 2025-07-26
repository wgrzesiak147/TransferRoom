import type { Squad } from "../models/Squad";
import type { Player } from "../models/Player";
import styles from "./SquadViewer.module.css";

interface SquadViewerProps {
  squad: Squad;
}

const SquadViewer = ({ squad }: SquadViewerProps) => {
  return (
    <div className={styles.playersGrid}>
      {squad.players.map((player: Player) => (
        <div className={styles.playerCard} key={player.playerId}>
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
